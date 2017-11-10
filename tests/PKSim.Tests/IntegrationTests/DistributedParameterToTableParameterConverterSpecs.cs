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

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_DistributedParameterToTableParameterConverter<TSimulation> : ContextForSimulationIntegration<IDistributedParameterToTableParameterConverter, TSimulation> where TSimulation : Simulation
   {
      protected Protocol _iv;
      protected Compound _compound;
      protected Individual _individual;
      protected Population _population;
      protected IEntityPathResolver _entityPathResolver;
      protected IBuildConfigurationTask _buildConfigurationTask;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _iv = DomainFactoryForSpecs.CreateStandardIVProtocol();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual(CoreConstants.Population.Preterm);
         _population = DomainFactoryForSpecs.CreateDefaultPopulation(_individual);
         _entityPathResolver = IoC.Resolve<IEntityPathResolver>();
         _buildConfigurationTask = IoC.Resolve<IBuildConfigurationTask>();
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
         foreach (var parameterName in CoreConstants.Parameter.AllPlasmaProteinOntogenyFactors)
         {
            organism.Parameter(parameterName).Formula.ShouldBeAnInstanceOf<TableFormula>();
            organism.Parameter(parameterName).IsFixedValue.ShouldBeFalse();
         }
      }

      [Observation]
      public void should_have_named_the_X_name_in_the_table_formula_to_simulation_time()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         foreach (var parameterName in CoreConstants.Parameter.AllPlasmaProteinOntogenyFactors)
         {
            var tableFormula = organism.Parameter(parameterName).Formula.DowncastTo<TableFormula>();
            tableFormula.XName.ShouldBeEqualTo(PKSimConstants.UI.SimulationTime);
         }
      }

      [Observation]
      public void should_have_changed_the_age_formula_to_an_explicit_formula_using_the_simulation_time()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         organism.Parameter(CoreConstants.Parameter.AGE).Formula.IsExplicit().ShouldBeTrue();
         organism.Parameter(CoreConstants.Parameter.AGE).IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void should_have_added_the_age_0_parameter_and_the_min_to_year_conversion_factor_to_the_organism()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         organism.Parameter(CoreConstants.Parameter.AGE_0).ShouldNotBeNull();
         organism.Parameter(CoreConstants.Parameter.MIN_TO_YEAR_FACTOR).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_the_age_0_parameter_and_the_min_to_year_conversion_factor_to_the_parameter_start_values()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         var age0Path = _entityPathResolver.ObjectPathFor(organism.Parameter(CoreConstants.Parameter.AGE_0));
         var minToYearFactorPath = _entityPathResolver.ObjectPathFor(organism.Parameter(CoreConstants.Parameter.MIN_TO_YEAR_FACTOR));
         var buildConfiguration = _buildConfigurationTask.CreateFor(_simulation, shouldValidate: false, createAgingDataInSimulation: true);
         var psv = buildConfiguration.ParameterStartValues;
         psv[age0Path].ShouldNotBeNull();
         psv[minToYearFactorPath].ShouldNotBeNull();
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
      public void should_not_have_replaced_the_plasma_protein_ontogeny_factor_with_a_table_formula()
      {
         var organism = _simulation.Model.Root.Container(Constants.ORGANISM);
         foreach (var parameterName in CoreConstants.Parameter.AllPlasmaProteinOntogenyFactors)
         {
            organism.Parameter(parameterName).Formula.ShouldBeAnInstanceOf<ConstantFormula>();
         }
      }
   }
}