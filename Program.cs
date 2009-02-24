// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

using Reg2Run.Errors;
using Reg2Run.Settings;

namespace Reg2Run
{
	static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Core.IsConsole = false;
				try
				{
					var dialog = new OpenFileDialog();
					dialog.AddExtension = true;
					dialog.CheckFileExists = true;
					dialog.DefaultExt = "exe";
					dialog.DereferenceLinks = true;
					dialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
					dialog.FilterIndex = 1;
					dialog.InitialDirectory = Application.StartupPath;
					dialog.Multiselect = false;
					dialog.ReadOnlyChecked = true;
					dialog.Title = "Choose a file to import..";
					dialog.ValidateNames = true;

					switch (dialog.ShowDialog())
					{
						case DialogResult.OK:
							{
								var obj = new ImportObject(dialog.FileName);
								dialog.Dispose();
								if (obj != null)
								{
									var result = MessageBox.Show(String.Format(CultureInfo.CurrentCulture, "Are you shure want to import specified file: '{0}'?", obj.FullPath), Core.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
									if (result == DialogResult.Yes)
									{
										Import(obj);
										MessageBox.Show("Done!", Core.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
									}
									else
									{
										throw new ImportCanceledException();
									}
								}
								else
								{
									// TODO
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
						Core.Settings = ApplicationSettings.Parse(args);
						if (Core.Settings.UsageFlag)
						{
							PrintUsage();
						}
						else
						{
							var obj = ImportObject.Parse(Core.Settings);
							if (obj != null)
							{
								Console.Write(String.Format(CultureInfo.CurrentCulture, "Adding '{0}'.. ", obj.FullPath));
								Import(obj);
								Console.WriteLine("Done!");
								if (obj.Run)
								{
									Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Starting '{0}'..", obj.FullPath));
									Process.Start(obj.FullPath, obj.RunArg);
								}
							}
							else
							{
								//TODO: Console.WriteLine("");
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("Error:");
						Console.WriteLine(ex.Message);
					}
				}
				else
				{
					Console.WriteLine(new TooManyParametersException().Message);
					return;
				}
				if (Core.KeepConsole)
				{
					Console.ReadKey(true);
				}
			}
		}

		private static void Import(ImportObject obj)
		{
			Process.Start(new ProcessStartInfo() { FileName = Core.Assembly.Location, Verb = "runas", UseShellExecute = true });
			//t.InvokeMember("Import", BindingFlags.Default | BindingFlags.InvokeMethod, null, Activator.CreateInstance(t), new object[] { obj });
			Core.Import(obj);
		}

		private static void PrintUsage()
		{
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Usage: reg2run -p PATH [-n NAME] [-d DIR] [-r [PARAM]] | -? | -s"));
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}\t\t{1}", "-?", "Print usage help"));
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}\t\t{1}", "-p", "Add file located in PATH to the registry"));
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}\t\t{1}", "-n", "Save as NAME"));
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}\t\t{1}", "-d", "Set working directory to DIR"));
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}\t\t{1}", "-r", "Run file after import, PARAM as argument if specified"));
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}\t\t{1}", "-s", "Add tool itself to the registry"));
			Console.WriteLine();
			Console.WriteLine("Remarks:");
			Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "\t{0}", "Parameter '-r' as flag must be specified in the end, otherwise next parameter would be recognized as it's value"));
		}
	}
}
