// Copyright (C) 2005-2011 Alexander M. Batishchev (abatishchev at gmail.com)

using System;
using System.IO;

namespace Reg2Run
{
	static class ImportObjectParser
	{
		#region Methods
		public static ImportObject Parse(ApplicationSettings settings)
		{
			var path = settings.FilePath;
			if (!String.IsNullOrEmpty(path))
			{
				if (!settings.Force)
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
							throw new FileNotFoundException(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Specified file '{0}' doesn't exists", path));
						}
					}
				}
			}
			else
			{
				if (settings.SelfFlag)
				{
					path = System.Reflection.Assembly.GetEntryAssembly().Location;
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

			var dir = settings.WorkingDirectory;
			if (!String.IsNullOrEmpty(dir))
			{
				if (!Directory.Exists(dir) && !settings.Force)
				{
					throw new DirectoryNotFoundException(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Specified directory '{0}' doesn't exists", dir));
				}
				else
				{
					obj.WorkingDirectory = dir;
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
