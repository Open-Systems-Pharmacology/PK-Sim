using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v12;
using PKSim.IntegrationTests;
using static PKSim.Infrastructure.ProjectConverter.ConverterConstants.CalculationMethod;

namespace PKSim.ProjectConverter.v12
{
   public class When_converting_the_simple_project_730_project_to_12 : ContextWithLoadedProject<Converter11To12>
   {
      private List<PopulationSimulation> _allSimulations;
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");
         _allSimulations = All<PopulationSimulation>();
         _allIndividuals = All<Individual>();
         _allSimulations.Each(Load);
         _allIndividuals.Each(Load);
      }

      [Observation]
      public void all_simulation_should_have_settings()
      {
         _allSimulations.Each(simulation => simulation.Settings.ShouldNotBeNull());
      }

      [Observation]
      public void should_have_added_the_PMA_parameters_to_all_individuals()
      {
         _allIndividuals.Each(validateIndividual);
      }

      [Observation]
      public void should_have_added_the_PMA_parameters_to_all_simulations()
      {
         _allSimulations.Select(x => x.Individual).Each(validateIndividual);
      }

      private void validateIndividual(Individual individual)
      {
         individual.Organism.Parameter(CoreConstants.Parameters.PMA).ShouldNotBeNull();
         individual.OriginData.CalculationMethodCache.Contains(Individual_HeightDependent).ShouldBeTrue();
         individual.OriginData.CalculationMethodCache.Contains(Individual_AgeDependent).ShouldBeTrue();
      }
   }
}