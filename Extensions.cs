// Copyright (C) 2005-2011 Alexander M. Batishchev (abatishchev at gmail.com)

using System;
using System.Collections.Generic;
using System.Linq;

namespace Reg2Run
{
	static class Extensions
	{
		public static IEnumerable<string> Escape(this IEnumerable<string> arr)
		{
			return arr.Select(s => s.Contains(" ") ? String.Format("\"{0}\"", s) : s);
		}

		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection)
			{
				action(item);
			}
		}
	}
}
