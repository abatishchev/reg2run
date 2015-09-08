// Copyright (C) 2005-2015 Alexander Batishchev (abatishchev at gmail.com)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

using Microsoft.Win32;

namespace Reg2Run
{
	static class Core
	{
		#region Fields
		private static IEnumerable<Tuple<RegistryWriteFlag, RegistryKey>> registryKeyDictionary = new[]
		{
			new Tuple<RegistryWriteFlag, RegistryKey>( RegistryWriteFlag.HKLM, Registry.LocalMachine ),
			new Tuple<RegistryWriteFlag, RegistryKey>(RegistryWriteFlag.HKCU, Registry.CurrentUser )
		};
		#endregion

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

		public static bool IsElevated
		{
			get
			{
				return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
			}
		}

		public static ApplicationSettings Settings { get; set; }
		#endregion

		#region Methods
		public static void Add(ImportObject obj)
		{
			foreach (var pair in registryKeyDictionary)
			{
				try
				{
					SetValue(pair, obj);
				}
				catch (System.Security.SecurityException)
				{
				}
			}
		}

		private static void DeleteValue(Tuple<RegistryWriteFlag, RegistryKey> tuple, ImportObject obj)
		{
			if ((Settings.RegistryWriteMode & tuple.Item1) == tuple.Item1)
			{
				tuple.Item2
					.OpenSubKey("Software")
					.OpenSubKey("Microsoft")
					.OpenSubKey("Windows")
					.OpenSubKey("CurrentVersion")
					.OpenSubKey("App Paths", true)
					.DeleteSubKeyTree(obj.FileName, false);
			}
		}

		public static void Remove(ImportObject obj)
		{
			int count = 0;
			foreach (var tuple in registryKeyDictionary)
			{
				try
				{
					DeleteValue(tuple, obj);
				}
				catch (System.Security.SecurityException)
				{
					count++;
				}
			}
		}

		private static void SetValue(Tuple<RegistryWriteFlag, RegistryKey> tuple, ImportObject obj)
		{
			if ((Settings.RegistryWriteMode & tuple.Item1) == tuple.Item1)
			{
				var currentVersion = tuple.Item2
					.OpenSubKey("Software")
					.OpenSubKey("Microsoft")
					.OpenSubKey("Windows")
					.OpenSubKey("CurrentVersion", true);

				var appPaths = currentVersion.OpenSubKey("App Paths", true);
				if (appPaths == null)
				{
					currentVersion.CreateSubKey("App Paths");
					appPaths = currentVersion.OpenSubKey("App Paths", true);
				}

				var key = appPaths.CreateSubKey(obj.FileName);
				key.SetValue(String.Empty, obj.FullPath);
				key.SetValue("Path", obj.WorkingDirectory);
				key.Flush();
			}
		}
		#endregion
	}
}
