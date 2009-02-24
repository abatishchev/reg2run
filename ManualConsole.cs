﻿// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

using System;
using System.Reflection;
using System.Runtime.InteropServices;

using Reg2Run.Errors;

namespace Reg2Run
{
	static class ManualConsole
	{
		#region DllImport
		/*
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AllocConsole();
		*/

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr handle);

		/*
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		private static extern IntPtr CreateFile(string fileName, int desiredAccess, int shareMode, IntPtr securityAttributes, int creationDisposition, int flagsAndAttributes, IntPtr templateFile);
		*/

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeConsole();

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		/*
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetStdHandle(int nStdHandle, IntPtr handle);
		*/
		#endregion

		#region Managed Methods
		/*
		internal static void Create()
		{
			var ptr = GetStdHandle(-11);
			AllocConsoleManaged();
			ptrNew = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
			SetStdHandleManaged(ptr);
			var newOut = new StreamWriter(Console.OpenStandardOutput());
			newOut.AutoFlush = true;
			Console.SetOut(newOut);
			Console.SetError(newOut);
		}
		*/

		internal static void Hide()
		{
			var ptr = GetStdHandle(-11);
			CloseHandleManaged(ptr);
			ptr = IntPtr.Zero;
			FreeConsoleManaged();
		}
		#endregion

		#region Unmanaged Methods
		/*
		private static void AllocConsoleManaged()
		{
			if (!AllocConsole())
			{
				throw new PInvokeException(MethodBase.GetCurrentMethod().Name);
			}
		}
		*/

		private static void CloseHandleManaged(IntPtr ptr)
		{
			if (!CloseHandle(ptr))
			{
				throw new PInvokeException(MethodBase.GetCurrentMethod().Name);
			}
		}

		private static void FreeConsoleManaged()
		{
			if (!FreeConsole())
			{
				throw new PInvokeException(MethodBase.GetCurrentMethod().Name);
			}
		}

		/*
		private static void SetStdHandleManaged(IntPtr ptr)
		{
			if (!SetStdHandle(-11, ptr))
			{
				throw new PInvokeException(MethodBase.GetCurrentMethod().Name);
			}
		}
		*/
		#endregion
	}
}
