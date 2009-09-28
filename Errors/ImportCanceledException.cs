// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;

namespace Reg2Run
{
	[Serializable]
	public class ImportCanceledException : Exception
	{
		public ImportCanceledException()
			: base("Importing was canceled by user") { }
	}
}
