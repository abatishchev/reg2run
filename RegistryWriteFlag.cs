// Copyright (C) 2005-2011 Alexander M. Batishchev (abatishchev at gmail.com)

using System;

namespace Reg2Run
{
	[Flags]
	enum RegistryWriteFlag
	{
		HKCU = 1,
		HKLM = 2
	}
}
