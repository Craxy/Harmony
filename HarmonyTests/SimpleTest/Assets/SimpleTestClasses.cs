using System.Collections.Generic;

namespace HarmonyTests.SimpleTest.Assets
{
  public class SimpleTestClass
  {
    public bool SourceCalled = false;
    public readonly List<object> SourceInputs = new List<object>();
		
    public const string SourceReturn = "Return from Source";
    public string Source(string value1, int value2)
    {
      SourceCalled = true;
      SourceInputs.Add(value1);
      SourceInputs.Add(value2);
      return SourceReturn;
    }

    public static bool PostfixCalled = false;
    public static readonly List<object> PostfixInputs = new List<object>();
    public static void Postfix(string value1, int value2, string __result)
    {
      PostfixCalled = true;
      PostfixInputs.Add(value1);
      PostfixInputs.Add(value2);
      PostfixInputs.Add(__result);
    }

    public void Reset()
    {
      SourceCalled = false;
      SourceInputs.Clear();
      PostfixCalled = false;
      PostfixInputs.Clear();
    }
  }
}
