// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Reg2Run.Errors;

namespace Reg2Run.Settings
{
	enum ParameterRole
	{
		FileName,
		FilePath,
		FileWorkingDirectory,
		Run,
		Self,
		Usage
	}

	class ApplicationSettings
	{
		string name, path, dir, runStr;
		bool runFl, self, usage;

		#region Properties
		public string FileName
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string FilePath
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}

		public string FileWorkingDirectory
		{
			get
			{
				return this.dir;
			}
			set
			{
				this.dir = value;
			}
		}

		public bool RunFlag
		{
			get
			{
				return this.runFl;
			}
			set
			{
				this.runFl = value;
			}
		}

		public string RunString
		{
			get
			{
				return this.runStr;
			}
			set
			{
				this.runFl = !String.IsNullOrEmpty(value);
				this.runStr = value;
			}
		}

		public bool SelfFlag
		{
			get
			{
				return this.self;
			}
			set
			{
				this.self = value;
			}
		}

		public bool UsageFlag
		{
			get
			{
				return this.usage;
			}
			set
			{
				this.usage = value;
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
