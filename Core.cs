// Copyright (C) 2005-2010 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

using Microsoft.Win32;

namespace Reg2Run
{
	static class Core
	{
		#region Properties
		private static string copyright;
		public static string ApplicationCopyright
		{
			get
			{
				if (copyright == null)
				{
					try
					{
						copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCopyrightAttribute))).Copyright;
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

		private static string title;
		public static string ApplicationTitle
		{
			get
			{
				if (title == null)
				{
					try
					{
						title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyTitleAttribute))).Title;
					}
					catch
					{
						title = String.Empty;
					}
				}
				return title;
			}
		}

		public static bool IsConsole
		{
			set
			{
				if (value)
				{
					var isInDic = new System.Collections.Generic.HashSet<string>
					{
						"far",
						"cmd"
					}.Contains(GetParentProcess().ProcessName.ToLower());

					switch (Environment.OSVersion.Version.Major)
					{
						case 5: // windows xp, server 2003
							{
								KeepConsole = !isInDic;
								break;
							}
						case 6: // windows vista, windows server 2008, windows 7
							{
								// TODO: fix
								// KeepConsole = isInDic ? !IsElevated : true;
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

		public static bool IsElevated
		{
			get
			{
				return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
			}
		}

		public static bool KeepConsole { get; private set; }

		public static ApplicationSettings Settings { get; set; }
		#endregion

		#region Methods
		public static Process GetParentProcess()
		{
			return Process.GetProcessById((int)new PerformanceCounter("Process", "Creating Process ID", Process.GetCurrentProcess().ProcessName).NextValue());
		}

		public static void Import(ImportObject obj)
		{
			new System.Collections.Generic.Dictionary<RegistryWriteFlag, RegistryKey>
			{
				{ RegistryWriteFlag.HKLM, Registry.LocalMachine },
				{ RegistryWriteFlag.HKCU, Registry.CurrentUser }
			}
			.ForEach(pair => SetValue(pair, obj));
		}

		public static void Remove(ImportObject obj)
		{
			// TODO: implement
			throw new NotImplementedException();
		}

		private static void SetValue(System.Collections.Generic.KeyValuePair<RegistryWriteFlag, RegistryKey> pair, ImportObject obj)
		{
			try
			{
				if ((Settings.RegistryWriteMode & pair.Key) == pair.Key)
				{
					var key = pair.Value
						.OpenSubKey("Software")
						.OpenSubKey("Microsoft")
						.OpenSubKey("Windows")
						.OpenSubKey("CurrentVersion")
						.OpenSubKey("App Paths", true)
						.CreateSubKey(obj.FileName);
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
