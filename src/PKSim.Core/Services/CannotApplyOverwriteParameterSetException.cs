using System.Collections.Generic;
using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class CannotApplyOverwriteParameterSetException : PKSimException
   {
      public CannotApplyOverwriteParameterSetException(IReadOnlyList<string> unresolvedPaths)
         : base(PKSimConstants.Error.CannotApplyOverwriteParameterSetUnresolvedPaths(unresolvedPaths))
      {
      }
   }
}
