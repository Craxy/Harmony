using System;
using System.Collections.Generic;
using System.Reflection;

namespace Harmony
{
	public class PatchProcessor
	{
		static object locker = new object();

		readonly HarmonyInstance instance;

		readonly Type container;
		readonly HarmonyMethod containerAttributes;

		MethodBase original;
		HarmonyMethod postfix;

		public PatchProcessor(HarmonyInstance instance, Type type, HarmonyMethod attributes)
		{
			this.instance = instance;
			container = type;
			containerAttributes = attributes ?? new HarmonyMethod(null);
			postfix = containerAttributes.Clone();
			ProcessType();
		}

		public PatchProcessor(HarmonyInstance instance, MethodBase original, HarmonyMethod postfix)
		{
			this.instance = instance;
			this.original = original;
			this.postfix = postfix ?? new HarmonyMethod(null);
		}

		public static Patches IsPatched(MethodBase method)
		{
			var patchInfo = HarmonySharedState.GetPatchInfo(method);
			if (patchInfo == null) return null;
			return new Patches(patchInfo.postfixes);
		}

		public static IEnumerable<MethodBase> AllPatchedMethods()
		{
			return HarmonySharedState.GetPatchedMethods();
		}

		public void Patch()
		{
			lock (locker)
			{
				var patchInfo = HarmonySharedState.GetPatchInfo(original);
				if (patchInfo == null) patchInfo = new PatchInfo();

				PatchFunctions.AddPostfix(patchInfo, instance.Id, postfix);
				PatchFunctions.UpdateWrapper(original, patchInfo);

				HarmonySharedState.UpdatePatchInfo(original, patchInfo);
			}
		}

		public void Restore()
		{
			lock (locker)
			{
				var patchInfo = HarmonySharedState.GetPatchInfo(original);
				if (patchInfo == null) return;

				PatchFunctions.RemovePostfix(patchInfo, postfix);
				PatchFunctions.UpdateWrapper(original, patchInfo);

				HarmonySharedState.UpdatePatchInfo(original, patchInfo);
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
//				if (original == null)
//					original = RunMethod<HarmonyTargetMethod, MethodBase>(null);
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

		T RunMethod<S, T>(T defaultIfNotExisting, params object[] parameters)
		{
			var methodName = typeof(S).Name.Replace("Harmony", "");

			var paramList = new List<object> { instance };
			paramList.AddRange(parameters);
			var paramTypes = AccessTools.GetTypes(paramList.ToArray());
			var method = PatchTools.GetPatchMethod<S>(container, methodName, paramTypes);
			if (method != null && typeof(T).IsAssignableFrom(method.ReturnType))
				return (T)method.Invoke(null, paramList.ToArray());

			method = PatchTools.GetPatchMethod<S>(container, methodName, new Type[] { typeof(HarmonyInstance) });
			if (method != null && typeof(T).IsAssignableFrom(method.ReturnType))
				return (T)method.Invoke(null, new object[] { instance });

			method = PatchTools.GetPatchMethod<S>(container, methodName, Type.EmptyTypes);
			if (method != null)
			{
				if (typeof(T).IsAssignableFrom(method.ReturnType))
					return (T)method.Invoke(null, Type.EmptyTypes);

				method.Invoke(null, Type.EmptyTypes);
				return defaultIfNotExisting;
			}

			return defaultIfNotExisting;
		}
	}
}
