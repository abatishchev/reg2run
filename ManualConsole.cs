﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using Reg2Run.Errors;

namespace Reg2Run
{
	public static class ManualConsole
	{
		static IntPtr ptrNew;

		#region DllImport
		[StructLayout(LayoutKind.Sequential)]
		struct PROCESSENTRY32
		{
			public uint dwSize;
			uint cntUsage;
			public uint th32ProcessID;
			IntPtr th32DefaultHeapID;
			uint th32ModuleID;
			uint cntThreads;
			public uint th32ParentProcessID;
			int pcPriClassBase;
			uint dwFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szExeFile;
		};

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern IntPtr CreateFile(string fileName, int desiredAccess, int shareMode, IntPtr securityAttributes, int creationDisposition, int flagsAndAttributes, IntPtr templateFile);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetStdHandle(int nStdHandle, IntPtr handle);
		#endregion

		public static void Create()
		{
			if (ptrNew == IntPtr.Zero)
			{
				ptrNew = GetStdHandle(-11);
			}
			if (!AllocConsole())
			{
				throw new ExternalCallException("AllocConsole");
			}
			ptrNew = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
			if (!SetStdHandle(-11, ptrNew))
			{
				throw new ExternalCallException("SetStdHandle");
			}
			StreamWriter newOut = new StreamWriter(Console.OpenStandardOutput());
			newOut.AutoFlush = true;
			Console.SetOut(newOut);
			Console.SetError(newOut);
		}

		public static void Hide()
		{
			if (ptrNew == IntPtr.Zero)
			{
				ptrNew = GetStdHandle(-11);
			}
			if (!CloseHandle(ptrNew))
			{
				throw new ExternalCallException("CloseHandle");
			}
			ptrNew = IntPtr.Zero;
			if (!FreeConsole())
			{
				throw new ExternalCallException("FreeConsole");
			}
		}

		static Process GetParentProc()
		{
			PerformanceCounter pc = new PerformanceCounter("Process", "Creating Process ID", Process.GetCurrentProcess().ProcessName);
			return Process.GetProcessById((int)pc.NextValue());
		}
	}
}
