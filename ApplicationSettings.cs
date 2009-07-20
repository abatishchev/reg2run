// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;

using Reg2Run.Errors;

namespace Reg2Run.Settings
{
	class ApplicationSettings
	{
		#region Constructors
		public ApplicationSettings()
		{
			this.RegistryWriteMode = RegistryWriteFlag.HKCU | RegistryWriteFlag.HKLM;
		}
		#endregion

		#region Properties
		public string FileName { get; set; }

		public string FilePath { get; set; }

		public string FileWorkingDirectory { get; set; }

		public bool RunFlag { get; set; }

		public string RunString { get; set; }

		public bool SelfFlag { get; set; }

		public bool SkipExistenceCheck { get; set; }

		public bool UsageFlag { get; set; }

		public RegistryWriteFlag RegistryWriteMode { get; set; }
		#endregion

		#region Methods
		public static ApplicationSettings Parse(string[] args)
		{
			var settings = new ApplicationSettings();
			if (args.Length == 1 && (!args[0].StartsWith("/") || !args[0].StartsWith("-")) && System.IO.File.Exists(args[0]))
			{
				settings.FilePath = args[0];
				return settings;
			}
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
					case "-d":
						{
							try
							{
								settings.FileWorkingDirectory = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException("Application working directory");
							}
							break;
						}
					case "-f":
						{
							settings.SkipExistenceCheck = true;
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
							try
							{
								settings.RunFlag = true;
								settings.RunString = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								settings.RunFlag = true;
							}
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
