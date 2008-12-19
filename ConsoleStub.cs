// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Diagnostics;
using System.Globalization;
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
								DialogResult result = MessageBox.Show(String.Format(CultureInfo.CurrentCulture, "Are you shure want to import specified file: '{0}'?", obj.FullName), Core.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
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
						Core.ParameterContainer.Parse(args);

						object tempUsage = Core.ParameterContainer.ReadParameter(ParameterRole.Usage);
						if (tempUsage != null && (bool)tempUsage)
						{
							PrintUsage();
							return;
						}

						ImportObject obj = ImportObject.Parse(Core.ParameterContainer);
						if (obj != null)
						{
							try
							{
								Console.Write(String.Format(CultureInfo.CurrentCulture, "Adding '{0}'.. ", obj.FileName));
								Core.Import(obj);
								Console.WriteLine("Done!");
								if (obj.Run)
								{
									Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Starting '{0}'..", obj.FileName));
									Process.Start(obj.FullName, obj.RunArg);
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine(String.Format(CultureInfo.CurrentCulture, "Error! '{0}'..", ex.Message));
								return;
							}
						}
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
			//Console.ReadKey(true);
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
