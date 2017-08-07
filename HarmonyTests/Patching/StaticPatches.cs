using Harmony;
using Harmony.ILCopying;
using HarmonyTests.Assets;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace HarmonyTests
{
	public class StaticPatches
	{
		[Fact]
		public void TestMethod1()
		{
			var originalClass = typeof(Class1);
			Assert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method1");
			Assert.NotNull(originalMethod);

			var patchClass = typeof(Class1Patch);
			var realPostfix = patchClass.GetMethod("Postfix");
			Assert.NotNull(realPostfix);

			Class1Patch._reset();

			MethodInfo postfixMethod;
			PatchTools.GetPatches(typeof(Class1Patch), originalMethod, out postfixMethod);

			Assert.Equal(realPostfix, postfixMethod);

			var instance = HarmonyInstance.Create("test");
			Assert.NotNull(instance);

			var patcher = new PatchProcessor(instance, originalMethod, new HarmonyMethod(postfixMethod));
			Assert.NotNull(patcher);

			var originalMethodStartPre = Memory.GetMethodStart(originalMethod);
			patcher.Patch();
			var originalMethodStartPost = Memory.GetMethodStart(originalMethod);
			Assert.Equal(originalMethodStartPre, originalMethodStartPost);
			unsafe
			{
				byte patchedCode = *(byte*) originalMethodStartPre;
				if (IntPtr.Size == sizeof(long))
					Assert.True(patchedCode == 0x48);
				else
					Assert.True(patchedCode == 0x68);
			}

			Class1.Method1();
			Assert.True(Class1Patch.originalExecuted);
			Assert.True(Class1Patch.postfixed);
		}

		[Fact]
		public void TestMethod2()
		{
			var originalClass = typeof(Class2);
			Assert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method2");
			Assert.NotNull(originalMethod);

			var patchClass = typeof(Class2Patch);
			var realPostfix = patchClass.GetMethod("Postfix");
			Assert.NotNull(realPostfix);

			Class2Patch._reset();

			MethodInfo postfixMethod;
			PatchTools.GetPatches(typeof(Class2Patch), originalMethod, out postfixMethod);

			Assert.Same(realPostfix, postfixMethod);

			var instance = HarmonyInstance.Create("test");
			Assert.NotNull(instance);

			var patcher = new PatchProcessor(instance, originalMethod, new HarmonyMethod(postfixMethod));
			Assert.NotNull(patcher);

			var originalMethodStartPre = Memory.GetMethodStart(originalMethod);
			patcher.Patch();
			var originalMethodStartPost = Memory.GetMethodStart(originalMethod);
			Assert.Equal(originalMethodStartPre, originalMethodStartPost);
			unsafe
			{
				byte patchedCode = *(byte*) originalMethodStartPre;
				if (IntPtr.Size == sizeof(long))
					Assert.True(patchedCode == 0x48);
				else
					Assert.True(patchedCode == 0x68);
			}

			new Class2().Method2();
			Assert.True(Class2Patch.originalExecuted);
			Assert.True(Class2Patch.postfixed);
		}

		[Fact]
		public void MethodRestorationTest()
		{
			var originalMethod = typeof(RestoreableClass).GetMethod("Method2");
			Assert.NotNull(originalMethod);

			MethodInfo postfixMethod;
			PatchTools.GetPatches(typeof(Class2Patch), originalMethod, out postfixMethod);

			var instance = HarmonyInstance.Create("test");
			var patcher = new PatchProcessor(instance, originalMethod, new HarmonyMethod(postfixMethod));

			// Check if the class is clean before using it for patching
			Assert.Equal(null, instance.IsPatched(originalMethod));

			var start = Memory.GetMethodStart(originalMethod);
			var oldBytes = new byte[12];
			if (IntPtr.Size == sizeof(long))
			{
				Marshal.Copy((IntPtr)start, oldBytes, 0, 12);
			}
			else
			{
				Marshal.Copy((IntPtr)start, oldBytes, 0, 6);
			}

			patcher.Patch();

			patcher.Restore();

			var newBytes = new byte[12];
			if (IntPtr.Size == sizeof(long))
			{
				Marshal.Copy((IntPtr)start, newBytes, 0, 12);
			}
			else
			{
				Marshal.Copy((IntPtr)start, newBytes, 0, 6);
			}
			for (int i = 0; i < oldBytes.Length; i++)
			{
				Assert.Equal(oldBytes[i], newBytes[i]);
			}

			Class2Patch._reset();
			new RestoreableClass().Method2();

			Assert.True(Class2Patch.originalExecuted);
			Assert.False(Class2Patch.postfixed);

			Assert.Same(0, instance.IsPatched(originalMethod).Postfixes.Count);
		}
	}
}
