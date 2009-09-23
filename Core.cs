// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Win32;

using Reg2Run.Settings;

namespace Reg2Run
{
	[Flags]
	enum RegistryWriteFlag
	{
		HKCU = 1,
		HKLM = 2
	}

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
				return Application.ProductName;
			}
		}

		public static string ApplicationVersion
		{
			get
			{
				return Application.ProductVersion;
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
						case 5: // windows xp, windows server 2003
							{
								KeepConsole = setProcess.Contains(ParentProcess.ProcessName.ToLower());
								break;
							}
						case 6: // windiows vista, windows server 2008, windows 7
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
				PerformanceCounter pc = new PerformanceCounter("Process", "Creating Process ID", Process.GetCurrentProcess().ProcessName);
				return Process.GetProcessById((int)pc.NextValue());
			}
		}

		internal static ApplicationSettings Settings { get; set; }
		#endregion

		#region Methods
		public static void Import(ImportObject obj)
		{
			if ((Settings.RegistryWriteMode & RegistryWriteFlag.HKCU) == RegistryWriteFlag.HKCU)
			{
				SetValue(Registry.CurrentUser, obj);
			}
			if ((Settings.RegistryWriteMode & RegistryWriteFlag.HKLM) == RegistryWriteFlag.HKLM)
			{
				SetValue(Registry.LocalMachine, obj);
			}
		}

		private static void SetValue(RegistryKey hive, ImportObject obj)
		{
			var appPaths = hive.OpenSubKey("Software")
				.OpenSubKey("Microsoft")
				.OpenSubKey("Windows")
				.OpenSubKey("CurrentVersion")
				.OpenSubKey("App Paths", true);

			var key = appPaths.CreateSubKey(obj.FileName);
			key.SetValue("", obj.FullPath);
			key.SetValue("Path", obj.WorkingDirectory);
			key.Flush();
		}
		#endregion
	}
}
