using PKSim.Assets;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class CannotCreatePopulationWithConstraintsException : PKSimException
   {
      public CannotCreatePopulationWithConstraintsException(string populationSettingsReport)
         : base(PKSimConstants.Error.UnableToCreateIndividual.FormatWith(populationSettingsReport))
      {
      }
   }
}