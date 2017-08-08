using Harmony.ILCopying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Harmony
{
	public static class PatchFunctions
	{
		public static void AddPostfix(PatchInfo patchInfo, HarmonyMethod info)
		{
			if (info == null || info.method == null) return;
			patchInfo.AddPostfix(info.method);
		}

		public static void RemovePostfix(PatchInfo patchInfo, HarmonyMethod info)
		{
			if (info == null || info.method == null) return;

			patchInfo.RemovePostfix(info.method);
		}

		public static List<MethodInfo> GetSortedPatchMethods(MethodBase original, Patch[] patches)
		{
			return patches
				.Where(p => p.patch != null)
				.OrderBy(p => p)
				.Select(p => p.GetMethod(original))
				.ToList();
		}

		internal struct PatchHandle
		{
			public DynamicMethod PatchedMethod;
			public byte[] OverwrittenCode;
		}

		public static void UpdateWrapper(MethodBase original, PatchInfo patchInfo)
		{
			var sortedPostfixes = GetSortedPatchMethods(original, patchInfo.postfixes);

			var originalCodeStart = Memory.GetMethodStart(original);

			// If we're overwriting an old patch, restore the original 12 (or 6) bytes of the method beforehand
			object oldHandle;
			if (PatchTools.RecallObject(original, out oldHandle))
			{
				var oldPatchHandle = (PatchHandle)oldHandle;
				Memory.WriteBytes(originalCodeStart, oldPatchHandle.OverwrittenCode);
			}

			if (patchInfo.postfixes.Length == 0)
			{
				// No patches, can just leave the original method intact
				PatchTools.ForgetObject(originalCodeStart);
				return;
			}

			var replacement = MethodPatcher.CreatePatchedMethod(original, sortedPostfixes);
			if (replacement == null) throw new MissingMethodException("Cannot create dynamic replacement for " + original);
			var patchCodeStart = Memory.GetMethodStart(replacement);

			// This part effectively corrupts the original compiled method, so we should prepare to restore the overwritten part later
			// (It doesn't look like it breaks something, but... better safe than sorry?)
			var oldBytes = new byte[(IntPtr.Size == sizeof(long)) ? 12 : 6];
			Marshal.Copy((IntPtr)originalCodeStart, oldBytes, 0, oldBytes.Length);
			// Store code being overwritten by the jump for the restoration
			PatchTools.RememberObject(original, new PatchHandle { PatchedMethod = replacement, OverwrittenCode = oldBytes });
			Memory.WriteJump(originalCodeStart, patchCodeStart);
		}
	}
}
