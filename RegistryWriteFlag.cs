// Copyright (C) 2005-2015 Alexander Batishchev (abatishchev at gmail.com)

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
