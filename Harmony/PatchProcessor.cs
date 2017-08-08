using System.Reflection;
using System.Runtime.CompilerServices;

namespace Harmony
{
  public class PatchProcessor
  {
    private readonly MethodBase _original;
    private readonly HarmonyMethod _postfix;
    private PatchInfo _patchInfo;

    public PatchProcessor(MethodBase original, HarmonyMethod postfix)
    {
      _original = original;
      _postfix = postfix ?? new HarmonyMethod(null);
    }

    public void Patch()
    {
      if (_patchInfo == null)
      {
        _patchInfo = new PatchInfo();
      }

      PatchFunctions.AddPostfix(_patchInfo, _postfix);
      PatchFunctions.UpdateWrapper(_original, _patchInfo);
    }

    public void Restore()
    {
      if (_patchInfo == null)
      {
        return;
      }

      PatchFunctions.RemovePostfix(_patchInfo, _postfix);
      PatchFunctions.UpdateWrapper(_original, _patchInfo);
    }

    public static bool DEBUG = false;

    public static PatchProcessor Patch(MethodBase original, HarmonyMethod postfix)
    {
      var processor = new PatchProcessor(original, postfix);
      processor.Patch();
      return processor;
    }
    public static PatchProcessor Patch(MethodBase original, MethodInfo postfix)
    {
      return Patch(original, new HarmonyMethod(postfix));
    }
  }
}
