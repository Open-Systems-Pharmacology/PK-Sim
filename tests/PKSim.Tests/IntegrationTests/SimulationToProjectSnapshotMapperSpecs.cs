using System;
using System.Text;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Infrastructure;
using Compound = PKSim.Core.Model.Compound;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using Individual = PKSim.Core.Model.Individual;
using Project = PKSim.Core.Snapshots.Project;
using Protocol = PKSim.Core.Model.Protocol;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationToProjectSnapshotMapper : ContextForSimulationIntegration<ISimulationToProjectSnapshotMapper, IndividualSimulation>
   {
      protected Compound _compound;
      protected Individual _individual;
      protected Protocol _protocol;
      protected ICoreWorkspace _workspace;
      protected ISnapshotMapper _snapshotMapper;
      protected IJsonSerializer _jsonSerializer;
      protected DataRepository _observedData;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _snapshotMapper = IoC.Resolve<ISnapshotMapper>();
         _jsonSerializer = IoC.Resolve<IJsonSerializer>();
         _workspace = IoC.Resolve<ICoreWorkspace>();
         _workspace.Project = new PKSimProject();

         _observedData = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("S").WithName("obs data");

         _workspace.Project.AddBuildingBlock(_compound);
         _workspace.Project.AddBuildingBlock(_individual);
         _workspace.Project.AddBuildingBlock(_protocol);
         _workspace.Project.AddObservedData(_observedData);
      }
   }

   public class When_mapping_simulation_to_project_snapshot_string : concern_for_SimulationToProjectSnapshotMapper
   {
      private PKSimProject _deserializedProject;

      protected override void Context()
      {
         base.Context();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
         _simulation.AddUsedObservedData(UsedObservedData.From(_observedData));
         _workspace.Project.AddBuildingBlock(_simulation);
      }

      protected override void Because()
      {
         var project = _jsonSerializer.DeserializeFromString<Project>(Encoding.UTF8.GetString(Convert.FromBase64String(sut.MapFrom(_simulation)))).Result;
         _deserializedProject = _snapshotMapper.MapToModel(project, new ProjectContext(runSimulations: false)).Result as PKSimProject;
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
   }
}