﻿// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Collections;

using Reg2Run.Errors;

namespace Reg2Run.Parameters
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

	public abstract class Parameter
	{
		ParameterRole role;

		string name, description, argumentSwitch;
		object value;

		#region Constructor
		protected Parameter(
			ParameterRole role,
			string name,
			string description,
			string argumentSwitch)
		{
			this.role = role;
			this.name = name;
			this.description = description;
			this.argumentSwitch = argumentSwitch;
		}

		protected Parameter(object value)
		{
			this.value = value;
		}
		#endregion

		#region Properties
		public string ArgumentSwitch
		{
			get { return this.argumentSwitch; }
		}

		public string Description
		{
			get { return this.description; }
		}

		public string Name
		{
			get { return this.name; }
		}

		public ParameterRole Role
		{
			get { return this.role; }
		}

		public object Value
		{
			get { return this.value; }
			set { this.value = value; }
		}
		#endregion
	}

	#region Inherited classes
	class FileNameParameter : Parameter
	{
		public FileNameParameter()
			: base(
			ParameterRole.FileName,
			"Name",
			"Save under given NAME",
			"-n"
		)
		{
			//
		}
	}

	class FilePathParameter : Parameter
	{
		public FilePathParameter()
			: base(
			ParameterRole.FilePath,
			"Path",
			"Add file located in PATH to the registry",
			"-p"
		)
		{
			//
		}
	}

	class FileWorkingDiretoryParameter : Parameter
	{
		public FileWorkingDiretoryParameter()
			: base(ParameterRole.FileWorkingDir,
			"Working directory",
			"Set working directory to DIR",
			"-w")
		{
			//
		}
	}

	class RunParameter : Parameter
	{
		public RunParameter()
			: base(
			ParameterRole.Run,
			"Run",
			"Run file after import with PARAM as command line argument",
			"-r"
			)
		{
			//
		}
	}

	class SelfParameter : Parameter
	{
		public SelfParameter()
			: base(
			ParameterRole.Self,
			"Self",
			"Add tool itself to the registry",
			"-s"
			)
		{
			//
		}
	}

	class UsageParameter : Parameter
	{
		public UsageParameter()
			: base(
			ParameterRole.Usage,
			"Help",
			"Print usage help",
			"-?"
			)
		{
			//
		}
	}
	#endregion

	class ParameterContainer : DictionaryBase
	{
		#region Properties
		public ICollection Values
		{
			get
			{
				return Dictionary.Values;
			}
		}
		#endregion

		#region Operators
		public Parameter this[ParameterRole role]
		{
			get
			{
				return (Parameter)Dictionary[role];
			}
		}
		#endregion

		#region Methods
		public void AddParameter(Parameter param)
		{
			Dictionary.Add(param.Role, param);
		}

		public bool Contains(ParameterRole role)
		{
			return Dictionary.Contains(role);
		}

		public void Parse(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];
				switch (arg)
				{
					case "-?":
						{
							this[ParameterRole.Usage].Value = true;
							break;
						}
					case "-n":
						{
							try
							{
								this[ParameterRole.FileName].Value = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException(ParameterRole.FileName);
							}
							break;
						}
					case "-p":
						{
							try
							{
								this[ParameterRole.FilePath].Value = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException(ParameterRole.FilePath);
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
							this[ParameterRole.Run].Value = run;
							break;
						}
					case "-s":
						{
							this[ParameterRole.Self].Value = true;
							break;
						}
					case "-w":
						{
							try
							{
								this[ParameterRole.FileWorkingDir].Value = args.GetValue(++i) as string;
							}
							catch (IndexOutOfRangeException)
							{
								throw new ParameterNotSetException(ParameterRole.FileWorkingDir);
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

		public object ReadParameter(ParameterRole role)
		{
			object value = null;
			if (this.Contains(role))
			{
				value = this[role].Value;
			}
			return value;
		}
		#endregion
	}
}
