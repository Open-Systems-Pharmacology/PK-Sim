using PKSim.Assets;
using PKSim.Core;

namespace PKSim.Presentation.Core
{
   public class CannotExecuteCommandWhileErrorNotCorrectedException : PKSimException
   {
      public CannotExecuteCommandWhileErrorNotCorrectedException()
         : base(PKSimConstants.Error.CannotExecuteCommandWhileErrorNotCorrected)
      {
      }
   }
}