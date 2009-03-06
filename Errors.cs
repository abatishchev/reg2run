// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Reg2Run.Errors
{
	[Serializable]
	public class ParameterMissedException : ArgumentNullException
	{
		public ParameterMissedException(string name)
			: base(name, String.Format(CultureInfo.CurrentCulture, "Value for required parameter '{0}' was not specified", name)) { }
	}

	[Serializable]
	public class ParameterNotSetException : ArgumentNullException
	{
		public ParameterNotSetException(string name)
			: base(name, String.Format(CultureInfo.CurrentCulture, "Parameter '{0}' was provided without assigning it's value", name)) { }
	}

	[Serializable]
	public class UnknownParameterException : ArgumentException
	{
		public UnknownParameterException(string name)
			: base(String.Format(CultureInfo.CurrentCulture, "Unknown parameter '{0}' was specified", name), name) { }
	}

	[Serializable]
	public class PInvokeException : Exception
	{
		public PInvokeException(string methodName)
			: base(String.Format(CultureInfo.CurrentCulture, "An error occured while external method '{0}' call", methodName)) { }
	}

	[Serializable]
	public class ImportCanceledException : Exception
	{
		public ImportCanceledException()
			: base("Importing was canceled by user") { }
	}

	[Serializable]
	public class NotExecutableException : Exception
	{
		public NotExecutableException(string fileName)
			: base(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' is not an executable", fileName)) { }
	}
}
