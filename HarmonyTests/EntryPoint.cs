using System;
using HarmonyTests.SimpleTest;

namespace HarmonyTests
{
  public class EntryPoint
  {
    public static void Main()
    {
      var test = new SimpleTests();
      test.Test();
      
      Console.WriteLine("---------");
    }
  }
}
