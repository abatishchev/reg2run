using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Reg2Run.Errors;

namespace Reg2Run
{
	public static class ManualConsole
	{
		static IntPtr ptrNew;

		#region DllImport
		[CLSCompliant(false)]
		[StructLayout(LayoutKind.Sequential)]
		public struct PROCESSENTRY32
		{
			public uint dwSize;
			public uint cntUsage;
			public uint th32ProcessID;
			public IntPtr th32DefaultHeapID;
			public uint th32ModuleID;
			public uint cntThreads;
			public uint th32ParentProcessID;
			public int pcPriClassBase;
			public uint dwFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szExeFile;
		};

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr CreateFile(string fileName, int desiredAccess, int shareMode, IntPtr securityAttributes, int creationDisposition, int flagsAndAttributes, IntPtr templateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

		[DllImport("kernel32.dll")]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleMode(IntPtr handle, out uint mode);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		static extern int GetLastError();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll")]
		static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		[DllImport("kernel32.dll")]
		static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleMode(IntPtr handle, uint mode);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetStdHandle(int nStdHandle, IntPtr handle);

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr handle, int nCmdShow);
		#endregion

		public static void Create()
		{
			MessageBox.Show(GetParentProc().StartInfo.RedirectStandardOutput.ToString());
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
			int pidParent = 0;
			int pidCurrent = Process.GetCurrentProcess().Id;

			IntPtr hSnapshot = CreateToolhelp32Snapshot(2, 0);
			if (hSnapshot == IntPtr.Zero)
			{
				return null;
			}

			PROCESSENTRY32 info = new PROCESSENTRY32();
			info.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

			if (!Process32First(hSnapshot, ref info))
			{
				return null;
			}
			do
			{
				if (pidCurrent == info.th32ProcessID)
				{
					pidParent = (int)info.th32ParentProcessID;
				}
			}
			while (pidParent == 0 && Process32Next(hSnapshot, ref info));

			if (pidParent > 0)
			{
				return Process.GetProcessById(pidParent);
			}
			else
			{
				return null;
			}
		}
	}
}
