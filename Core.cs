// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Microsoft.Win32;

namespace Reg2Run
{
	public static class Core
	{
		private static string copyright, title;

		private static System.Collections.Generic.HashSet<string> setProcess = new System.Collections.Generic.HashSet<string>
		{
			"explorer",
			"rundll32"
		};

		#region Properties
		public static string ApplicationCopyright
		{
			get
			{
				if (copyright == null)
				{
					try
					{
						var customAttributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
						if (customAttributes != null && customAttributes.Length > 0)
						{
							copyright = ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
						}
					}
					catch
					{
						copyright = String.Empty;
					}
				}
				return copyright;
			}
		}

		public static string ApplicationName
		{
			get
			{
				return System.Windows.Forms.Application.ProductName;
			}
		}

		public static string ApplicationVersion
		{
			get
			{
				return System.Windows.Forms.Application.ProductVersion;
			}
		}

		public static string ApplicationTitle
		{
			get
			{
				if (title == null)
				{
					try
					{
						var customAttributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
						if (customAttributes != null && customAttributes.Length > 0)
						{
							title = ((AssemblyTitleAttribute)customAttributes[0]).Title;
						}
					}
					catch
					{
						title = String.Empty;
					}
				}
				return title;
			}
		}

		public static Assembly Assembly
		{
			get
			{
				return Assembly.GetEntryAssembly();
			}
		}

		public static bool IsConsole
		{
			set
			{
				if (value)
				{
					switch (Environment.OSVersion.Version.Major)
					{
						case 5: // windows xp, server 2003
							{
								KeepConsole = setProcess.Contains(ParentProcess.ProcessName.ToLower());
								break;
							}
						case 6: // windows vista, windows server 2008, 7
							{
								KeepConsole = true;
								break;
							}
					}
				}
				else
				{
					ManualConsole.Hide();
				}
			}
		}

		public static bool KeepConsole { get; private set; }

		public static Process ParentProcess
		{
			get
			{
				return Process.GetProcessById((int)new PerformanceCounter("Process", "Creating Process ID", Process.GetCurrentProcess().ProcessName).NextValue());
			}
		}

		internal static ApplicationSettings Settings { get; set; }
		#endregion

		#region Methods
		public static void Import(ImportObject obj)
		{
			new RegistryWriteFlag[] { RegistryWriteFlag.HKCU, RegistryWriteFlag.HKLM }.ForEach(f => SetValue(Registry.LocalMachine, f, obj));
		}

		private static void SetValue(RegistryKey hive, RegistryWriteFlag flag, ImportObject obj)
		{
			try
			{
				if ((Settings.RegistryWriteMode & flag) == flag)
				{
					var appPaths = hive.OpenSubKey("Software")
						.OpenSubKey("Microsoft")
						.OpenSubKey("Windows")
						.OpenSubKey("CurrentVersion")
						.OpenSubKey("App Paths", true);

					var key = appPaths.CreateSubKey(obj.FileName);
					key.SetValue(String.Empty, obj.FullPath);
					key.SetValue("Path", obj.WorkingDirectory);
					key.Flush();
				}
			}
			catch
			{
				// do nothing; just skip
			}
		}
		#endregion
	}
}
