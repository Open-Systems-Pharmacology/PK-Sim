using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v6_1;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v6_1
{
   public class When_converting_the_PopulationRange_565_project : ContextWithLoadedProject<Converter602To612>
   {
      private RandomPopulation _population;
      private PopulationSimulation _simulation;
      private RandomPopulationSettings _populationSettings;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("PopulationRange_565");
         _populationSettings = _project.All<RandomPopulation>().First().Settings;
         _population = First<RandomPopulation>();
         _simulation= First<PopulationSimulation>();
      }

      [Observation]
      public void should_have_converted_the_weight_min_and_max()
      {
         validateWeightRange(_population);
         validateWeightRange(_simulation.Population.DowncastTo<RandomPopulation>());
      }

      [Observation]
      public void should_have_converted_the_height_min_and_max()
      {
         validateHeightRange(_population);
         validateHeightRange(_simulation.Population.DowncastTo<RandomPopulation>());
      }

      private void validateWeightRange(RandomPopulation population)
      {
         population.Settings.ParameterRange(CoreConstants.Parameter.MEAN_WEIGHT).MinValueInDisplayUnit.ShouldBeEqualTo(90);
         population.Settings.ParameterRange(CoreConstants.Parameter.MEAN_WEIGHT).MaxValueInDisplayUnit.ShouldBeEqualTo(130);
      }

      private void validateHeightRange(RandomPopulation population)
      {
         population.Settings.ParameterRange(CoreConstants.Parameter.MEAN_HEIGHT).MinValueInDisplayUnit.ShouldBeEqualTo(160);
         population.Settings.ParameterRange(CoreConstants.Parameter.MEAN_HEIGHT).MaxValueInDisplayUnit.ShouldBeEqualTo(170);
      }

      [Observation]
      public void should_have_moved_the_formula_cache_in_the_population_settings_to_xml_to_ensure_proper_deserialization_of_individual()
      {
         validateFormulaParameterInIndividual(_populationSettings.BaseIndividual);
         validateFormulaParameterInIndividual(_population.FirstIndividual);
         validateFormulaParameterInIndividual(_simulation.Individual);
      }

      private void validateFormulaParameterInIndividual(Individual individual)
      {
         foreach (var parameter in individual.Organism.AllParameters())
         {
            double.IsNaN(parameter.Value).ShouldBeFalse();
         }
      }
   }
}