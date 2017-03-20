using PKSim.Assets;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Services
{
   public class CannotCreateIndividualWithConstraintsException : PKSimException
   {
      public CannotCreateIndividualWithConstraintsException(string originDataReport)
         : base(PKSimConstants.Error.UnableToCreateIndividual.FormatWith(originDataReport))
      {
      }
   }
}