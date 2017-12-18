using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v7_2;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v7_2
{
   public class When_converting_the_humand_and_rat_720_from_710_project : ContextWithLoadedProject<Converter720To721>
   {
      private List<Individual> _allIndividuals;
      private List<Simulation> _allSImulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("HumanAndRat_7.2.0_from_7.1.0");

         _allIndividuals = All<Individual>();
         _allSImulations = All<Simulation>();
         _allIndividuals.Each(Load);
         _allSImulations.Each(Load);
      }

      [Observation]
      public void should_be_able_to_load_the_project()
      {
         _project.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_the_dynamic_formula_calculation_method_to_all_individuals()
      {
         foreach (var individual in _allIndividuals)
         {
            individual.OriginData.CalculationMethodFor(ConverterConstants.Category.DynamicFormulas).Name.ShouldBeEqualTo(ConverterConstants.CalculationMethod.DynamicSumFormulas);
         }
      }

      [Observation]
      public void should_have_added_the_dynamic_formula_calculation_method_to_all_simulation_individuals()
      {
         foreach (var simulation in _allSImulations.OfType<IndividualSimulation>())
         {
            simulation.Individual.OriginData.CalculationMethodFor(ConverterConstants.Category.DynamicFormulas).Name.ShouldBeEqualTo(ConverterConstants.CalculationMethod.DynamicSumFormulas);
         }
      }
   }
}