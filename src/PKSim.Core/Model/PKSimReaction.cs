using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class PKSimReaction : ReactionBuilder, IPKSimProcess
   {
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceReaction = sourceObject as PKSimReaction;
         if (sourceReaction == null) return;
         CalculationMethod = sourceReaction.CalculationMethod;
         Rate = sourceReaction.Rate;
      }
   }
}