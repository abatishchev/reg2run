// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;
using System.IO;

using Reg2Run.Errors;
using Reg2Run.Settings;

namespace Reg2Run
{
	class ImportObject
	{
		string fileName, filePath, fullName, newName, workingDir, runArg;
		bool runFlag;

		#region Constructor
		public ImportObject(string fileName)
		{
			FileInfo info = new FileInfo(fileName);
			this.fileName = info.Name;
			this.filePath = info.Directory.FullName;
			this.fullName = info.FullName;
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

		public string FullName
		{
			get
			{
				return this.fullName;
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

		public bool Run
		{
			get
			{
				return this.runFlag;
			}
			set
			{
				this.runFlag = value;
			}
		}

		public string RunArg
		{
			get
			{
				return this.runArg;
			}
			set
			{
				this.runArg = value;
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

		#region Methods
		public static ImportObject Parse(ParameterContainer container)
		{
			string path = container[ParameterRole.FilePath] as string;
			if (!String.IsNullOrEmpty(path))
			{
				try
				{
					FileInfo info = new FileInfo(path);
					if (info.Exists)
					{
						if (!String.Equals(info.Extension, ".exe"))
						{
							throw new NotExecutableException(path);
						}
					}
					else
					{
						throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' doesn't exists", path));
					}
				}
				catch (ArgumentException)
				{
					throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' doesn't exists", path));
				}
			}
			else
			{
				object tempSelf = container[ParameterRole.Self];
				if (tempSelf != null)
				{
					if ((bool)tempSelf)
					{
						path = Core.Assembly.Location;
					}
				}
				else
				{
					throw new ParameterMissedException("File path");
				}
			}

			ImportObject obj = new ImportObject(path);

			string name = container[ParameterRole.FileName] as string;
			if (name != null)
			{
				if (String.IsNullOrEmpty(new FileInfo(name).Extension))
				{
					name = String.Concat(name, ".exe");
				}
				else if (!String.Equals(new FileInfo(name).Extension, ".exe"))
				{
					throw new NotExecutableException(name);
				}
				obj.NewName = name;
			}

			string dir = container[ParameterRole.FileWorkingDir] as string;
			if (dir != null)
			{
				try
				{
					DirectoryInfo info = new DirectoryInfo(dir);
					if (info.Exists)
					{
						obj.WorkingDirectory = info.FullName;
					}
					else
					{
						throw new DirectoryNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified directory '{0}' doesn't exists", dir));

					}
				}
				catch (ArgumentException)
				{
					throw new DirectoryNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified directory '{0}' doesn't exists", dir));
				}
			}

			object tempRun = container[ParameterRole.Run];
			string runArg = tempRun as string;
			if (runArg != null)
			{
				obj.Run = true;
				obj.RunArg = runArg;
			}
			else if (tempRun is bool)
			{
				obj.Run = (bool)tempRun;
			}
			return obj;
		}
		#endregion
	}
}
