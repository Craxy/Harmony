using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Harmony
{
	public class Patches
	{
		public readonly ReadOnlyCollection<Patch> Postfixes;

		public Patches(Patch[] postfixes)
		{
			if (postfixes == null) postfixes = new Patch[0];

			Postfixes = postfixes.ToList().AsReadOnly();
		}
	}

	public class HarmonyInstance
	{
		public static bool DEBUG = false;

		HarmonyInstance()
		{
		}

		public static HarmonyInstance Create()
		{
			return new HarmonyInstance();
		}

		public PatchProcessor Patch(MethodBase original, HarmonyMethod postfix)
		{
			var processor = new PatchProcessor(this, original, postfix);
			processor.Patch();
			return processor;
		}

		public void Restore(MethodBase original, HarmonyMethod postfix)
		{
			var processor = new PatchProcessor(this, original, postfix);
			processor.Restore();
		}

		public Patches IsPatched(MethodBase method)
		{
			return PatchProcessor.IsPatched(method);
		}

		public IEnumerable<MethodBase> GetPatchedMethods()
		{
			return HarmonySharedState.GetPatchedMethods();
		}
	}
}
