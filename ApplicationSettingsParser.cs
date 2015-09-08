// Copyright (C) 2005-2015 Alexander Batishchev (abatishchev at gmail.com)

using System;

namespace Reg2Run
{
	static class ApplicationSettingsParser
	{
		#region Methods
		public static ApplicationSettings Parse(string[] args)
		{
			if (args.Length == 1 && (!args[0].StartsWith("/") || !args[0].StartsWith("-")) && System.IO.File.Exists(args[0]))
			{
				return new ApplicationSettings { FilePath = args[0] };
			}
			var settings = new ApplicationSettings();
			for (int i = 0; i < args.Length; i++)
			{
				var param = args[i];
				switch (param)
				{
					case "-?":
					case "/?":
						{
							return new ApplicationSettings() { UsageFlag = true };  // stop further parsing and return only meaning flag
						}
					case "--add":
						{
							settings.ActionTypeMode = ActionTypeFlag.Add;
							break;
						}
					case "--d":
						{
							try
							{
								settings.WorkingDirectory = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException("Application working directory");
							}
							break;
						}
					case "--engage":
						{
							settings.EngageFlag = true;
							break;
						}
					case "-f":
						{
							settings.Force = true;
							break;
						}
					case "--hkcu":
						{
							settings.RegistryWriteMode |= RegistryWriteFlag.HKCU;
							break;
						}
					case "--hklm":
						{
							settings.RegistryWriteMode |= RegistryWriteFlag.HKLM;
							break;
						}
					case "-l":
						{
							System.Windows.Forms.MessageBox.Show(Environment.CurrentDirectory, Core.ApplicationName);
							Environment.Exit(0);
							break;
						}
					case "-n":
						{
							try
							{
								settings.FileName = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException("File name");
							}
							break;
						}
					case "-p":
						{
							try
							{
								settings.FilePath = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException("File path");
							}
							break;
						}
					case "-r":
						{
							settings.RunFlag = true;
							try
							{
								settings.RunString = args.GetValue(++i) as string;
							}
							catch
							{
								// do nothing
							}
							break;
						}
					case "--remove":
						{
							settings.ActionTypeMode = ActionTypeFlag.Remove;
							break;
						}
					case "-s":
						{
							settings.SelfFlag = true;
							break;
						}
					default:
						{
							throw new UnknownParameterException(param);
						}
				}
			}
			return settings;
		}
		#endregion
	}
}
