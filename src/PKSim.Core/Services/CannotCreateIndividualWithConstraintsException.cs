using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class CannotCreateIndividualWithConstraintsException : PKSimException
   {
      public CannotCreateIndividualWithConstraintsException(string originDataReport)
         : base(PKSimConstants.Error.UnableToCreateIndividual(originDataReport))
      {
      }
   }
}