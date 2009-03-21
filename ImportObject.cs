// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Globalization;
using System.IO;

using Reg2Run.Errors;
using Reg2Run.Settings;

namespace Reg2Run
{
	internal class ImportObject
	{
		private string workingDir;

		#region Constructors
		public ImportObject(string fileName)
		{
			this.FileName = Path.GetFileName(fileName);
			this.FullPath = Path.GetFullPath(fileName);
		}
		#endregion

		#region Properties
		public string FileName { get; private set; }

		public string FullPath { get; private set; }

		public bool Run { get; private set; }

		public string RunArg { get; private set; }

		public string WorkingDirectory
		{
			get
			{
				if (String.IsNullOrEmpty(this.workingDir))
				{
					this.workingDir = Path.GetDirectoryName(this.FullPath);
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
			var path = settings.FilePath;
			if (!String.IsNullOrEmpty(path))
			{
				var info = new FileInfo(path);
				if (info.Exists)
				{
					if (!String.Equals(info.Extension, ".exe", StringComparison.OrdinalIgnoreCase))
					{
						throw new NotExecutableException(path);
					}
				}
				else
				{
					var pathGuess = String.Concat(Path.Combine(info.Directory.FullName, info.Name), ".exe");
					if (File.Exists(pathGuess))
					{
						path = pathGuess;
					}
					else
					{
						throw new FileNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified file '{0}' doesn't exists", path));
					}
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

			var obj = new ImportObject(path);

			var name = settings.FileName;
			if (!String.IsNullOrEmpty(name))
			{
				var ext = Path.GetExtension(name);
				if (String.IsNullOrEmpty(ext))
				{
					name = String.Concat(name, ".exe");
				}
				else if (!String.Equals(ext, ".exe", StringComparison.OrdinalIgnoreCase))
				{
					throw new NotExecutableException(name);
				}
				obj.FileName = name;
			}

			var dir = settings.FileWorkingDirectory;
			if (!String.IsNullOrEmpty(dir))
			{
				if (Directory.Exists(dir))
				{
					obj.WorkingDirectory = dir;
				}
				else
				{
					throw new DirectoryNotFoundException(String.Format(CultureInfo.CurrentCulture, "Specified directory '{0}' doesn't exists", dir));
				}
			}

			if (settings.RunFlag)
			{
				obj.Run = true;
				obj.RunArg = settings.RunString;
			}
			return obj;
		}
		#endregion
	}
}
