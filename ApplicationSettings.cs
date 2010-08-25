// Copyright (C) 2005-2010 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;

namespace Reg2Run
{
	class ApplicationSettings
	{
		#region Constructors
		public ApplicationSettings()
		{
			this.ActionTypeMode = ActionTypeFlag.Add;
			this.RegistryWriteMode = RegistryWriteFlag.HKCU | RegistryWriteFlag.HKLM;
		}
		#endregion

		#region Properties
		public ActionTypeFlag ActionTypeMode { get; set; }

		public string FileName { get; set; }

		public string FilePath { get; set; }

		public bool Force { get; set; }

		public RegistryWriteFlag RegistryWriteMode { get; set; }

		public bool RunFlag { get; set; }

		public string RunString { get; set; }

		public bool SelfFlag { get; set; }

		public bool UsageFlag { get; set; }

		public string WorkingDirectory { get; set; }
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
					case "--add":
						{
							settings.ActionTypeMode |= ActionTypeFlag.Add;
							break;
						}
					case "-d":
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
							settings.ActionTypeMode |= ActionTypeFlag.Remove;
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
