// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;

using Reg2Run.Parameters;

namespace Reg2Run.Errors
{
	public abstract class ParameterException : Exception
	{
		public ParameterException(string message)
			: base(message)
		{
			//
		}
	}

	#region Parameters
	public class ParameterAlreadySetException : ParameterException
	{
		public ParameterAlreadySetException(ParameterRole role) :
			base(String.Format(CultureInfo.CurrentCulture, "Parameter '{0}' was already specified", Core.ParameterContainer[role].Name))
		{
			//
		}
	}

	public class ParameterMissedException : ParameterException
	{
		public ParameterMissedException(ParameterRole role)
			: base(String.Format(CultureInfo.CurrentCulture, "Value for required parameter '{0}' was not specified", Core.ParameterContainer[role].Name))
		{
			//
		}
	}

	public class ParameterNotSetException : ParameterException
	{
		public ParameterNotSetException(ParameterRole role)
			: base(String.Format(CultureInfo.CurrentCulture, "Parameter '{0}' was provided without assigning it's value", Core.ParameterContainer[role].Name))
		{
			//
		}
	}

	public class UnknownParameterException : Exception
	{
		string parameterName;

		public UnknownParameterException(string param)
			: base("Unknown parameter '{0}' was specified")
		{
			this.parameterName = param;
		}

		public string ParameterName
		{
			get { return parameterName; }
		}
	}

	public class TooManyParametersException : Exception
	{
		public TooManyParametersException()
			: base("Too many parameters was specified")
		{
			//
		}
	}
	#endregion

	public class ExternalCallException : Exception
	{
		public ExternalCallException(string name)
			: base(String.Format(CultureInfo.CurrentCulture, "An error occured while call of external method '{0}' ", name))
		{
			//			
		}
	}

	public class ImportCanceledException : Exception
	{
		public ImportCanceledException()
			: base("Importing was canceled by user")
		{
			//
		}
	}

	public class NotExecutableExecption : Exception
	{
		public NotExecutableExecption(string name)
			: base(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' is not an executable", name))
		{
			//
		}
	}
}
