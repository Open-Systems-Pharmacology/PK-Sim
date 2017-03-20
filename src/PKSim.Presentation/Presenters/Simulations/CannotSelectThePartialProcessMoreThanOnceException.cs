using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.Simulations
{
   public class CannotSelectThePartialProcessMoreThanOnceException : PKSimException
   {
      public CannotSelectThePartialProcessMoreThanOnceException(PartialProcess partialProcess) : base(PKSimConstants.Error.CannotSelectTheSamePartialProcessMoreThanOnce(partialProcess.ToString()))
      {
      }
   }
}