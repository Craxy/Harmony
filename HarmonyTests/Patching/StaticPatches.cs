using Harmony;
using Harmony.ILCopying;
using HarmonyTests.Assets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HarmonyTests
{
	[TestClass]
	public class StaticPatches
	{
		[TestMethod]
		public void TestMethod1()
		{
			var originalClass = typeof(Class1);
			Assert.IsNotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method1");
			Assert.IsNotNull(originalMethod);

			var patchClass = typeof(Class1Patch);
			var realPostfix = patchClass.GetMethod("Postfix");
			Assert.IsNotNull(realPostfix);

			Class1Patch._reset();

			MethodInfo postfixMethod;
			PatchTools.GetPatches(typeof(Class1Patch), originalMethod, out postfixMethod);

			Assert.AreSame(realPostfix, postfixMethod);

			var instance = HarmonyInstance.Create("test");
			Assert.IsNotNull(instance);

			var patcher = new PatchProcessor(instance, originalMethod, new HarmonyMethod(postfixMethod));
			Assert.IsNotNull(patcher);

			var originalMethodStartPre = Memory.GetMethodStart(originalMethod);
			patcher.Patch();
			var originalMethodStartPost = Memory.GetMethodStart(originalMethod);
			Assert.AreEqual(originalMethodStartPre, originalMethodStartPost);
			unsafe
			{
				byte patchedCode = *(byte*) originalMethodStartPre;
				if (IntPtr.Size == sizeof(long))
					Assert.IsTrue(patchedCode == 0x48);
				else
					Assert.IsTrue(patchedCode == 0x68);
			}

			Class1.Method1();
			Assert.IsTrue(Class1Patch.originalExecuted);
			Assert.IsTrue(Class1Patch.postfixed);
		}

		[TestMethod]
		public void TestMethod2()
		{
			var originalClass = typeof(Class2);
			Assert.IsNotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method2");
			Assert.IsNotNull(originalMethod);

			var patchClass = typeof(Class2Patch);
			var realPostfix = patchClass.GetMethod("Postfix");
			Assert.IsNotNull(realPostfix);

			Class2Patch._reset();

			MethodInfo postfixMethod;
			PatchTools.GetPatches(typeof(Class2Patch), originalMethod, out postfixMethod);

			Assert.AreSame(realPostfix, postfixMethod);

			var instance = HarmonyInstance.Create("test");
			Assert.IsNotNull(instance);

			var patcher = new PatchProcessor(instance, originalMethod, new HarmonyMethod(postfixMethod));
			Assert.IsNotNull(patcher);

			var originalMethodStartPre = Memory.GetMethodStart(originalMethod);
			patcher.Patch();
			var originalMethodStartPost = Memory.GetMethodStart(originalMethod);
			Assert.AreEqual(originalMethodStartPre, originalMethodStartPost);
			unsafe
			{
				byte patchedCode = *(byte*) originalMethodStartPre;
				if (IntPtr.Size == sizeof(long))
					Assert.IsTrue(patchedCode == 0x48);
				else
					Assert.IsTrue(patchedCode == 0x68);
			}

			new Class2().Method2();
			Assert.IsTrue(Class2Patch.prefixed);
			Assert.IsTrue(Class2Patch.originalExecuted);
			Assert.IsTrue(Class2Patch.postfixed);
		}

		[TestMethod]
		public void MethodRestorationTest()
		{
			var originalMethod = typeof(RestoreableClass).GetMethod("Method2");
			Assert.IsNotNull(originalMethod);

			MethodInfo postfixMethod;
			PatchTools.GetPatches(typeof(Class2Patch), originalMethod, out postfixMethod);

			var instance = HarmonyInstance.Create("test");
			var patcher = new PatchProcessor(instance, originalMethod, new HarmonyMethod(postfixMethod));

			// Check if the class is clean before using it for patching
			Assert.AreEqual(null, instance.IsPatched(originalMethod), "Class already patched!");

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
				Assert.AreEqual(oldBytes[i], newBytes[i], string.Format("Byte {0} differs after restoration", i));
			}

			Class2Patch._reset();
			new RestoreableClass().Method2();

			Assert.IsTrue(Class2Patch.originalExecuted);
			Assert.IsFalse(Class2Patch.postfixed);

			Assert.AreEqual(0, instance.IsPatched(originalMethod).Postfixes.Count);
		}
	}
}
