using System;
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
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool AllocConsole();

		[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
		static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr CreateFile(string fileName, int desiredAccess, int shareMode, IntPtr securityAttributes, int creationDisposition, int flagsAndAttributes, IntPtr templateFile);

		[DllImport("kernel32.dll")]
		static extern bool FreeConsole();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetConsoleMode(IntPtr handle, out uint mode);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		static extern int GetLastError();

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetConsoleMode(IntPtr handle, uint mode);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool SetStdHandle(int nStdHandle, IntPtr handle);

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr handle, int nCmdShow);
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

		public static void WriteLine()
		{
			Console.WriteLine();
		}

		public static void WriteLine(string text)
		{
			WriteLine(text, MessageBoxIcon.None);
		}

		public static void WriteLine(string text, MessageBoxIcon icon)
		{
			switch (Core.CurrentContext)
			{
				case Core.Context.Console:
					{
						Console.WriteLine(text);
						break;
					}
				case Core.Context.Windows:
					{
						MessageBox.Show(text, Core.ApplicationName, MessageBoxButtons.OK, icon);
						break;
					}
			}
		}
	}
}
