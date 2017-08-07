using Harmony;
using HarmonyTests.Assets;
using System;
using Xunit;

namespace HarmonyTests
{
	public class TestTraverse_Basics
	{
		// Basic integrity check for our test class and the field-testvalue relations
		//
		[Fact]
		public void Instantiate_TraverseFields_AccessModifiers()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);

			for (int i = 0; i < TraverseFields.testStrings.Length; i++)
				Assert.Equal(TraverseFields.testStrings[i], instance.GetTestField(i));
		}

		public static void AssertIsEmpty(Traverse trv)
		{
			Assert.Equal(null, AccessTools.Field(typeof(Traverse), "_root").GetValue(trv));
			Assert.Equal(null, AccessTools.Field(typeof(Traverse), "_type").GetValue(trv));
			Assert.Equal(null, AccessTools.Field(typeof(Traverse), "_info").GetValue(trv));
			Assert.Equal(null, AccessTools.Field(typeof(Traverse), "_index").GetValue(trv));
		}

		class FooBar
		{
#pragma warning disable CS0169
			string field;
#pragma warning restore CS0169
		}

		// Traverse should default to an empty instance to avoid errors
		//
		[Fact]
		public void Traverse_SilentFailures()
		{
			var trv1 = new Traverse(null);
			AssertIsEmpty(trv1);

			trv1 = Traverse.Create(null);
			AssertIsEmpty(trv1);

			var trv2 = trv1.Type("FooBar");
			AssertIsEmpty(trv2);

			var trv3 = Traverse.Create<FooBar>().Field("field");
			AssertIsEmpty(trv3);

			var trv4 = new Traverse(new FooBar()).Field("field");
			AssertIsEmpty(trv4.Method("", new object[0]));
			AssertIsEmpty(trv4.Method("", new Type[0], new object[0]));
		}

		// Traverse should handle basic null values
		//
		[Fact]
		public void Traverse_Create_With_Null()
		{
			var trv = Traverse.Create(null);

			Assert.NotNull(trv);
			Assert.Null(trv.ToString());

			// field access

			var ftrv = trv.Field("foo");
			Assert.NotNull(ftrv);

			Assert.Null(ftrv.GetValue());
			Assert.Null(ftrv.ToString());
			Assert.Equal(0, ftrv.GetValue<int>());
			Assert.Same(ftrv, ftrv.SetValue(123));

			// property access

			var ptrv = trv.Property("foo");
			Assert.NotNull(ptrv);

			Assert.Null(ptrv.GetValue());
			Assert.Null(ptrv.ToString());
			Assert.Null(ptrv.GetValue<string>());
			Assert.Same(ptrv, ptrv.SetValue("test"));

			// method access

			var mtrv = trv.Method("zee");
			Assert.NotNull(mtrv);

			Assert.Null(mtrv.GetValue());
			Assert.Null(mtrv.ToString());
			Assert.Equal(0, mtrv.GetValue<float>());
			Assert.Same(mtrv, mtrv.SetValue(null));
		}

		// Traverse.ToString() should return a meaningful string representation of its initial value
		//
		[Fact]
		public void Traverse_Create_Instance_ToString()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);

			var trv = Traverse.Create(instance);
			Assert.Equal(instance.ToString(), trv.ToString());
		}

		// Traverse.ToString() should return a meaningful string representation of its initial type
		//
		[Fact]
		public void Traverse_Create_Type_ToString()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);

			var type = typeof(TraverseFields_AccessModifiers);
			var trv = Traverse.Create(type);
			Assert.Equal(type.ToString(), trv.ToString());
		}
	}
}
