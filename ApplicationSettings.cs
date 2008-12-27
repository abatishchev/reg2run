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
		ParameterContainer container = new ParameterContainer();

		#region Properties
		public string FileName
		{
			get
			{
				return container[ParameterRole.FileName] as string;
			}
			set
			{
				container[ParameterRole.FileName] = value;
			}
		}

		public string FilePath
		{
			get
			{
				return container[ParameterRole.FilePath] as string;
			}
			set
			{
				container[ParameterRole.FilePath] = value;
			}
		}

		public string FileWorkingDirectory
		{
			get
			{
				return container[ParameterRole.FileWorkingDirectory] as string;
			}
			set
			{
				container[ParameterRole.FileWorkingDirectory] = value;
			}
		}

		public bool RunFlag
		{
			get
			{
				object value = container[ParameterRole.Run];
				return (value as string != null) ? true : (value != null ? (bool)value : false);
			}
			set
			{
				container[ParameterRole.Run] = value;
			}
		}

		public string RunString
		{
			get
			{
				string value = container[ParameterRole.Run] as string;
				return (value != null) ? value : String.Empty;
			}
			set
			{
				container[ParameterRole.Run] = value;
			}
		}

		public bool SelfFlag
		{
			get
			{
				object value = container[ParameterRole.Self];
				return (value != null) ? (bool)value : false;
			}
			set
			{
				container[ParameterRole.Self] = value;
			}
		}

		public bool UsageFlag
		{
			get
			{
				object value = container[ParameterRole.Usage];
				return (value != null) ? (bool)value : false;
			}
			set
			{
				container[ParameterRole.Usage] = value;
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

	class ParameterContainer : Dictionary<ParameterRole, object>
	{
		#region Operators
		public new object this[ParameterRole role]
		{
			get
			{
				if (base.ContainsKey(role))
				{
					return base[role];
				}
				else
				{
					return null; ;
				}
			}
			set
			{
				if (base.ContainsKey(role))
				{
					base[role] = value;
				}
				else
				{
					base.Add(role, value);
				}
			}
		}
		#endregion
	}
}
