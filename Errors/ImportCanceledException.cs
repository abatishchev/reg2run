// Copyright (C) 2005-2015 Alexander Batishchev (abatishchev at gmail.com)

using System;

namespace Reg2Run
{
	[Serializable]
	class ImportCanceledException : Exception
	{
		public ImportCanceledException()
			: base("Importing was canceled by user") { }
	}
}
