// Copyright (C) 2005-2010 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

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
		private static IDictionary<RegistryWriteFlag, RegistryKey> registryKeyDictionary = new Dictionary<RegistryWriteFlag, RegistryKey>
		{
			{ RegistryWriteFlag.HKLM, Registry.LocalMachine },
			{ RegistryWriteFlag.HKCU, Registry.CurrentUser }
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
			int count = 0;
			foreach (var pair in registryKeyDictionary)
			{
				try
				{
					SetValue(pair, obj);
				}
				catch (System.Security.SecurityException)
				{
					count++;
				}
			}
		}

		private static void DeleteValue(KeyValuePair<RegistryWriteFlag, RegistryKey> pair, ImportObject obj)
		{
			if ((Settings.RegistryWriteMode & pair.Key) == pair.Key)
			{
				pair.Value
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
			foreach (var pair in registryKeyDictionary)
			{
				try
				{
					DeleteValue(pair, obj);
				}
				catch (System.Security.SecurityException)
				{
					count++;
				}
			}
		}

		private static void SetValue(KeyValuePair<RegistryWriteFlag, RegistryKey> pair, ImportObject obj)
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
		#endregion
	}
}
