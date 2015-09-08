// Copyright (C) 2005-2015 Alexander Batishchev (abatishchev at gmail.com)

using System;
using System.Globalization;

namespace Reg2Run
{
	[Serializable]
	class ParameterMissedException : ArgumentNullException
	{
		public ParameterMissedException(string name)
			: base(name, String.Format(CultureInfo.CurrentCulture, "Value for required parameter '{0}' was not specified", name)) { }
	}
}
