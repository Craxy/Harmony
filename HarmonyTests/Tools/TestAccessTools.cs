using Harmony;
using HarmonyTests.Assets;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace HarmonyTests
{
	public class Test_AccessTools
	{
		[Fact]
		public void AccessTools_Field()
		{
			var type = typeof(AccessToolsClass);

			Assert.Null(AccessTools.Field(null, null));
			Assert.Null(AccessTools.Field(type, null));
			Assert.Null(AccessTools.Field(null, "field"));
			Assert.Null(AccessTools.Field(type, "unknown"));

			var field = AccessTools.Field(type, "field");
			Assert.NotNull(field);
			Assert.Equal(type, field.DeclaringType);
			Assert.Equal("field", field.Name);
		}

		[Fact]
		public void AccessTools_Property()
		{
			var type = typeof(AccessToolsClass);

			Assert.Null(AccessTools.Property(null, null));
			Assert.Null(AccessTools.Property(type, null));
			Assert.Null(AccessTools.Property(null, "property"));
			Assert.Null(AccessTools.Property(type, "unknown"));

			var prop = AccessTools.Property(type, "property");
			Assert.NotNull(prop);
			Assert.Equal(type, prop.DeclaringType);
			Assert.Equal("property", prop.Name);
		}

		[Fact]
		public void AccessTools_Method()
		{
			var type = typeof(AccessToolsClass);

			Assert.Null(AccessTools.Method(null, null));
			Assert.Null(AccessTools.Method(type, null));
			Assert.Null(AccessTools.Method(null, "method"));
			Assert.Null(AccessTools.Method(type, "unknown"));

			var m1 = AccessTools.Method(type, "method");
			Assert.NotNull(m1);
			Assert.Equal(type, m1.DeclaringType);
			Assert.Equal("method", m1.Name);

			var m2 = AccessTools.Method(type, "method", new Type[] { });
			Assert.NotNull(m2);

			var m3 = AccessTools.Method(type, "setfield", new Type[] { typeof(string) });
			Assert.NotNull(m3);
		}

		[Fact]
		public void AccessTools_InnerClass()
		{
			var type = typeof(AccessToolsClass);

			Assert.Null(AccessTools.Inner(null, null));
			Assert.Null(AccessTools.Inner(type, null));
			Assert.Null(AccessTools.Inner(null, "inner"));
			Assert.Null(AccessTools.Inner(type, "unknown"));

			var cls = AccessTools.Inner(type, "inner");
			Assert.NotNull(cls);
			Assert.Equal(type, cls.DeclaringType);
			Assert.Equal("inner", cls.Name);
		}

		[Fact]
		public void AccessTools_GetTypes()
		{
			var empty = AccessTools.GetTypes(null);
			Assert.NotNull(empty);
			Assert.Equal(0, empty.Length);

			// TODO: typeof(null) is ambiguous and resolves for now to <object>. is this a problem?
			var types = AccessTools.GetTypes(new object[] { "hi", 123, null, new Test_AccessTools() });
			Assert.NotNull(types);
			Assert.Equal(4, types.Length);
			Assert.Equal(typeof(string), types[0]);
			Assert.Equal(typeof(int), types[1]);
			Assert.Equal(typeof(object), types[2]);
			Assert.Equal(typeof(Test_AccessTools), types[3]);
		}
	}
}
