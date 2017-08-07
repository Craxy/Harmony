using System.Collections.Generic;
using System.Linq;
using HarmonyTests.Assets;
using Harmony;
using Xunit;

namespace HarmonyTests
{
	public class TestTraverse_Types
	{
		private class InnerClass { }

		[Fact]
		public void Traverse_Types()
		{
			var instance = new Assets.TraverseTypes<InnerClass>();
			var trv = Traverse.Create(instance);

			Assert.Equal(
				100,
				trv.Field("IntField").GetValue<int>()
			);

			Assert.Equal(
				"hello",
				trv.Field("StringField").GetValue<string>()
			);

			var boolArray = trv.Field("ListOfBoolField").GetValue<IEnumerable<bool>>().ToArray();
			Assert.Equal(true, boolArray[0]);
			Assert.Equal(false, boolArray[1]);

			var mixed = trv.Field("MixedField").GetValue<Dictionary<InnerClass, List<string>>>();
			var key = trv.Field("key").GetValue<InnerClass>();

			List<string> value;
			mixed.TryGetValue(key, out value);
			Assert.Equal("world", value.First());

			var trvEmpty = Traverse.Create(instance).Type("FooBar");
			TestTraverse_Basics.AssertIsEmpty(trvEmpty);
		}

		[Fact]
		public void Traverse_InnerInstance()
		{
			var instance = new TraverseNestedTypes(null);

			var trv1 = Traverse.Create(instance);
			var field1 = trv1.Field("innerInstance").Field("inner2").Field("field");
			field1.SetValue("somevalue");

			var trv2 = Traverse.Create(instance);
			var field2 = trv1.Field("innerInstance").Field("inner2").Field("field");
			Assert.Equal("somevalue", field2.GetValue());
		}

		[Fact]
		public void Traverse_InnerStatic()
		{
			var trv1 = Traverse.Create(typeof(TraverseNestedTypes));
			var field1 = trv1.Field("innerStatic").Field("inner2").Field("field");
			field1.SetValue("somevalue1");

			var trv2 = Traverse.Create(typeof(TraverseNestedTypes));
			var field2 = trv1.Field("innerStatic").Field("inner2").Field("field");
			Assert.Equal("somevalue1", field2.GetValue());

			var _ = new TraverseNestedTypes("somevalue2");
			var value = Traverse
				.Create(typeof(TraverseNestedTypes))
				.Type("InnerStaticClass1")
				.Type("InnerStaticClass2")
				.Field("field")
				.GetValue<string>();
			Assert.Equal("somevalue2", value);
		}
	}
}
