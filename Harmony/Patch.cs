using System;
using System.Diagnostics;
using System.Reflection;

namespace Harmony
{
  public sealed class Patch : IDisposable
  {
    private MethodBase _original;
    private MethodInfo _postfix;
    private PatchFunctions.PatchHandle _patchHandle;
    public bool IsPatched => _patchHandle != null && !_patchHandle.IsDisposed;

    private Patch(MethodBase original, MethodInfo postfix, PatchFunctions.PatchHandle patchHandle)
    {
      _original = original;
      _postfix = postfix;
      _patchHandle = patchHandle;
    }

    /// <returns>Must be kept alive</returns>
    public static Patch Apply(MethodInfo original, MethodInfo postfix)
    {
      return new Patch(original, postfix, PatchFunctions.Update(original, postfix));
    }

    public void Restore()
    {
      if (IsPatched)
      {
        PatchFunctions.Restore(_original, _patchHandle);
      }
      Debug.Assert(!IsPatched);
      
      _patchHandle = null;
      
      _original = null;
      _postfix = null;
    }

    public void Dispose()
    {
      Restore();
    }

    public static bool DEBUG = false;
  }
}
