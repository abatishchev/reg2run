// Copyright (C) 2005-2008 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.Win32;

using Reg2Run.Errors;
using Reg2Run.Settings;

namespace Reg2Run
{
	abstract class Core
	{
		static ApplicationSettings settings;

		static string copyright, title;

		#region Properties
		public static string ApplicationCopyright
		{
			get
			{
				if (String.IsNullOrEmpty(copyright))
				{
					object[] customAttributes = Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
					copyright = ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
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
				if (String.IsNullOrEmpty(title))
				{

					object[] customAttributes = Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
					title = ((AssemblyTitleAttribute)customAttributes[0]).Title;
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
					//ManualConsole.Create();					
				}
				else
				{
					ManualConsole.Hide();
				}
			}
		}

		public static ApplicationSettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = value;
			}
		}
		#endregion

		#region Methods
		public static void Import(ImportObject obj)
		{
			RegistryKey registry = Registry.LocalMachine
				.CreateSubKey("Software")
				.CreateSubKey("Microsoft")
				.CreateSubKey("Windows")
				.CreateSubKey("CurrentVersion")
				.CreateSubKey("App Paths");
			RegistryKey key = registry.CreateSubKey(obj.FileName);
			key.SetValue("", obj.FullPath);
			key.SetValue("Path", obj.WorkingDirectory);
			key.Flush();
		}
		#endregion
	}
}
