using Harmony;
using HarmonyTests.Assets;
using Xunit;

namespace HarmonyTests
{
	public class TestTraverse_Fields
	{
		// Traverse.ToString() should return the value of a traversed field
		//
		[Fact]
		public void Traverse_Field_ToString()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);

			var trv = Traverse.Create(instance).Field(TraverseFields.fieldNames[0]);
			Assert.Equal(TraverseFields.testStrings[0], trv.ToString());
		}

		// Traverse.GetValue() should return the value of a traversed field
		// regardless of its access modifier
		//
		[Fact]
		public void Traverse_Field_GetValue()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);
			var trv = Traverse.Create(instance);

			for (int i = 0; i < TraverseFields.testStrings.Length; i++)
			{
				var name = TraverseFields.fieldNames[i];
				var ftrv = trv.Field(name);
				Assert.NotNull(ftrv);

				Assert.Equal(TraverseFields.testStrings[i], ftrv.GetValue());
				Assert.Equal(TraverseFields.testStrings[i], ftrv.GetValue<string>());
			}
		}

		// Traverse.SetValue() should set the value of a traversed field
		// regardless of its access modifier
		//
		[Fact]
		public void Traverse_Field_SetValue()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);
			var trv = Traverse.Create(instance);

			for (int i = 0; i < TraverseFields.testStrings.Length; i++)
			{
				var newValue = "newvalue" + i;

				// before
				Assert.Equal(TraverseFields.testStrings[i], instance.GetTestField(i));

				var name = TraverseFields.fieldNames[i];
				var ftrv = trv.Field(name);
				ftrv.SetValue(newValue);

				// after
				Assert.Equal(newValue, instance.GetTestField(i));
				Assert.Equal(newValue, ftrv.GetValue());
				Assert.Equal(newValue, ftrv.GetValue<string>());
			}
		}
	}
}
