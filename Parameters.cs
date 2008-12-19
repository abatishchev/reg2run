// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Collections;
using System.Collections.Generic;

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
			"Run file after import",
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
		#endregion
	}
}
