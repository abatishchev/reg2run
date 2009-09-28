// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;

namespace Reg2Run
{
	[Serializable]
	public class ParameterNotSetException : ArgumentNullException
	{
		public ParameterNotSetException(string name)
			: base(name, String.Format(CultureInfo.CurrentCulture, "Parameter '{0}' was provided without assigning it's value", name)) { }
	}
}
