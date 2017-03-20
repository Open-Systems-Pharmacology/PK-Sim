using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_5;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Extensions;

namespace PKSim.ProjectConverter.v5_5
{
   public class When_converting_the_SurfaceAreaVariability_551_project : ContextWithLoadedProject<Converter551To552>
   {
      private Individual _individual;
      private Population _population;
      private IndividualSimulation _individualSimulation;
      private PopulationSimulation _populationSimulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SurfaceAreaVariability_551");
         _individual = First<Individual>();
         _population = First<Population>();
         _individualSimulation = First<IndividualSimulation>();
         _populationSimulation = First<PopulationSimulation>();
      }

      [Observation]
      public void should_have_added_the_surface_area_variability_factor_to_the_individual()
      {
         validate(_individual);
         validate(_population.FirstIndividual);
         validate(_individualSimulation.BuildingBlock<Individual>());
         validate(_populationSimulation.BuildingBlock<Individual>());
      }

      [Observation]
      public void should_have_added_variability_to_the_populations()
      {
         validate(_population);
         validate(_populationSimulation.BuildingBlock<Population>());
      }


      [Observation]
      public void should_have_updated_SA_int_cell_in_all_segments_neighborhood()
      {
         validateSA_int_cell(_individual);
      }

      private void validate(Population population)
      {
         var allValues = population.AllValuesFor(new[] {Constants.ORGANISM, CoreConstants.Organ.Lumen, ConverterConstants.Parameter.EffectiveSurfaceAreaVariabilityFactor}.ToPathString());
         allValues.Contains(double.NaN).ShouldBeFalse();
      }

      private void validate(Individual individual)
      {
         var factor = individual.EntityAt<IDistributedParameter>(Constants.ORGANISM, CoreConstants.Organ.Lumen, ConverterConstants.Parameter.EffectiveSurfaceAreaVariabilityFactor);
         factor.ShouldNotBeNull();
      }

      private void validateSA_int_cell(Individual individual)
      {
         var neighborhood = individual.Neighborhoods.GetSingleChildByName<IContainer>("Duodenum_int_Duodenum_cell");
         var explicitFormula = neighborhood.Parameter(ConverterConstants.Parameter.SA_int_cell).Formula.DowncastTo<ExplicitFormula>();
         explicitFormula.FormulaString.ShouldBeEqualTo("Aeff / MicrovilliFactor");
      }
   }
}