// Copyright (C) 2005-2009 Alexander M. Batishchev aka Godfather (abatishchev at gmail.com)

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
