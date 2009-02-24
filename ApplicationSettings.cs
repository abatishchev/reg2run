// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;

using Reg2Run.Errors;

namespace Reg2Run.Settings
{
	class ApplicationSettings
	{
		private string runString;

		#region Properties
		public string FileName { get; set; }

		public string FilePath { get; set; }

		public string FileWorkingDirectory { get; set; }

		public bool RunFlag { get; set; }

		public string RunString
		{
			get
			{
				return this.runString;
			}
			set
			{
				this.RunFlag = !String.IsNullOrEmpty(value);
				this.runString = value;
			}
		}

		public bool SelfFlag { get; set; }

		public bool UsageFlag { get; set; }

		public RegistryWriteFlag RegistryWriteMode { get; set; }
		#endregion

		#region Methods
		internal static ApplicationSettings Parse(string[] args)
		{
			var settings = new ApplicationSettings();
			for (int i = 0; i < args.Length; i++)
			{
				string name = args[i];
				switch (name)
				{
					case "-?":
					case "/?":
						{
							return new ApplicationSettings { UsageFlag = true }; // stop further parsing and return only meaning flag
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
					case "-w":
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
					default:
						{
							throw new UnknownParameterException(name);
						}
				}
			}
			if (settings.RegistryWriteMode == 0)
			{
				settings.RegistryWriteMode = RegistryWriteFlag.HKCU | RegistryWriteFlag.HKLM;
			}
			return settings;
		}
		#endregion
	}
}
