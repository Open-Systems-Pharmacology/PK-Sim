using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_DistributedParameterToTableParameterConverter<TSimulation> : ContextForSimulationIntegration<IDistributedParameterToTableParameterConverter, TSimulation> where TSimulation : Simulation
   {
      protected Protocol _iv;
      protected Compound _compound;
      protected Individual _individual;
      protected Population _population;
      protected IEntityPathResolver _entityPathResolver;
      protected ISimulationConfigurationTask _simulationConfigurationTask;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _iv = DomainFactoryForSpecs.CreateStandardIVProtocol();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Population.PRETERM);
         _population = DomainFactoryForSpecs.CreateDefaultPopulation(_individual);
         _entityPathResolver = IoC.Resolve<IEntityPathResolver>();
         _simulationConfigurationTask = IoC.Resolve<ISimulationConfigurationTask>();
      }
   }

   public class When_constructing_a_preterm_population_simulation_aging : concern_for_DistributedParameterToTableParameterConverter<PopulationSimulation>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_population, _compound, _iv, allowAging: true).DowncastTo<PopulationSimulation>();
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_have_creating_the_expected_aging_data()
      {
         _simulation.AgingData.AllParameterData.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void should_have_replaced_the_plasma_protein_ontogeny_factor_with_a_table_formula()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         foreach (var parameterName in CoreConstants.Parameters.AllPlasmaProteinOntogenyFactors)
         {
            organism.Parameter(parameterName).Formula.ShouldBeAnInstanceOf<TableFormula>();
            organism.Parameter(parameterName).IsFixedValue.ShouldBeFalse();
         }
      }

      [Observation]
      public void should_have_named_the_X_name_in_the_table_formula_to_simulation_time()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         foreach (var parameterName in CoreConstants.Parameters.AllPlasmaProteinOntogenyFactors)
         {
            var tableFormula = organism.Parameter(parameterName).Formula.DowncastTo<TableFormula>();
            tableFormula.XName.ShouldBeEqualTo(PKSimConstants.UI.SimulationTime);
         }
      }

      [Observation]
      public void should_have_changed_the_age_formula_to_an_explicit_formula_using_the_simulation_time()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         organism.Parameter(CoreConstants.Parameters.AGE).Formula.IsExplicit().ShouldBeTrue();
         organism.Parameter(CoreConstants.Parameters.AGE).IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_have_added_the_age_0_parameter_and_the_min_to_year_conversion_factor_to_the_organism()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         organism.Parameter(CoreConstants.Parameters.AGE_0).ShouldNotBeNull();
         organism.Parameter(CoreConstants.Parameters.MIN_TO_YEAR_FACTOR).ShouldNotBeNull();
      }
   }

   public class When_constructing_an_adult_population_simulation_aging : concern_for_DistributedParameterToTableParameterConverter<IndividualSimulation>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _iv).DowncastTo<IndividualSimulation>();
         _simulation.AllowAging = true;
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         
      }

      [Observation]
      public void should_be_able_to_run_the_simulation()
      {
         var simulationRunner = IoC.Resolve<ISimulationRunner>();
         //this config may lead to negative values. We want to check here that the simulation can run without errors
         simulationRunner.RunSimulation(_simulation, new SimulationRunOptions{CheckForNegativeValues = false}).Wait();
         _simulation.HasResults.ShouldBeTrue();
      }

      [Observation]
      public void should_not_have_replaced_the_plasma_protein_ontogeny_factor_with_a_table_formula()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         foreach (var parameterName in CoreConstants.Parameters.AllPlasmaProteinOntogenyFactors)
         {
            if (parameterName.Equals(CoreConstants.Parameters.ONTOGENY_FACTOR_AGP))
               continue; //new ontogeny is defined up to 90 years, so the table will not be replaced by const


            //Parameter are by default table formula with X Arguments 
            organism.Parameter(parameterName).Formula.ShouldBeAnInstanceOf<TableFormulaWithXArgument>();
         }
      }
   }
}