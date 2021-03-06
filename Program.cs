// Copyright (C) 2005-2015 Alexander Batishchev (abatishchev at gmail.com)

using System;
using System.Diagnostics;
using System.Linq;
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
				ManualConsole.Hide();
				try
				{
					using (var dialog = new OpenFileDialog())
					{
						dialog.DefaultExt = "exe";
						dialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
						dialog.Title = "Choose file to import..";

						var dialogResult = dialog.ShowDialog();
						if (dialogResult == DialogResult.OK)
						{
							var obj = new ImportObject(dialog.FileName);
							if (MessageBox.Show(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Are you shure want to import specified file: '{0}'?", obj.FullPath), Core.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
							{
								Core.Settings = new ApplicationSettings();
								Core.Add(obj);
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
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, Core.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
			else
			{
				Console.WriteLine("{0} version {1}", Core.ApplicationTitle, Core.ApplicationVersion);
				Console.WriteLine(Core.ApplicationCopyright);
				Console.WriteLine();
				try
				{
					Core.Settings = ApplicationSettingsParser.Parse(args);
					if (Core.Settings.UsageFlag)
					{
						PrintUsage();
					}
					else if (!Core.Settings.EngageFlag)
					{
						var info = new ProcessStartInfo(
							System.Reflection.Assembly.GetEntryAssembly().Location,
							String.Join(" ", Enumerable.Concat(new[] { "--engage" }, args).Escape()))
						{
							UseShellExecute = true,
							Verb = "runas",
						};

						var process = new Process
						{
							EnableRaisingEvents = true,
							StartInfo = info
						};

						process.Start();
						process.WaitForExit();
					}
					else if (Core.Settings.EngageFlag)
					{
						if (!Core.IsElevated)
						{
							Console.WriteLine("Implying elevated privileges not obtained");
						}

						var obj = ImportObjectParser.Parse(Core.Settings);
						Action<ImportObject> action;
						if (new System.Collections.Generic.Dictionary<ActionTypeFlag, Action<ImportObject>>
						{
							// adding to registry
							{
								ActionTypeFlag.Add, (o) =>
								{
									Console.WriteLine(String.Equals(System.IO.Path.GetFileName(obj.FullPath), obj.FileName, StringComparison.OrdinalIgnoreCase) ? "Adding: '{0}'" : "Adding: '{0}' as '{1}'", obj.FullPath, obj.FileName);
									Core.Add(obj);
									Console.WriteLine("Done.");
									if (o.Run)
									{
										Console.WriteLine(String.IsNullOrEmpty(obj.RunArg) ? "Running: '{0}'" : "Running: '{0} {1}'", obj.FullPath, obj.RunArg);
										Process.Start(obj.FullPath, obj.RunArg);
									}
								}
							},
							// removing from registry
							{
								ActionTypeFlag.Remove, (o) =>
								{
									Console.WriteLine("Deleting: '{0}'", o.FileName);
									Core.Remove(obj);
									Console.WriteLine("Done.");
								}
							}
						}
						.TryGetValue(Core.Settings.ActionTypeMode, out action))
						{
							action(obj);
						}
						if (!obj.Run)
						{
							Console.ReadKey(true);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error:");
					Console.WriteLine(ex.Message);
					Console.ReadKey(true);
				}
			}
		}

		private static void PrintUsage()
		{
			string format;
			Console.WriteLine("Usage:");
			Console.WriteLine("reg2run [PATH]");
			Console.WriteLine("reg2run -p PATH [-n NAME] [-d DIR] [--hkcu] [--hklm] [--engage] [-r [PARAM]]");
			Console.WriteLine("reg2run -s [-n NAME] [-r [PARAM]]");
			Console.WriteLine("reg2run -?");
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
			format = "\t{0}\t{1}"; // formatting workaround
			Console.WriteLine(format, "--engage", "Perform an action implying elevated privileges obtained");
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
