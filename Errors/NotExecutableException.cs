// Copyright (C) 2005-2015 Alexander Batishchev (abatishchev at gmail.com)

using System;
using System.Globalization;

namespace Reg2Run
{
	[Serializable]
	class NotExecutableException : Exception
	{
		public NotExecutableException(string fileName)
			: base(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' is not an executable", fileName)) { }
	}
}
