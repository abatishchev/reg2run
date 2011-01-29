// Copyright (C) 2005-2011 Alexander M. Batishchev (abatishchev at gmail.com)

using System;
using System.Globalization;

namespace Reg2Run
{
	[Serializable]
	class ParameterNotSetException : ArgumentNullException
	{
		public ParameterNotSetException(string name)
			: base(name, String.Format(CultureInfo.CurrentCulture, "Parameter '{0}' was provided without assigning it's value", name)) { }
	}
}
