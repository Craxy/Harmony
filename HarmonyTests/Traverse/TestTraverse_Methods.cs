using Harmony;
using HarmonyTests.Assets;
using System;
using Xunit;

namespace HarmonyTests
{
	public class TestTraverse_Methods
	{
		[Fact]
		public void Traverse_Missing_Method()
		{
			var instance = new TraverseMethods_Instance();
			var trv = Traverse.Create(instance);

			string methodSig1 = "";
			try
			{
				trv.Method("FooBar", new object[] { "hello", 123 });
			}
			catch (MissingMethodException e)
			{
				methodSig1 = e.Message;
			}
			Assert.Equal("FooBar(System.String, System.Int32)", methodSig1);

			string methodSig2 = "";
			try
			{
				var types = new Type[] { typeof(string), typeof(int) };
				trv.Method("FooBar", types, new object[] { "hello", 123 });
			}
			catch (MissingMethodException e)
			{
				methodSig2 = e.Message;
			}
			Assert.Equal("FooBar(System.String, System.Int32)", methodSig2);
		}

		[Fact]
		public void Traverse_Method_Instance()
		{
			var instance = new TraverseMethods_Instance();
			var trv = Traverse.Create(instance);

			instance.Method1_called = false;
			var mtrv1 = trv.Method("Method1");
			Assert.Equal(null, mtrv1.GetValue());
			Assert.Equal(true, instance.Method1_called);

			var mtrv2 = trv.Method("Method2", new object[] { "arg" });
			Assert.Equal("argarg", mtrv2.GetValue());
		}

		[Fact]
		public void Traverse_Method_Static()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Static));
			var mtrv = trv.Method("StaticMethod", new object[] { 6, 7 });
			Assert.Equal(42, mtrv.GetValue<int>());
		}

		[Fact]
		public void Traverse_Method_VariableArguments()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_VarArgs));

			Assert.Equal(30, trv.Method("Test1", 10, 20).GetValue<int>());
			Assert.Equal(60, trv.Method("Test2", 10, 20, 30).GetValue<int>());

			// Calling varargs methods directly won't work. Use parameter array instead
			// Assert.AreEqual(60, trv.Method("Test3", 100, 10, 20, 30).GetValue<int>());
			Assert.Equal(6000, trv.Method("Test3", 100, new int[] { 10, 20, 30 }).GetValue<int>());
		}

		[Fact]
		public void Traverse_Method_RefParameters()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Parameter));

			string result = null;
			var parameters = new object[] { result };
			var types = new Type[] { typeof(string).MakeByRefType() };
			var mtrv1 = trv.Method("WithRefParameter", types, parameters);
			Assert.Equal("ok", mtrv1.GetValue<string>());
			Assert.Equal("hello", parameters[0]);
		}

		[Fact]
		public void Traverse_Method_OutParameters()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Parameter));

			string result = null;
			var parameters = new object[] { result };
			var types = new Type[] { typeof(string).MakeByRefType() };
			var mtrv1 = trv.Method("WithOutParameter", types, parameters);
			Assert.Equal("ok", mtrv1.GetValue<string>());
			Assert.Equal("hello", parameters[0]);
		}
	}
}
