// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;
using System.Security.Permissions;
using System.Runtime.Serialization;

using Reg2Run.Parameters;

namespace Reg2Run.Errors
{
	[Serializable]
	public class ParameterException : Exception
	{
		public ParameterException(string message)
			: base(message)
		{
			//
		}
	}

	#region Parameters
	[Serializable]
	public class ParameterAlreadySetException : ParameterException
	{
		public ParameterAlreadySetException(ParameterRole role) :
			base(String.Format(CultureInfo.CurrentCulture, "Parameter '{0}' was already specified", Core.ParameterContainer[role].Name))
		{
			//
		}
	}

	[Serializable]
	public class ParameterMissedException : ParameterException
	{
		public ParameterMissedException(ParameterRole role)
			: base(String.Format(CultureInfo.CurrentCulture, "Value for required parameter '{0}' was not specified", Core.ParameterContainer[role].Name))
		{
			//
		}
	}

	[Serializable]
	public class ParameterNotSetException : ParameterException
	{
		public ParameterNotSetException(ParameterRole role)
			: base(String.Format(CultureInfo.CurrentCulture, "Parameter '{0}' was provided without assigning it's value", Core.ParameterContainer[role].Name))
		{
			//
		}
	}

	[Serializable]
	public class UnknownParameterException : Exception
	{
		string parameterName;

		public UnknownParameterException(string name)
			: base("Unknown parameter '{0}' was specified")
		{
			this.parameterName = name;
		}

		public string ParameterName
		{
			get { return parameterName; }
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (ParameterName != null)
			{
				info.AddValue("name", ParameterName);
			}
			else
			{
				throw new ArgumentNullException("name");
			}
		}
	}

	[Serializable]
	public class TooManyParametersException : Exception
	{
		public TooManyParametersException()
			: base("Too many parameters was specified")
		{
			//
		}
	}
	#endregion

	[Serializable]
	public class ExternalCallException : Exception
	{
		public ExternalCallException(string name)
			: base(String.Format(CultureInfo.CurrentCulture, "An error occured while call of external method '{0}' ", name))
		{
			//			
		}
	}

	[Serializable]
	public class ImportCanceledException : Exception
	{
		public ImportCanceledException()
			: base("Importing was canceled by user")
		{
			//
		}
	}

	[Serializable]
	public class NotExecutableException : Exception
	{
		public NotExecutableException(string name)
			: base(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' is not an executable", name))
		{
			//
		}
	}
}
