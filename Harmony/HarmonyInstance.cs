using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Harmony
{

	public class HarmonyInstance
	{
		public static bool DEBUG = false;

		private HarmonyInstance()
		{
		}

		public static PatchProcessor Patch(MethodBase original, HarmonyMethod postfix)
		{
			var processor = new PatchProcessor(original, postfix);
			processor.Patch();
			return processor;
		}

		public static void Restore(MethodBase original, HarmonyMethod postfix)
		{
			var processor = new PatchProcessor(original, postfix);
			processor.Restore();
		}

		public static bool IsPatched(MethodBase method)
		{
			return PatchProcessor.IsPatched(method);
		}

		public static IEnumerable<MethodBase> GetPatchedMethods()
		{
			return HarmonySharedState.GetPatchedMethods();
		}
	}
}
