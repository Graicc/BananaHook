using System;
using System.Collections.Generic;
using System.Text;

namespace BananaHook
{
	internal static class EventUtils
	{
		internal static bool SafeInvoke(this EventHandler eventHandler, params object[] args)
		{
			if (eventHandler != null)
			{
				foreach (var del in eventHandler.GetInvocationList())
				{
					try
					{
						del.DynamicInvoke(args);
					}
					catch (Exception e) {
						BananaHook.Log("Exception in event invoke: " + e.Message + "\n" + e.StackTrace);
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		internal static bool SafeInvoke<T>(this EventHandler<T> eventHandler, params object[] args)
		{
			if (eventHandler != null)
			{
				foreach (var del in eventHandler.GetInvocationList())
				{
					try
					{
						del.DynamicInvoke(args);
					}
					catch (Exception e) {
						BananaHook.Log("Exception in event invoke: " + e.Message + "\n" + e.StackTrace);
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
