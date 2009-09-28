// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Windows.Forms;

namespace Reg2Run
{
	static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Core.IsConsole = false;
				try
				{
					var dialog = new OpenFileDialog();
					dialog.DefaultExt = "exe";
					dialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
					dialog.InitialDirectory = Application.StartupPath;
					dialog.Title = "Choose a file to import..";

					var dialogResult = dialog.ShowDialog();
					dialog.Dispose();
					if (dialogResult == DialogResult.OK)
					{
						var obj = new ImportObject(dialog.FileName);
						if (MessageBox.Show(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Are you shure want to import specified file: '{0}'?", obj.FullPath), Core.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
						{
							Core.Settings = new ApplicationSettings();
							Core.Import(obj);
							MessageBox.Show("Done!", Core.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
						}
						else
						{
							throw new ImportCanceledException();
						}
					}
					else
					{
						throw new ImportCanceledException();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, Core.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			else
			{
				Core.IsConsole = true;
				Console.WriteLine("{0} version {1}", Core.ApplicationTitle, Core.ApplicationVersion);
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
						// removing from registry
						if ((Core.Settings.ActionTypeMode |= ActionTypeFlag.Remove) == ActionTypeFlag.Remove)
						{
							Console.WriteLine("Deleting: '{0}'", obj.FileName);
							Core.Remove(obj);
							Console.WriteLine("Done.");
						}

						// adding to registry
						if ((Core.Settings.ActionTypeMode |= ActionTypeFlag.Add) == ActionTypeFlag.Add)
						{

							Console.WriteLine(String.Equals(System.IO.Path.GetFileName(obj.FullPath), obj.FileName, StringComparison.OrdinalIgnoreCase) ? "Adding: '{0}'" : "Adding: '{0}' as '{1}'", obj.FullPath, obj.FileName);
							Core.Import(obj);
							Console.WriteLine("Done.");
							if (obj.Run)
							{
								Console.WriteLine(String.IsNullOrEmpty(obj.RunArg) ? "Running: '{0}'" : "Running: '{0} {1}'", obj.FullPath, obj.RunArg);
								System.Diagnostics.Process.Start(obj.FullPath, obj.RunArg);
							}
						}
					}
				}
				// TODO: test
				catch (NullReferenceException)
				{
					throw new Exception("No object to import");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error:");
					Console.WriteLine(ex.Message);
				}
				if (Core.KeepConsole)
				{
					Console.ReadKey(true);
				}
			}
		}

		private static void PrintUsage()
		{
			string format;
			Console.WriteLine("Usage: reg2run [PATH] | -p PATH [-n NAME] [-d DIR] [--hkcu] [--hklm] [-r [PARAM]] | -s | -?");
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
			Console.WriteLine(format, "If single specified parameter is an existed application path, it will be imported, all other parameters are ignored");
			Console.WriteLine(format, "If no registry hive indicated, both are assumed");
			Console.WriteLine(format, "Parameter '-r' as flag must be specified in the end, otherwise next parameter will be recognized as it's value");
			Console.WriteLine(format, "If parameter '-?' is specified, all other are ignored");
			Console.WriteLine();
			Console.WriteLine("Examples:");
			Console.WriteLine(format, "reg2run -p \"C:\\Program Files\\Far Manager\\far.exe\" -d C:\\");
			Console.WriteLine(format, "reg2run -p \"C:\\Program Files\\Mozilla Firefox\\firefox.exe\" -n ff -r \"http://reg2run.sf.net\"");
			Console.WriteLine(format, "reg2run -s -n rr");
			Console.WriteLine(format, "reg2run \"C:\\Windows\\reg2run.exe\"");
		}
	}
}
