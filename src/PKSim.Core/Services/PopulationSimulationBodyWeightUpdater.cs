using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPopulationSimulationBodyWeightUpdater
   {
      void UpdateBodyWeightForIndividual(PopulationSimulation populationSimulation, int individualId);
      void ResetBodyWeightParameter(PopulationSimulation populationSimulation);
   }

   public class PopulationSimulationBodyWeightUpdater : IPopulationSimulationBodyWeightUpdater
   {
      private readonly IEntityPathResolver _entityPathResolver;

      public PopulationSimulationBodyWeightUpdater(IEntityPathResolver entityPathResolver)
      {
         _entityPathResolver = entityPathResolver;
      }

      public void UpdateBodyWeightForIndividual(PopulationSimulation populationSimulation, int individualId)
      {
         var bodyWeightParameter = populationSimulation.BodyWeight;
         var bodyWeightParameterPath = bodyWeightParameterPathFrom(bodyWeightParameter);
         var allBodyWeights = populationSimulation.AllValuesFor(bodyWeightParameterPath);

         if (bodyWeightParameter == null)
            return;

         bodyWeightParameter.Value = allBodyWeights.Count > individualId ? allBodyWeights[individualId] : double.NaN;
      }

      public void ResetBodyWeightParameter(PopulationSimulation populationSimulation)
      {
         populationSimulation.BodyWeight?.ResetToDefault();
      }

      private string bodyWeightParameterPathFrom(IParameter bodyWeightParameter)
      {
         return bodyWeightParameter != null ? _entityPathResolver.PathFor(bodyWeightParameter) : string.Empty;
      }
   }
}