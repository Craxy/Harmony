using Harmony;
using HarmonyTests.Assets;
using Xunit;

namespace HarmonyTests
{
	public class TestMethodInvoker
    {

		[Fact]
        public void TestMethodInvokerGeneral()
        {
            var type = typeof(MethodInvokerClass);
            Assert.NotNull(type);
            var method = type.GetMethod("Method1");
            Assert.NotNull(method);

            var handler = MethodInvoker.GetHandler(method);
            Assert.NotNull(handler);

            object[] args = new object[] { 1, 0, 0, /*out*/ null, /*ref*/ new TestMethodInvokerStruct() };
            handler(null, args);
            Assert.Equal(args[0], 1);
            Assert.Equal(args[1], 1);
            Assert.Equal(args[2], 2);
            Assert.Equal(((TestMethodInvokerObject) args[3])?.Value, 1);
            Assert.Equal(((TestMethodInvokerStruct) args[4]).Value, 1);
        }

        [Fact]
        public void TestMethodInvokerSelfObject()
        {
            var type = typeof(TestMethodInvokerObject);
            Assert.NotNull(type);
            var method = type.GetMethod("Method1");
            Assert.NotNull(method);

            var handler = MethodInvoker.GetHandler(method);
            Assert.NotNull(handler);

            var instance = new TestMethodInvokerObject();
            instance.Value = 1;

            object[] args = new object[] { 2 };
            handler(instance, args);
            Assert.Equal(instance.Value, 3);
        }

    }
}
