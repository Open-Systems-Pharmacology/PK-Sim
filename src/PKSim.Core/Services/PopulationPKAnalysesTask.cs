using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface IPopulationPKAnalysesTask : IPKAnalysesTask
   {
      /// <summary>
      ///    Calculates the PKAnalyses for the given <paramref name="populationSimulation" />. It does not delete the previous pk
      ///    calculation from the <paramref name="populationSimulation" />
      /// </summary>
      /// <param name="populationSimulation">Population simulation for which pk parameters should be calculated</param>
      /// <returns>The PopulationSimulationPKAnalyses containing all calculated values</returns>
      PopulationSimulationPKAnalyses CalculateFor(PopulationSimulation populationSimulation);
   }

   public class PopulationPKAnalysesTask : PKAnalysesTask, IPopulationPKAnalysesTask
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IEntityPathResolver _entityPathResolver;

      public PopulationPKAnalysesTask(ILazyLoadTask lazyLoadTask, IPKValuesCalculator pkValuesCalculator, IPKParameterRepository pkParameterRepository,
         IPKCalculationOptionsFactory pkCalculationOptionsFactory, IEntityPathResolver entityPathResolver) : base(lazyLoadTask, pkValuesCalculator, pkParameterRepository, pkCalculationOptionsFactory)
      {
         _lazyLoadTask = lazyLoadTask;
         _entityPathResolver = entityPathResolver;
      }

      public PopulationSimulationPKAnalyses CalculateFor(PopulationSimulation populationSimulation)
      {
         _lazyLoadTask.LoadResults(populationSimulation);
         if (!populationSimulation.HasResults)
            return new NullPopulationSimulationPKAnalyses();

         var bodyWeightParameter = populationSimulation.BodyWeight;
         var bodyWeightParameterPath = bodyWeightParameterPathFrom(bodyWeightParameter);
         var allBodyWeights = populationSimulation.AllValuesFor(bodyWeightParameterPath);

         try
         {
            return base.CalculateFor(populationSimulation, populationSimulation.NumberOfItems, populationSimulation.Results, (individualId) => { updateBodyWeightFromCurrentIndividual(bodyWeightParameter, allBodyWeights, individualId); });
         }
         finally
         {
            bodyWeightParameter?.ResetToDefault();
         }
      }

      private string bodyWeightParameterPathFrom(IParameter bodyWeightParameter)
      {
         return bodyWeightParameter != null ? _entityPathResolver.PathFor(bodyWeightParameter) : string.Empty;
      }

      private void updateBodyWeightFromCurrentIndividual(IParameter bodyWeightParameter, IReadOnlyList<double> allBodyWeights, int individualId)
      {
         if (bodyWeightParameter == null)
            return;

         bodyWeightParameter.Value = allBodyWeights.Count > individualId ? allBodyWeights[individualId] : double.NaN;
      }
   }
}