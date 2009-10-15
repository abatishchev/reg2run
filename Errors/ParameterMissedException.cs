﻿// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;

namespace Reg2Run
{
	[Serializable]
	public class ParameterMissedException : ArgumentNullException
	{
		public ParameterMissedException(string name)
			: base(name, String.Format(CultureInfo.CurrentCulture, "Value for required parameter '{0}' was not specified", name)) { }
	}
}