using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Infrastructure;
using SnapshotSimulation = PKSim.Core.Snapshots.Simulation;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationMapper_with_v12_application_paths : ContextForSimulationIntegration<SimulationMapper>
   {
      protected Compound _compound;
      protected Individual _individual;
      protected Protocol _protocol;
      protected PKSimProject _project;
      protected SnapshotSimulation _snapshot;
      protected IndividualSimulation _mappedSimulation;

      protected static string ApplicationName1 => $"{CoreConstants.APPLICATION_NAME_TEMPLATE}1";

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         var workspace = IoC.Resolve<ICoreWorkspace>();
         workspace.Project = new PKSimProject();
         _project = workspace.Project;
         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_individual);
         CreateSimulation();
         _project.AddBuildingBlock(_simulation);
      }

      protected abstract void CreateSimulation();

      //Maps the simulation to snapshot, replaces the localized parameters with the given ones (simulating a snapshot
      //saved in the V12 format) and maps the snapshot back to a simulation using the V12 snapshot version
      protected void MapSimulationToModelAsV12Snapshot(params LocalizedParameter[] snapshotParameters)
      {
         _snapshot = sut.MapToSnapshot(_simulation, _project).Result;
         _snapshot.Parameters = snapshotParameters;
         var simulationContext = new SimulationContext(run: false, new SnapshotContext(_project, SnapshotVersions.V12));
         _mappedSimulation = sut.MapToModel(_snapshot, simulationContext).Result as IndividualSimulation;
      }
   }

   public class When_loading_a_v12_snapshot_simulation_with_an_application_parameter_path_defined_without_formulation : concern_for_SimulationMapper_with_v12_application_paths
   {
      private IParameter _infusionTimeParameter;
      private IParameter _originalInfusionTimeParameter;

      protected override void CreateSimulation()
      {
         _protocol = DomainFactoryForSpecs.CreateStandardIVProtocol();
         _project.AddBuildingBlock(_protocol);
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
      }

      protected override void Because()
      {
         //Pre-v13 snapshots do not have the 'No formulation' container between the protocol and the application
         var v12ParameterPath = new ObjectPath(
            Constants.EVENTS,
            _protocol.Name,
            ApplicationName1,
            CoreConstants.ContainerName.ProtocolSchemaItem,
            Constants.Parameters.INFUSION_TIME).PathAsString;

         MapSimulationToModelAsV12Snapshot(new LocalizedParameter {Path = v12ParameterPath, Value = 42, Unit = "min"});

         _originalInfusionTimeParameter = _simulation.Model.Root.EntityAt<IParameter>(
            Constants.EVENTS,
            _protocol.Name,
            CoreConstants.ContainerName.NoFormulation,
            ApplicationName1,
            CoreConstants.ContainerName.ProtocolSchemaItem,
            Constants.Parameters.INFUSION_TIME);

         _infusionTimeParameter = _mappedSimulation.Model.Root.EntityAt<IParameter>(
            Constants.EVENTS,
            _protocol.Name,
            CoreConstants.ContainerName.NoFormulation,
            ApplicationName1,
            CoreConstants.ContainerName.ProtocolSchemaItem,
            Constants.Parameters.INFUSION_TIME);
      }

      [Observation]
      public void the_rebuilt_simulation_should_nest_the_application_under_the_no_formulation_container() => _infusionTimeParameter.ShouldNotBeNull();

      [Observation]
      public void should_apply_the_snapshot_value_to_the_parameter_nested_under_the_no_formulation_container() => _infusionTimeParameter.Value.ShouldBeEqualTo(42);

      [Observation]
      public void should_override_the_value_coming_from_the_protocol_building_block() => _infusionTimeParameter.Value.ShouldNotBeEqualTo(_originalInfusionTimeParameter.Value);
   }

   public class When_loading_a_v12_snapshot_simulation_with_an_application_parameter_path_defined_under_a_formulation : concern_for_SimulationMapper_with_v12_application_paths
   {
      private Formulation _formulation;
      private IParameter _startTimeParameter;

      protected override void CreateSimulation()
      {
         _protocol = DomainFactoryForSpecs.CreateStandardOralProtocol();
         _formulation = IoC.Resolve<IFormulationRepository>().FormulationBy(CoreConstants.Formulation.DISSOLVED);
         _project.AddBuildingBlock(_protocol);
         _project.AddBuildingBlock(_formulation);
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol, _formulation) as IndividualSimulation;
      }

      protected override void Because()
      {
         //Formulation-bearing applications were already nested under the formulation container before v13
         //and their paths are identical in v12 and v13 snapshots
         var v12ParameterPath = new ObjectPath(
            Constants.EVENTS,
            _protocol.Name,
            _formulation.Name,
            ApplicationName1,
            CoreConstants.ContainerName.ProtocolSchemaItem,
            Constants.Parameters.START_TIME).PathAsString;

         MapSimulationToModelAsV12Snapshot(new LocalizedParameter {Path = v12ParameterPath, Value = 30, Unit = "min"});

         _startTimeParameter = _mappedSimulation.Model.Root.EntityAt<IParameter>(
            Constants.EVENTS,
            _protocol.Name,
            _formulation.Name,
            ApplicationName1,
            CoreConstants.ContainerName.ProtocolSchemaItem,
            Constants.Parameters.START_TIME);
      }

      [Observation]
      public void the_rebuilt_simulation_should_nest_the_application_under_the_formulation_container() => _startTimeParameter.ShouldNotBeNull();

      [Observation]
      public void should_apply_the_snapshot_value_to_the_parameter_nested_under_the_formulation_container() => _startTimeParameter.Value.ShouldBeEqualTo(30);
   }
}
