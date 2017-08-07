using Harmony;
using HarmonyTests.Assets;
using Xunit;

namespace HarmonyTests
{
	public class TestTraverse_Properties
	{
		// Traverse.ToString() should return the value of a traversed property
		//
		[Fact]
		public void Traverse_Property_ToString()
		{
			var instance = new TraverseProperties_AccessModifiers(TraverseProperties.testStrings);

			var trv = Traverse.Create(instance).Property(TraverseProperties.propertyNames[0]);
			Assert.Equal(TraverseProperties.testStrings[0], trv.ToString());
		}

		// Traverse.GetValue() should return the value of a traversed property
		// regardless of its access modifier
		//
		[Fact]
		public void Traverse_Property_GetValue()
		{
			var instance = new TraverseProperties_AccessModifiers(TraverseProperties.testStrings);
			var trv = Traverse.Create(instance);

			for (int i = 0; i < TraverseProperties.testStrings.Length; i++)
			{
				var name = TraverseProperties.propertyNames[i];
				var ptrv = trv.Property(name);
				Assert.NotNull(ptrv);

				Assert.Equal(TraverseProperties.testStrings[i], ptrv.GetValue());
				Assert.Equal(TraverseProperties.testStrings[i], ptrv.GetValue<string>());
			}
		}

		// Traverse.SetValue() should set the value of a traversed property
		// regardless of its access modifier
		//
		[Fact]
		public void Traverse_Property_SetValue()
		{
			var instance = new TraverseProperties_AccessModifiers(TraverseProperties.testStrings);
			var trv = Traverse.Create(instance);

			for (int i = 0; i < TraverseProperties.testStrings.Length - 1; i++)
			{
				var newValue = "newvalue" + i;

				// before
				Assert.Equal(TraverseProperties.testStrings[i], instance.GetTestProperty(i));

				var name = TraverseProperties.propertyNames[i];
				var ptrv = trv.Property(name);
				ptrv.SetValue(newValue);

				// after
				Assert.Equal(newValue, instance.GetTestProperty(i));
				Assert.Equal(newValue, ptrv.GetValue());
				Assert.Equal(newValue, ptrv.GetValue<string>());
			}
		}
	}
}
