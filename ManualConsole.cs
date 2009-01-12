using System;
using System.Reflection;
using System.Runtime.InteropServices;

using Reg2Run.Errors;

namespace Reg2Run
{
	public static class ManualConsole
	{
		#region DllImport
		/*
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();
		*/

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool CloseHandle(IntPtr handle);

		/*
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern IntPtr CreateFile(string fileName, int desiredAccess, int shareMode, IntPtr securityAttributes, int creationDisposition, int flagsAndAttributes, IntPtr templateFile);
		*/

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern IntPtr GetStdHandle(int nStdHandle);

		/*
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetStdHandle(int nStdHandle, IntPtr handle);
		*/
		#endregion

		#region Managed Methods
		/*
		public static void Create()
		{
			IntPtr ptr = GetStdHandle(-11);
			AllocConsoleManaged();
			ptrNew = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, 3, 0, IntPtr.Zero);
			SetStdHandleManaged(ptr);
			StreamWriter newOut = new StreamWriter(Console.OpenStandardOutput());
			newOut.AutoFlush = true;
			Console.SetOut(newOut);
			Console.SetError(newOut);
		}
		*/

		public static void Hide()
		{
			IntPtr ptr = GetStdHandle(-11);
			CloseHandleManaged(ptr);
			ptr = IntPtr.Zero;
			FreeConsoleManaged();
		}
		#endregion

		#region Unmanaged Methods
		/*
		static void AllocConsoleManaged()
		{
			if (!AllocConsole())
			{
				throw new PInvokeException(MethodBase.GetCurrentMethod().Name);
			}
		}
		*/

		static void CloseHandleManaged(IntPtr ptr)
		{
			if (!CloseHandle(ptr))
			{
				throw new PInvokeException(MethodBase.GetCurrentMethod().Name);
			}
		}

		static void FreeConsoleManaged()
		{
			if (!FreeConsole())
			{
				throw new PInvokeException(MethodBase.GetCurrentMethod().Name);
			}
		}

		/*
		static void SetStdHandleManaged(IntPtr ptr)
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
