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
				Core.IsConsole = false;
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
				Core.IsConsole = true;
				if (args.Length >= 1 & args.Length <= 8)
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
			Console.ReadKey(true);
		}

		static void ProcessParameters()
		{
			ImportObject obj = null;

			bool self = false;
			bool runFlag = false;
			string runArg = null;

			object tempUsage = ReadParameter(ParameterRole.Usage);
			if (tempUsage != null)
			{
				if ((bool)tempUsage)
				{
					PrintUsage();
					return;
				}
			}

			object tempSelf = ReadParameter(ParameterRole.Self);
			if (tempSelf != null)
			{
				self = (bool)tempSelf;
				obj = new ImportObject(Core.Assembly.Location);
			}

			if (!self)
			{
				string path = ReadParameter(ParameterRole.FilePath) as string;
				if (path != null)
				{
					if (!self)
					{
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
									throw new NotExecutableException(path);
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

					string name = ReadParameter(ParameterRole.FileName) as string;
					if (name != null)
					{
						if (String.IsNullOrEmpty(new FileInfo(name).Extension))
						{
							name = name + ".exe";
						}
						else if (!String.Equals(new FileInfo(name).Extension, ".exe"))
						{
							throw new NotExecutableException(name);
						}
						obj.NewName = name;
					}

					string dir = ReadParameter(ParameterRole.FileWorkingDir) as string;
					if (dir != null)
					{
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

					object tempRun = ReadParameter(ParameterRole.Run);
					runArg = tempRun as string;
					if (runArg != null)
					{
						runFlag = true;
					}
					else if (tempRun is bool)
					{
						runFlag = (bool)tempRun;
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
					if (runFlag)
					{
						Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Starting '{0}'..", obj.FileName));
						Process.Start(obj.FullPath, runArg);
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
				string arg = args[i];
				switch (arg)
				{
					case "-?":
						{
							Core.ParameterContainer[ParameterRole.Usage].Value = true;
							break;
						}
					case "-n":
						{
							try
							{
								Core.ParameterContainer[ParameterRole.FileName].Value = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException(ParameterRole.FileName);
							}
							break;
						}
					case "-p":
						{
							try
							{
								Core.ParameterContainer[ParameterRole.FilePath].Value = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException(ParameterRole.FilePath);
							}
							break;
						}
					case "-r":
						{
							object run;
							try
							{
								run = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								run = true;
							}
							Core.ParameterContainer[ParameterRole.Run].Value = run;
							break;
						}
					case "-s":
						{
							Core.ParameterContainer[ParameterRole.Self].Value = true;
							break;
						}
					case "-w":
						{
							try
							{
								Core.ParameterContainer[ParameterRole.FileWorkingDir].Value = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException(ParameterRole.FileWorkingDir);
							}
							break;
						}
					default:
						{
							throw new UnknownParameterException(arg);
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
			Console.WriteLine("Usage: reg2run [-r [PARAM]] [-n NAME] [-w DIR] -p PATH");
			Console.WriteLine();
			Console.WriteLine("Options:");
			foreach (Parameter p in Core.ParameterContainer.Values)
			{
				Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}\t\t{1}", p.ArgumentSwitch, p.Description));
			}
		}
	}
}
