// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.IO;

namespace Reg2Run
{
	class ImportObject
	{
		string fullPath, filePath, fileName, newName, workingDir;

		#region Constructor
		public ImportObject(string fileName)
		{
			FileInfo info = new FileInfo(fileName);
			this.fullPath = info.FullName;
			this.filePath = info.Directory.FullName;
			this.fileName = info.Name;
		}

		#endregion

		#region Properties
		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public string FullPath
		{
			get
			{
				return this.fullPath;
			}
		}

		public string NewName
		{
			get
			{
				if (String.IsNullOrEmpty(this.newName))
				{
					this.newName = this.fileName;
				}
				return this.newName;
			}
			set
			{
				this.newName = value;
			}
		}

		public string WorkingDirectory
		{
			get
			{
				if (String.IsNullOrEmpty(this.workingDir))
				{
					this.workingDir = this.filePath;
				}
				return workingDir;
			}
			set
			{
				this.workingDir = value;
			}
		}
		#endregion
	}
}
