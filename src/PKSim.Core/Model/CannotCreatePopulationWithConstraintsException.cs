using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class CannotCreatePopulationWithConstraintsException : PKSimException
   {
      public CannotCreatePopulationWithConstraintsException(string populationSettingsReport)
         : base(PKSimConstants.Error.UnableToCreateIndividual(populationSettingsReport))
      {
      }
   }
}