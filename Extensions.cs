using System;

namespace Reg2Run
{
	public static class Extensions
	{
		public static void ForEach<T>(this System.Collections.Generic.IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection)
			{
				action(item);
			}
		}
	}
}
