// Copyright (C) 2005-2010 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.IO;

namespace Reg2Run
{
	class ImportObject
	{
		#region Constructors
		public ImportObject(string fileName)
		{
			this.FileName = Path.GetFileName(fileName);
			this.FullPath = Path.GetFullPath(fileName);
			this.WorkingDirectory = Path.GetDirectoryName(fileName);
		}
		#endregion

		#region Properties
		public string FileName { get; set; }

		public string FullPath { get; private set; }

		public bool Run { get; set; }

		public string RunArg { get; set; }

		public string WorkingDirectory { get; set; }
		#endregion
	}
}
