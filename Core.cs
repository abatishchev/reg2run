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
	internal enum RegistryWriteFlag
	{
		HKCU = 1,
		HKLM = 2
	}

	internal abstract class Core
	{
		private static string copyright, title;

		#region Properties
		internal static string ApplicationCopyright
		{
			get
			{
				if (copyright == null)
				{
					try
					{
						object[] customAttributes = Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
						copyright = ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
					}
					catch
					{
						copyright = String.Empty;
					}
				}
				return copyright;
			}
		}

		internal static string ApplicationName
		{
			get
			{
				return Application.ProductName;
			}
		}

		internal static string ApplicationVersion
		{
			get
			{
				return Application.ProductVersion;
			}
		}

		internal static string ApplicationTitle
		{
			get
			{
				if (title == null)
				{

					try
					{
						object[] customAttributes = Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
						title = ((AssemblyTitleAttribute)customAttributes[0]).Title;
					}
					catch
					{
						copyright = String.Empty;
					}
				}
				return title;
			}
		}

		internal static Assembly Assembly
		{
			get
			{
				return Assembly.GetEntryAssembly();
			}
		}

		internal static bool IsConsole
		{
			set
			{
				if (value)
				{
					string parentProcessName = ParentProcess.ProcessName;
					KeepConsole = String.Equals(parentProcessName, "explorer", StringComparison.OrdinalIgnoreCase) || String.Equals(parentProcessName, "rundll32", StringComparison.OrdinalIgnoreCase);
				}
				else
				{
					ManualConsole.Hide();
				}
			}
		}

		internal static bool KeepConsole { get; private set; }

		internal static Process ParentProcess
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
