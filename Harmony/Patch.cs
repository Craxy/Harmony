using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Harmony
{
	public static class PatchInfoSerialization
	{
		class Binder : SerializationBinder
		{
			public override Type BindToType(string assemblyName, string typeName)
			{
				var types = new Type[] {
					typeof(PatchInfo),
					typeof(Patch[]),
					typeof(Patch)
				};
				foreach (var type in types)
					if (typeName == type.FullName)
						return type;
				var typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
				return typeToDeserialize;
			}
		}

		public static byte[] Serialize(this PatchInfo patchInfo)
		{
#pragma warning disable XS0001
			using (var streamMemory = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(streamMemory, patchInfo);
				return streamMemory.GetBuffer();
			}
#pragma warning restore XS0001
		}

		public static PatchInfo Deserialize(byte[] bytes)
		{
			var formatter = new BinaryFormatter();
			formatter.Binder = new Binder();
#pragma warning disable XS0001
			var streamMemory = new MemoryStream(bytes);
#pragma warning restore XS0001
			return (PatchInfo)formatter.Deserialize(streamMemory);
		}

		// general sorting by (in that order): before, after, and index
		public static int Comparer(object obj, int index)
		{
			var trv = Traverse.Create(obj);
			var theirIndex = trv.Field("index").GetValue<int>();

			return index.CompareTo(theirIndex);
		}
	}

	[Serializable]
	public class PatchInfo
	{
		public Patch[] postfixes;

		public PatchInfo()
		{
			postfixes = new Patch[0];
		}

		public void AddPostfix(MethodInfo patch)
		{
			AddPatch(patch, ref postfixes);
		}

		private void AddPatch(MethodInfo patch, ref Patch[] patchlist)
		{
			var l = patchlist.ToList();
			l.Add(new Patch(patch, (patchlist.LastOrDefault()?.index ?? 0) + 1));
			patchlist = l.ToArray();
		}

		public void RemovePostfix(MethodInfo patch)
		{
			RemovePatch(patch, ref postfixes);
		}

		private void RemovePatch(MethodInfo patch, ref Patch[] patchlist)
		{
			var l = patchlist.ToList();
			l.RemoveAll(p => p.patch == patch);
			patchlist = l.ToArray();
		}
	}

	[Serializable]
	public class Patch : IComparable
	{
		public readonly int index;

		public readonly MethodInfo patch;

		public Patch(MethodInfo patch, int index)
		{
			if (patch is DynamicMethod) throw new Exception("Cannot directly reference dynamic method \"" + patch + "\" in Harmony. Use a factory method instead that will return the dynamic method.");

			this.index = index;
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

		public int CompareTo(object obj)
		{
			return PatchInfoSerialization.Comparer(obj, index);
		}

		public override int GetHashCode()
		{
			return patch.GetHashCode();
		}
	}
}
