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
			Core.Import(obj);
		}

		private static void PrintUsage()
		{
			string format;
			Console.WriteLine("Usage: reg2run -p PATH [-n NAME] [-d DIR] [--hkcu] [--hklm] [-r [PARAM]] | -s | -?");
			Console.WriteLine();
			Console.WriteLine("Options:");
			format = "\t{0}\t\t{1}";
			Console.WriteLine(format, "-?", "Print usage help");
			Console.WriteLine(format, "-p", "Add file located in PATH to the registry");
			Console.WriteLine(format, "-n", "Save as NAME");
			Console.WriteLine(format, "-d", "Set working directory to DIR");
			Console.WriteLine(format, "-r", "Run file after import, PARAM as argument if specified");
			Console.WriteLine(format, "-s", "Add tool itself to the registry");
			Console.WriteLine(format, "--hkcu", "Write into HKEY_CURRENT_USER registry hive");
			Console.WriteLine(format, "--hklm", "Write into HKEY_LOCAL_MACHINE registry hive");
			Console.WriteLine();
			Console.WriteLine("Remarks:");
			format = "\t{0}";
			Console.WriteLine(format, "Parameter '-r' as flag must be specified in the end, otherwise next parameter will be recognized as it's value");
			Console.WriteLine(format, "If no registry hive indicated, both are assumed");
			Console.WriteLine(format, "If no parameter '-?' is specified, all other are ignored");
			Console.WriteLine();
			Console.WriteLine("Examples:");
			Console.WriteLine(format, "reg2run -p \"C:\\Cygwin\\bin\\grep.exe\" -d C:\\");
			Console.WriteLine(format, "reg2run -p \"C:\\Program Files\\Internet Explorer\\iexplore.exe\" -n ie");
			Console.WriteLine(format, "reg2run -p \"D:\\Program Files\\Mozilla Firefox\\firefox.exe\" -n ff -r \"http://reg2run.sf.net\"");
			Console.WriteLine(format, "reg2run -s -n rr");
		}
	}
}
