using System.Reflection;
using System.Runtime.CompilerServices;

namespace Harmony
{
  public class PatchProcessor
  {
    private readonly MethodBase _original;
    private readonly MethodInfo _postfix;

    public PatchProcessor(MethodBase original, MethodInfo postfix)
    {
      _original = original;
      _postfix = postfix;
    }

    //todo: state for IsPatched
    public void Patch()
    {
      PatchFunctions.UpdateWrapper(_original, _postfix);
    }

    public void Restore()
    {
      PatchFunctions.UpdateWrapper(_original, null);
    }

    public static bool DEBUG = false;

    public static PatchProcessor Patch(MethodBase original, MethodInfo postfix)
    {
      var processor = new PatchProcessor(original, postfix);
      processor.Patch();
      return processor;
    }
  }
}
