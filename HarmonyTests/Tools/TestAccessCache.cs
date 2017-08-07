using Harmony;
using HarmonyTests.Assets;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace HarmonyTests
{
	public class Test_AccessCache
	{
		private void InjectField(AccessCache cache)
		{
			var f_fields = cache.GetType().GetField("fields", AccessTools.all);
			Assert.NotNull(f_fields);
			var fields = (Dictionary<Type, Dictionary<string, FieldInfo>>)f_fields.GetValue(cache);
			Assert.NotNull(fields);
			Dictionary<string, FieldInfo> infos;
			fields.TryGetValue(typeof(AccessToolsClass), out infos);
			Assert.NotNull(infos);

			infos.Remove("field");
			infos.Add("field", typeof(AccessToolsClass).GetField("field2", AccessTools.all));
		}

		private void InjectProperty(AccessCache cache)
		{
			var f_properties = cache.GetType().GetField("properties", AccessTools.all);
			Assert.NotNull(f_properties);
			var properties = (Dictionary<Type, Dictionary<string, PropertyInfo>>)f_properties.GetValue(cache);
			Assert.NotNull(properties);
			Dictionary<string, PropertyInfo> infos;
			properties.TryGetValue(typeof(AccessToolsClass), out infos);
			Assert.NotNull(infos);

			infos.Remove("property");
			infos.Add("property", typeof(AccessToolsClass).GetProperty("property2", AccessTools.all));
		}

		private void InjectMethod(AccessCache cache)
		{
			var f_methods = cache.GetType().GetField("methods", AccessTools.all);
			Assert.NotNull(f_methods);
			var methods = (Dictionary<Type, Dictionary<string, Dictionary<Type[], MethodInfo>>>)f_methods.GetValue(cache);
			Assert.NotNull(methods);
			Dictionary<string, Dictionary<Type[], MethodInfo>> dicts;
			methods.TryGetValue(typeof(AccessToolsClass), out dicts);
			Assert.NotNull(dicts);
			Dictionary<Type[], MethodInfo> infos;
			dicts.TryGetValue("method", out infos);
			Assert.NotNull(dicts);

			infos.Remove(Type.EmptyTypes);
			infos.Add(Type.EmptyTypes, typeof(AccessToolsClass).GetMethod("method2", AccessTools.all));
		}

		[Fact]
		public void AccessCache_Field()
		{
			var type = typeof(AccessToolsClass);

			Assert.NotNull((new AccessCache()).GetFieldInfo(type, "field"));

			var cache1 = new AccessCache();
			var finfo1 = cache1.GetFieldInfo(type, "field");
			InjectField(cache1);
			var cache2 = new AccessCache();
			var finfo2 = cache2.GetFieldInfo(type, "field");
			Assert.Same(finfo1, finfo2);

			var cache = new AccessCache();
			var finfo3 = cache.GetFieldInfo(type, "field");
			InjectField(cache);
			var finfo4 = cache.GetFieldInfo(type, "field");
			Assert.NotSame(finfo3, finfo4);
		}

		[Fact]
		public void AccessCache_Property()
		{
			var type = typeof(AccessToolsClass);

			Assert.NotNull((new AccessCache()).GetPropertyInfo(type, "property"));

			var cache1 = new AccessCache();
			var pinfo1 = cache1.GetPropertyInfo(type, "property");
			InjectProperty(cache1);
			var cache2 = new AccessCache();
			var pinfo2 = cache2.GetPropertyInfo(type, "property");
			Assert.Same(pinfo1, pinfo2);

			var cache = new AccessCache();
			var pinfo3 = cache.GetPropertyInfo(type, "property");
			InjectProperty(cache);
			var pinfo4 = cache.GetPropertyInfo(type, "property");
			Assert.NotSame(pinfo3, pinfo4);
		}

		[Fact]
		public void AccessCache_Method()
		{
			var type = typeof(AccessToolsClass);

			Assert.NotNull((new AccessCache()).GetMethodInfo(type, "method", Type.EmptyTypes));

			var cache1 = new AccessCache();
			var minfo1 = cache1.GetMethodInfo(type, "method", Type.EmptyTypes);
			InjectMethod(cache1);
			var cache2 = new AccessCache();
			var minfo2 = cache2.GetMethodInfo(type, "method", Type.EmptyTypes);
			Assert.Same(minfo1, minfo2);

			var cache = new AccessCache();
			var minfo3 = cache.GetMethodInfo(type, "method", Type.EmptyTypes);
			InjectMethod(cache);
			var minfo4 = cache.GetMethodInfo(type, "method", Type.EmptyTypes);
			Assert.NotSame(minfo3, minfo4);
		}
	}
}
