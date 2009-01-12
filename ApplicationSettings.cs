// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;

using Reg2Run.Errors;

namespace Reg2Run.Settings
{
	class ApplicationSettings
	{
		string fileName, filePath, runString, fileWorkingDirectory;
		bool runFlag, selfFlag, usageFlag;

		#region Properties
		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
				this.fileName = value;
			}
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
			set
			{
				this.filePath = value;
			}
		}

		public string FileWorkingDirectory
		{
			get
			{
				return this.fileWorkingDirectory;
			}
			set
			{
				this.fileWorkingDirectory = value;
			}
		}

		public bool RunFlag
		{
			get
			{
				return this.runFlag;
			}
			set
			{
				this.runFlag = value;
			}
		}

		public string RunString
		{
			get
			{
				return this.runString;
			}
			set
			{
				this.runFlag = !String.IsNullOrEmpty(value);
				this.runString = value;
			}
		}

		public bool SelfFlag
		{
			get
			{
				return this.selfFlag;
			}
			set
			{
				this.selfFlag = value;
			}
		}

		public bool UsageFlag
		{
			get
			{
				return this.usageFlag;
			}
			set
			{
				this.usageFlag = value;
			}
		}
		#endregion

		#region Methods
		public static ApplicationSettings Parse(string[] args)
		{
			ApplicationSettings settings = new ApplicationSettings();
			for (int i = 0; i < args.Length; i++)
			{
				string name = args[i];
				switch (name)
				{
					case "-?":
					case "/?":
						{
							settings.UsageFlag = true;
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
								throw new ParameterNotSetException("File working directory");
							}
							break;
						}
					default:
						{
							throw new UnknownParameterException(name);
						}
				}
			}
			return settings;
		}
		#endregion
	}
}
