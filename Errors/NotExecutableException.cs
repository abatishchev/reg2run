﻿// Copyright (C) 2005-2010 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;

namespace Reg2Run
{
	[Serializable]
	public class NotExecutableException : Exception
	{
		public NotExecutableException(string fileName)
			: base(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' is not an executable", fileName)) { }
	}
}
