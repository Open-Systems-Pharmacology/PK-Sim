using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PopulationSimulation = PKSim.Core.Model.PopulationSimulation;

namespace PKSim.R.Mappers
{
   public interface IPopulationSimulationToIndividualValuesCacheMapper : IMapper<PopulationSimulation, IndividualValuesCache>
   {
   }

   public class PopulationSimulationToIndividualValuesCacheMapper : IPopulationSimulationToIndividualValuesCacheMapper
   {
      private readonly IEntityPathResolver _entityPathResolver;

      public PopulationSimulationToIndividualValuesCacheMapper(IEntityPathResolver entityPathResolver)
      {
         _entityPathResolver = entityPathResolver;
      }

      /// <summary>
      ///    Builds the full per-individual cache for a population simulation: the base population values plus the
      ///    simulation-level advanced parameters merged in (mirroring PopulationExportTask.CreatePopulationDataFor).
      /// </summary>
      public IndividualValuesCache MapFrom(PopulationSimulation populationSimulation)
      {
         var individualValuesCache = populationSimulation.Population.IndividualValuesCache.Clone();
         populationSimulation.AllAdvancedParameters(_entityPathResolver).Each(advancedParameter =>
         {
            var parameterPath = _entityPathResolver.PathFor(advancedParameter);
            individualValuesCache.SetValues(parameterPath, populationSimulation.AllValuesFor(parameterPath));
         });

         return individualValuesCache;
      }
   }
}
