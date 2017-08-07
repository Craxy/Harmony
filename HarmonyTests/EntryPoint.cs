using System;

namespace HarmonyTests
{
  public class EntryPoint
  {
    public static void Main()
    {
      var sp = new StaticPatches();
      sp.TestMethod1();
      sp.TestMethod2();
      
      Console.WriteLine("---------");
    }
  }
}
