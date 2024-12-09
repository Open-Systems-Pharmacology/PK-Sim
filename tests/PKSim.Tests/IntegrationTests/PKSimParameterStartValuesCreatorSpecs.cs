using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using static PKSim.Core.CoreConstants.Compartment;
using static PKSim.Core.CoreConstants.Organ;

namespace PKSim.IntegrationTests
{
   public class When_creating_a_parameter_start_value_building_block_for_a_simulation_whose_parameter_where_changed_from_default : ContextForSimulationIntegration<IPKSimParameterValuesCreator>
   {
      private ISimulationConfigurationTask _simulationConfigurationTask;
      private IEntityPathResolver _entityPathResolver;
      private ObjectPath _parameterPath;
      private ParameterValuesBuildingBlock _parameterValues;
      private Individual _individual;
      private Compound _compound;
      private Protocol _protocol;
      private IndividualMolecule _enzyme;
      private PartialProcess _metabolizationProcess;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulationConfigurationTask = IoC.Resolve<ISimulationConfigurationTask>();
         _entityPathResolver = IoC.Resolve<IEntityPathResolver>();
         var enzymeFactory = IoC.Resolve<IIndividualEnzymeFactory>();
         var compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();
         var cloneManager = IoC.Resolve<ICloneManager>();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _enzyme = enzymeFactory.AddMoleculeTo(_individual, "CYP").DowncastTo<IndividualEnzyme>();
         _metabolizationProcess = cloneManager.Clone(compoundProcessRepository
            .ProcessByName(CoreConstantsForSpecs.Process.METABOLIZATION_SPECIFIC_FIRST_ORDER).DowncastTo<PartialProcess>());
         _metabolizationProcess.Name = "My Partial Process";
         _metabolizationProcess.Parameter(ConverterConstants.Parameters.CLspec).Value = 15;
         _compound.AddProcess(_metabolizationProcess);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      protected override void Because()
      {
         var compoundName = _simulation.CompoundNames.First();
         var parameter = _simulation.Model.Root.EntityAt<IParameter>(compoundName, CoreConstantsForSpecs.Parameters.BLOOD_PLASMA_CONCENTRATION_RATIO);
         parameter.Value = 10;
         _parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         var simulationConfiguration = _simulationConfigurationTask.CreateFor(_simulation, shouldValidate: true, createAgingDataInSimulation: false);
         _parameterValues = simulationConfiguration.ModuleConfigurations[0].SelectedParameterValues;
      }

      [Observation]
      public void should_have_created_one_entry_for_the_changed_parameter_in_the_parameter_start_value_building_block()
      {
         _parameterValues[_parameterPath].ShouldNotBeNull();
         _parameterValues[_parameterPath].Value.ShouldBeEqualTo(10);
      }

   }

   public class When_creating_a_parameter_start_value_for_an_individual_and_simulation_where_the_initial_concentration_was_changed_in_the_expression_profile : ContextForSimulationIntegration<IPKSimParameterValuesCreator>
   {
      private ISimulationConfigurationTask _simulationConfigurationTask;
      private Individual _individual;
      private Compound _compound;
      private Protocol _protocol;
      private ExpressionProfile _expressionProfileForEnzyme;
      private IParameter _initialConcentrationBrainIntracellular;
      private string[] _parameterPathArray;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulationConfigurationTask = IoC.Resolve<ISimulationConfigurationTask>();
         var moleculeExpressionTask = IoC.Resolve<IMoleculeExpressionTask<Individual>>();
         var expressionProfileUpdater = IoC.Resolve<IExpressionProfileUpdater>();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _expressionProfileForEnzyme = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _parameterPathArray = new[] {BONE, INTRACELLULAR, _expressionProfileForEnzyme.MoleculeName, CoreConstants.Parameters.INITIAL_CONCENTRATION};

         moleculeExpressionTask.AddExpressionProfile(_individual, _expressionProfileForEnzyme);

         _initialConcentrationBrainIntracellular = _expressionProfileForEnzyme.Individual.Organism.EntityAt<IParameter>(_parameterPathArray);
         _initialConcentrationBrainIntracellular.Value = 10;


         expressionProfileUpdater.SynchroniseSimulationSubjectWithExpressionProfile(_individual, _expressionProfileForEnzyme);

         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
      }

      protected override void Because()
      {
         _simulationConfigurationTask.CreateFor(_simulation, shouldValidate: true, createAgingDataInSimulation: false);
      }

      [Observation]
      public void should_have_updated_the_value_in_the_simulation_based_on_the_value_in_expression_profile()
      {
         _simulation.Model.Root.Container(Constants.ORGANISM).EntityAt<IParameter>(_parameterPathArray).Value.ShouldBeEqualTo(10);
      }
   }

   public class When_creating_a_simulation_with_spatial_structure_should_contain_parameter_building_block : ContextForSimulationIntegration<IPKSimParameterValuesCreator>
   {
      private ISimulationConfigurationTask _simulationConfigurationTask;
      private SimulationConfiguration _simulationConfiguration;
      private Individual _individual;
      private Compound _compound;
      private Protocol _protocol;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _simulationConfigurationTask = IoC.Resolve<ISimulationConfigurationTask>();
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      protected override void Because()
      {
         _simulationConfiguration = _simulationConfigurationTask.CreateFor(_simulation, shouldValidate: true, createAgingDataInSimulation: false);
      }

      [Observation]
      public void should_have_created_event_container_withing_spatial_structure_building_block()
      {
         var organism = _simulationConfiguration.ModuleConfigurations[0].Module.BuildingBlocks.FirstOrDefault() as SpatialStructure;
         organism.ShouldNotBeNull();
         organism.TopContainers.FindByName("Events").ShouldNotBeNull();
      }

   }
}