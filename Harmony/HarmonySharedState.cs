using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony
{
	public static class HarmonySharedState
	{
		private static readonly Dictionary<MethodBase, byte[]> State = new Dictionary<MethodBase, byte[]>();

		internal static PatchInfo GetPatchInfo(MethodBase method)
		{
			var bytes = State.GetValueSafe(method);
			if (bytes == null) return null;
			return PatchInfoSerialization.Deserialize(bytes);
		}

		internal static IEnumerable<MethodBase> GetPatchedMethods()
		{
			return State.Keys.AsEnumerable();
		}

		internal static void UpdatePatchInfo(MethodBase method, PatchInfo patchInfo)
		{
			State[method] = patchInfo.Serialize();
		}
	}
}
