﻿using System;
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

		public ReadOnlyCollection<string> Owners
		{
			get
			{
				var result = new HashSet<string>();
				result.UnionWith(Postfixes.Select(p => p.owner));
				return result.ToList().AsReadOnly();
			}
		}

		public Patches(Patch[] postfixes)
		{
			if (postfixes == null) postfixes = new Patch[0];

			Postfixes = postfixes.ToList().AsReadOnly();
		}
	}

	public class HarmonyInstance
	{
		readonly string id;
		public string Id => id;
		public static bool DEBUG = false;

		HarmonyInstance(string id)
		{
			this.id = id;
		}

		public static HarmonyInstance Create(string id)
		{
			if (id == null) throw new Exception("id cannot be null");
			return new HarmonyInstance(id);
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

		//

		public Patches IsPatched(MethodBase method)
		{
			return PatchProcessor.IsPatched(method);
		}

		public IEnumerable<MethodBase> GetPatchedMethods()
		{
			return HarmonySharedState.GetPatchedMethods();
		}

		public Dictionary<string, Version> VersionInfo(out Version currentVersion)
		{
			currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
			var assemblies = new Dictionary<string, Assembly>();
			GetPatchedMethods().Do(method =>
			{
				var info = HarmonySharedState.GetPatchInfo(method);
				info.postfixes.Do(fix => assemblies[fix.owner] = fix.patch.DeclaringType.Assembly);
			});

			var result = new Dictionary<string, Version>();
			assemblies.Do(info =>
			{
				var assemblyName = info.Value.GetReferencedAssemblies().FirstOrDefault(a => a.FullName.StartsWith("0Harmony, Version"));
				if (assemblyName != null)
					result[info.Key] = assemblyName.Version;
			});
			return result;
		}
	}
}
