// Copyright (C) 2005-2010 Alexander M. Batishchev (abatishchev at gmail.com)

using System;

namespace Reg2Run
{
	class ApplicationSettings
	{
		#region Constructors
		public ApplicationSettings()
		{
			// default settings
			this.ActionTypeMode = ActionTypeFlag.Add;
			this.RegistryWriteMode = RegistryWriteFlag.HKCU | RegistryWriteFlag.HKLM;
		}
		#endregion

		#region Properties
		public ActionTypeFlag ActionTypeMode { get; set; }

		public bool EngageFlag { get; set; }

		public string FileName { get; set; }

		public string FilePath { get; set; }

		public bool Force { get; set; }

		public RegistryWriteFlag RegistryWriteMode { get; set; }

		public bool RunFlag { get; set; }

		public string RunString { get; set; }

		public bool SelfFlag { get; set; }

		public bool UsageFlag { get; set; }

		public string WorkingDirectory { get; set; }
		#endregion
	}
}
