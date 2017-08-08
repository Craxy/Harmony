using System.Reflection;

namespace Harmony
{
  public class PatchProcessor
  {
    private readonly MethodBase _original;
    private readonly HarmonyMethod _postfix;
    private PatchInfo _patchInfo;

    public PatchProcessor(MethodBase original, HarmonyMethod postfix)
    {
      this._original = original;
      this._postfix = postfix ?? new HarmonyMethod(null);
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
  }
}
