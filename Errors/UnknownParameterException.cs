﻿// Copyright (C) 2005-2011 Alexander M. Batishchev (abatishchev at gmail.com)

using System;
using System.Globalization;

namespace Reg2Run
{
	[Serializable]
	class UnknownParameterException : ArgumentException
	{
		public UnknownParameterException(string name)
			: base(String.Format(CultureInfo.CurrentCulture, "Unknown parameter '{0}' was specified", name), name) { }
	}
}
