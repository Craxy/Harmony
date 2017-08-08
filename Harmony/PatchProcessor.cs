using System;
using System.Collections.Generic;
using System.Reflection;

namespace Harmony
{
	public class PatchProcessor
	{
		static object locker = new object();

		readonly Type container;
		readonly HarmonyMethod containerAttributes;

		MethodBase original;
		HarmonyMethod postfix;

		PatchInfo patchInfo;

		public PatchProcessor(Type type, HarmonyMethod attributes)
		{
			container = type;
			containerAttributes = attributes ?? new HarmonyMethod(null);
			postfix = containerAttributes.Clone();
			ProcessType();
		}

		public PatchProcessor(MethodBase original, HarmonyMethod postfix)
		{
			this.original = original;
			this.postfix = postfix ?? new HarmonyMethod(null);
		}

		public void Patch()
		{
			lock (locker)
			{
				if (patchInfo == null) patchInfo = new PatchInfo();

				PatchFunctions.AddPostfix(patchInfo, postfix);
				PatchFunctions.UpdateWrapper(original, patchInfo);
			}
		}

		public void Restore()
		{
			lock (locker)
			{
				if (patchInfo == null) return;

				PatchFunctions.RemovePostfix(patchInfo, postfix);
				PatchFunctions.UpdateWrapper(original, patchInfo);
			}
		}

		bool CallPrepare()
		{
//			if (original != null)
//				return RunMethod<HarmonyPrepare, bool>(true, original);
//			return RunMethod<HarmonyPrepare, bool>(true);
			return false;
		}

		void ProcessType()
		{
			original = GetOriginalMethod();

			var patchable = CallPrepare();
			if (patchable)
			{
				if (original == null)
					throw new ArgumentException("No target method specified for class " + container.FullName);


				if (postfix.method != null)
				{
					if (postfix.method.IsStatic == false)
						throw new ArgumentException("Patch method " + postfix.method.Name + " in " + postfix.method.DeclaringType + " must be static");

					containerAttributes.CopyTo(postfix);
				}

			}
		}

		MethodBase GetOriginalMethod()
		{
			var attr = containerAttributes;
			if (attr.originalType == null) return null;
			if (attr.methodName == null)
				return AccessTools.Constructor(attr.originalType, attr.parameter);
			return AccessTools.Method(attr.originalType, attr.methodName, attr.parameter);
		}
	}
}
