using Harmony;
using HarmonyTests.Assets;
using Xunit;

namespace HarmonyTests.Tools
{
	public class Test_Attributes
	{
		[Fact]
		public void TestAttributes()
		{
			var type = typeof(AllAttributesClass);
			var infos = type.GetHarmonyMethods();
			var info = HarmonyMethod.Merge(infos);
			Assert.NotNull(info);
			Assert.Equal(typeof(string), info.originalType);
			Assert.Equal("foobar", info.methodName);
			Assert.NotNull(info.parameter);
			Assert.Equal(2, info.parameter.Length);
			Assert.Equal(typeof(float), info.parameter[0]);
			Assert.Equal(typeof(string), info.parameter[1]);
		}
	}
}
