// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using Reg2Run.Errors;
using Reg2Run.Parameters;

namespace Reg2Run
{
	static class ConsoleStub
	{
		[STAThread]
		static void Main(string[] args)
		{
			Core.Init();
			if (args.Length == 0)
			{
				//TODO:
				Core.CurrentContext = Core.Context.Windows;
				try
				{
					OpenFileDialog dialog = new OpenFileDialog();
					dialog.AddExtension = true;
					dialog.CheckFileExists = true;
					dialog.DefaultExt = "exe";
					dialog.DereferenceLinks = true;
					dialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
					dialog.FilterIndex = 2;
					dialog.InitialDirectory = Application.StartupPath;
					dialog.Multiselect = false;
					dialog.ReadOnlyChecked = true;
					dialog.Title = "Choose a file to import..";
					dialog.ValidateNames = true;

					switch (dialog.ShowDialog())
					{
						case DialogResult.OK:
							{
								ImportObject obj = new ImportObject(dialog.FileName);
								dialog.Dispose();
								DialogResult result = MessageBox.Show(String.Format(CultureInfo.CurrentCulture, "Are you shure want to import specified file: '{0}'?", obj.FullPath), Core.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
								if (result == DialogResult.Yes)
								{
									Core.Import(obj);
									MessageBox.Show("Done!", Core.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
								}
								else
								{
									throw new ImportCanceledException();
								}
								break;
							}
						case DialogResult.Cancel:
							{
								throw new ImportCanceledException();
							}
					}
				}
				catch (ImportCanceledException ex)
				{
					MessageBox.Show(ex.Message, Core.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			else
			{
				//TODO:
				Core.CurrentContext = Core.Context.Console;
				if (args.Length >= 1 & args.Length <= 7)
				{
					Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0} version {1}", Core.ApplicationTitle, Core.ApplicationVersion));
					Console.WriteLine(Core.ApplicationCopyright);
					Console.WriteLine();
					try
					{
						ReadParameters(args);
						ProcessParameters();
					}
					catch (Exception ex)
					{
						Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Error! {0}", ex.Message));
					}
				}
				else
				{
					Console.WriteLine(new TooManyParametersException().Message);
					return;
				}
			}
		}

		static void ProcessParameters()
		{
			ImportObject obj = null;
			object temp = null;

			bool self = false;
			bool run = false;

			temp = ReadParameter(ParameterRole.Usage);
			if (temp != null)
			{
				bool showUsage = (bool)temp;
				if (showUsage)
				{
					PrintUsage();
					return;
				}
			}

			temp = ReadParameter(ParameterRole.Self);
			if (temp != null)
			{
				self = (bool)temp;
				obj = new ImportObject(Core.Assembly.Location);
			}

			if (!self)
			{
				temp = ReadParameter(ParameterRole.FilePath);
				if (temp != null && !String.IsNullOrEmpty((string)temp))
				{
					if (!self)
					{
						string path = (string)temp;
						try
						{
							FileInfo info = new FileInfo(path);
							if (info.Exists)
							{
								if (String.Equals(info.Extension, ".exe"))
								{
									obj = new ImportObject(path);
								}
								else
								{
									throw new NotExecatableExecption(path);
								}
							}
							else
							{
								throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' doesn't exists", path));
							}
						}
						catch (ArgumentException)
						{
							throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' doesn't exists", path));
						}
					}

					temp = ReadParameter(ParameterRole.FileName);
					if (temp != null)
					{
						string name = (string)temp;
						if (String.IsNullOrEmpty(new FileInfo(name).Extension))
						{
							name = name + ".exe";
						}
						else if (!String.Equals(new FileInfo(name).Extension, ".exe"))
						{
							throw new NotExecatableExecption(name);
						}
						obj.NewName = name;
					}

					temp = ReadParameter(ParameterRole.FileWorkingDir);
					{
						if (temp != null)
						{
							string dir = (string)temp;
							try
							{
								DirectoryInfo info = new DirectoryInfo(dir);
								if (info.Exists)
								{
									obj.WorkingDirectory = info.FullName;
								}
								else
								{
									throw new DirectoryNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified directory '{0}' doesn't exists", dir));

								}
							}
							catch (ArgumentException)
							{
								throw new DirectoryNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified directory '{0}' doesn't exists", dir));
							}
						}
					}

					temp = ReadParameter(ParameterRole.Run);
					if (temp != null)
					{
						run = (bool)temp;
					}
				}
				else
				{
					throw new ParameterMissedException(ParameterRole.FilePath);
				}
			}
			if (obj != null)
			{
				try
				{
					Console.Write(String.Format(CultureInfo.CurrentCulture, "Adding '{0}'.. ", obj.FileName));
					Core.Import(obj);
					Console.WriteLine("Done!");
					if (run)
					{
						Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Starting '{0}'..", obj.FileName));
						Process.Start(obj.FullPath);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Error! '{0}'..", ex.Message));
					return;
				}
			}
		}

		static void ReadParameters(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				foreach (Parameter p in Core.ParameterContainer.Values)
				{
					if (String.Equals(args[i], p.ArgumentSwitch))
					{
						switch (p.Type)
						{
							case ParameterType.Flag:
								{
									Core.ParameterContainer[p.Role].Value = true;
									break;
								}
							case ParameterType.Value:
								{
									try
									{
										Core.ParameterContainer[p.Role].Value = args.GetValue(++i);
									}
									catch (IndexOutOfRangeException)
									{
										throw new ParameterNotSetException(p.Role);
									}
									break;
								}
						}
					}
				}
			}
		}

		static object ReadParameter(ParameterRole role)
		{
			object value = null;
			if (Core.ParameterContainer.Contains(role))
			{
				value = Core.ParameterContainer[role].Value;
			}
			return value;
		}

		static void PrintUsage()
		{
			Console.WriteLine("Usage: reg2run [-r] [-n NAME] [-w DIR] -p PATH");
			Console.WriteLine();
			Console.WriteLine("Options:");
			foreach (Parameter p in Core.ParameterContainer.Values)
			{
				Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}\t\t{1}", p.ArgumentSwitch, p.Description));
			}
		}
	}
}
