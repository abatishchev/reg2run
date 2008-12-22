// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Reg2Run.Errors;

namespace Reg2Run.Settings
{
	public enum ParameterRole
	{
		FileName,
		FilePath,
		FileWorkingDir,
		Run,
		Self,
		Usage
	}

	class ParameterContainer : Dictionary<ParameterRole, object>
	{
		#region Operators
		public new object this[ParameterRole role]
		{
			get
			{
				object value;
				this.TryGetValue(role, out value);
				return value;
			}
		}
		#endregion

		#region Methods
		public void Parse(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];
				switch (arg)
				{
					case "-?":
					case "/?":
						{
							this.Add(ParameterRole.Usage, true);
							break;
						}
					case "-n":
						{
							try
							{
								this.Add(ParameterRole.FileName, args.GetValue(++i) as string);
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
								this.Add(ParameterRole.FilePath, args.GetValue(++i) as string);
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException("File path");
							}
							break;
						}
					case "-r":
						{
							object run;
							try
							{
								run = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								run = true;
							}
							this.Add(ParameterRole.Run, run);
							break;
						}
					case "-s":
						{
							this.Add(ParameterRole.Self, true);
							break;
						}
					case "-w":
						{
							try
							{
								this.Add(ParameterRole.FileWorkingDir, args.GetValue(++i) as string);
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException("File working directory");
							}
							break;
						}
					default:
						{
							throw new UnknownParameterException(arg);
						}
				}
			}
		}
		#endregion
	}
}
