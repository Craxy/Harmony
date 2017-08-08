using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Harmony
{

	public static class HarmonyInstance
	{
		public static bool DEBUG = false;

		public static PatchProcessor Patch(MethodBase original, HarmonyMethod postfix)
		{
			var processor = new PatchProcessor(original, postfix);
			processor.Patch();
			return processor;
		}
	}
}
