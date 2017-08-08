using Harmony.ILCopying;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Harmony
{
	public static class PatchFunctions
	{
		internal struct PatchHandle
		{
			public DynamicMethod PatchedMethod;
			public byte[] OverwrittenCode;
		}

		public static void UpdateWrapper(MethodBase original, MethodInfo postfix)
		{
//			if (postfix == null)
//			{
//				// revert postfix
//				return;
//			}
			
			var originalCodeStart = Memory.GetMethodStart(original);

			// If we're overwriting an old patch, restore the original 12 (or 6) bytes of the method beforehand
			object oldHandle;
			if (PatchTools.RecallObject(original, out oldHandle))
			{
				var oldPatchHandle = (PatchHandle)oldHandle;
				Memory.WriteBytes(originalCodeStart, oldPatchHandle.OverwrittenCode);
			}

			if (postfix == null)
			{
				// No patches, can just leave the original method intact
				PatchTools.ForgetObject(originalCodeStart);
				return;
			}

			var replacement = MethodPatcher.CreatePatchedMethod(original, postfix);
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
