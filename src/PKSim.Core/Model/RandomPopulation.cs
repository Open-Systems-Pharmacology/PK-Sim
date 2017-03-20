using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class RandomPopulation : Population
   {
      /// <summary>
      ///    Population settings for the given random population
      /// </summary>
      public virtual RandomPopulationSettings Settings { get; set; }

      public override Individual FirstIndividual => Settings?.BaseIndividual;

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourcePopulation = sourceObject as RandomPopulation;
         if (sourcePopulation == null) return;
         Settings = sourcePopulation.Settings.Clone(cloneManager);
      }
   }
}