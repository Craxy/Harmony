using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	[Serializable]
	public class Patch// : IComparable
	{
		public readonly MethodInfo patch;

		public Patch(MethodInfo patch)
		{
			if (patch is DynamicMethod) throw new Exception("Cannot directly reference dynamic method \"" + patch + "\" in Harmony. Use a factory method instead that will return the dynamic method.");

			this.patch = patch;
		}

		public MethodInfo GetMethod(MethodBase original)
		{
			if (patch.ReturnType != typeof(DynamicMethod)) return patch;
			if (patch.IsStatic == false) return patch;
			var parameters = patch.GetParameters();
			if (parameters.Count() != 1) return patch;
			if (parameters[0].ParameterType != typeof(MethodBase)) return patch;

			// we have a DynamicMethod factory, let's use it
			return patch.Invoke(null, new object[] { original }) as DynamicMethod;
		}

		public override bool Equals(object obj)
		{
			return ((obj != null) && (obj is Patch) && (patch == ((Patch)obj).patch));
		}

		public override int GetHashCode()
		{
			return patch.GetHashCode();
		}
	}
}
