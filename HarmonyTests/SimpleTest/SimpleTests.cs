using System.Collections.Generic;
using Harmony;
using HarmonyTests.SimpleTest.Assets;
using Xunit;

namespace HarmonyTests.SimpleTest
{
  public class SimpleTests
  {
    [Fact]
    public void Test()
    {
      var obj = new SimpleTestClass();

      var src = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.Source));
      var prefix = typeof(SimpleTestClass).GetMethod(nameof(SimpleTestClass.Postfix));

      var parameter1 = "Hello Wrodl";
      var parameter2 = 123456;
      
      var expectedInput = new object[] {parameter1, parameter2};
      var expectedOutput = SimpleTestClass.SourceReturn;
      var expectedPostfixInput = new object[] {parameter1, parameter2, expectedOutput};
      var emptyList = new List<object>();

      {
        var result = obj.Source(parameter1, parameter2);
        Assert.True(obj.SourceCalled);
        Assert.Equal(expectedOutput, result);
        Assert.Equal(expectedInput, obj.SourceInputs);
        
        Assert.False(SimpleTestClass.PostfixCalled);
        Assert.Equal(emptyList, SimpleTestClass.PostfixInputs);
        
        obj.Reset();
      }
      
      var hi = HarmonyInstance.Create();
      var patch = hi.Patch(src, new HarmonyMethod(prefix));
      
      {
        var result = obj.Source(parameter1, parameter2);
        Assert.True(obj.SourceCalled);
        Assert.Equal(expectedOutput, result);
        Assert.Equal(expectedInput, obj.SourceInputs);
        
        Assert.True(SimpleTestClass.PostfixCalled);
        Assert.Equal(expectedPostfixInput, SimpleTestClass.PostfixInputs);
        
        obj.Reset();
      }
      
      patch.Restore();
      
      {
        var result = obj.Source(parameter1, parameter2);
        Assert.True(obj.SourceCalled);
        Assert.Equal(expectedOutput, result);
        Assert.Equal(expectedInput, obj.SourceInputs);
        
        Assert.False(SimpleTestClass.PostfixCalled);
        Assert.Equal(emptyList, SimpleTestClass.PostfixInputs);
        
        obj.Reset();
      }
    }
  }
}
