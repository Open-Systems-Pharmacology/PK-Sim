using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using System;
using System.Linq;
using System.Text;

using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Utility.Extensions;
using Compound = PKSim.Core.Model.Compound;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using Individual = PKSim.Core.Model.Individual;
using Project = PKSim.Core.Snapshots.Project;
using Protocol = PKSim.Core.Model.Protocol;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ModelCoreSimulationSnapshotUpdater : ContextForSimulationIntegration<IModelCoreSimulationSnapshotUpdater, IndividualSimulation>
   {
      protected Compound _compound;
      protected Individual _individual;
      protected Protocol _protocol;
      protected ICoreWorkspace _workspace;
      protected ISnapshotMapper _snapshotMapper;
      protected IJsonSerializer _jsonSerializer;
      protected DataRepository _observedData;
      protected ISimulationConfigurationTask _simulationConfigurationTask;
      protected ISimulationToModelCoreSimulationMapper _simulationMapper;
      private ExpressionProfile _expressionProfile;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(_individual);
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _snapshotMapper = IoC.Resolve<ISnapshotMapper>();
         _jsonSerializer = IoC.Resolve<IJsonSerializer>();
         _workspace = IoC.Resolve<ICoreWorkspace>();
         _workspace.Project = new PKSimProject();
         _simulationConfigurationTask = IoC.Resolve<ISimulationConfigurationTask>();
         _simulationMapper = IoC.Resolve<ISimulationToModelCoreSimulationMapper>();
         
         _observedData = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("S").WithName("obs data");

         _workspace.Project.AddBuildingBlock(_compound);
         _workspace.Project.AddBuildingBlock(_individual);
         _workspace.Project.AddBuildingBlock(_protocol);
         _workspace.Project.AddBuildingBlock(_expressionProfile);
         _workspace.Project.AddObservedData(_observedData);

         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
         _simulation.AddUsedObservedData(UsedObservedData.From(_observedData));
         _workspace.Project.AddBuildingBlock(_simulation);
      }
   }

   public class mapping_model_core_simulation_to_project_snapshot_string : concern_for_ModelCoreSimulationSnapshotUpdater
   {
      private PKSimProject _deserializedProject;
      private IModelCoreSimulation _moBiSimulation;
      protected override void Context()
      {
         base.Context();
         var configuration = _simulationConfigurationTask.CreateFor(_simulation, shouldValidate: true, createAgingDataInSimulation: false);
         _moBiSimulation = _simulationMapper.MapFrom(_simulation, configuration, shouldCloneModel: true);
      }

      protected override void Because()
      {
         sut.AddSnapshotsToModelCoreSimulation(_simulation, _moBiSimulation);
         var project = _jsonSerializer.DeserializeFromString<Project>(Encoding.UTF8.GetString(Convert.FromBase64String(_moBiSimulation.Configuration.ModuleConfigurations.Single().Module.Snapshot))).Result;
         _deserializedProject = _snapshotMapper.MapToModel(project, new ProjectContext(new PKSimProject(), runSimulations: false)).Result as PKSimProject;
      }

      [Observation]
      public void snapshot_should_contain_building_blocks_observed_data_and_simulation()
      {
         _deserializedProject.All<Compound>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<Protocol>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<Individual>().Count.ShouldBeEqualTo(1);
         _deserializedProject.All<IndividualSimulation>().Count.ShouldBeEqualTo(1);
         _deserializedProject.AllObservedData.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void the_snapshots_should_be_set()
      {
         _moBiSimulation.Configuration.Individual.Snapshot.ShouldNotBeEmpty();
         _moBiSimulation.Configuration.ModuleConfigurations.Single().Module.Snapshot.ShouldNotBeEmpty();
         _moBiSimulation.Configuration.ExpressionProfiles.Each(x => x.Snapshot.ShouldNotBeEmpty());
      }
   }
}