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
		string fileName, fullPath, workingDir, runArg;
		bool runFlag;

		#region Constructor
		public ImportObject(string fileName)
		{
			FileInfo info = new FileInfo(fileName);
			this.fileName = info.Name;
			this.fullPath = info.FullName;
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
					this.workingDir = new FileInfo(this.fullPath).Directory.FullName;
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
		public static ImportObject Parse(ApplicationSettings settings)
		{
			string path = settings.FilePath;
			if (!String.IsNullOrEmpty(path))
			{
				FileInfo info = new FileInfo(path);
				try
				{
					if (info.Exists)
					{
						if (!String.Equals(info.Extension, ".exe"))
						{
							throw new NotExecutableException(path);
						}
					}
					else
					{
						string pathGuess = String.Concat(Path.Combine(info.Directory.FullName, info.Name), ".exe");
						if (new FileInfo(pathGuess).Exists)
						{
							path = pathGuess;
						}
						else
						{
							throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' doesn't exists", path));
						}
					}
				}
				catch (ArgumentException)
				{
					throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' doesn't exists", path));
				}
			}
			else
			{
				if (settings.SelfFlag)
				{
					path = Core.Assembly.Location;
				}
				else
				{
					throw new ParameterMissedException("File path");
				}
			}

			ImportObject obj = new ImportObject(path);

			{
				string name = settings.FileName;
				if (!String.IsNullOrEmpty(name))
				{
					if (String.IsNullOrEmpty(new FileInfo(name).Extension))
					{
						name = String.Concat(name, ".exe");
					}
					else if (!String.Equals(new FileInfo(name).Extension, ".exe"))
					{
						throw new NotExecutableException(name);
					}
					obj.fileName = name;
				}
			}

			{
				string dir = settings.FileWorkingDirectory;
				if (!String.IsNullOrEmpty(dir))
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
			}

			{
				if (settings.RunFlag)
				{
					obj.Run = true;
					obj.RunArg = settings.RunString;
				}
			}
			return obj;
		}
		#endregion
	}
}
