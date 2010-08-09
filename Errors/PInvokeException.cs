// Copyright (C) 2005-2010 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;

namespace Reg2Run
{
	[Serializable]
	class PInvokeException : Exception
	{
		public PInvokeException(string methodName)
			: base(String.Format(CultureInfo.CurrentCulture, "An error occured while external method '{0}' call", methodName)) { }
	}
}
