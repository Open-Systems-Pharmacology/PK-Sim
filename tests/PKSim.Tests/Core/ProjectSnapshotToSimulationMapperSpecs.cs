using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   internal abstract class concern_for_ProjectSnapshotToSimulationMapper : ContextSpecificationAsync<ProjectSnapshotToSimulationMapper>
   {
      private ISimulationToModelCoreSimulationMapper _simulationMapper;
      private ISimulationConfigurationTask _simulationConfigurationTask;
      protected ISnapshotMapper _snapshotMapper;
      private IJsonSerializer _jsonSerializer;

      protected override async Task Context()
      {
         await base.Context();
         _simulationMapper = A.Fake<ISimulationToModelCoreSimulationMapper>();
         _simulationConfigurationTask = A.Fake<ISimulationConfigurationTask>();
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         _jsonSerializer = A.Fake<IJsonSerializer>();

         sut = new ProjectSnapshotToSimulationMapper(_jsonSerializer, _snapshotMapper, _simulationConfigurationTask, _simulationMapper);
      }
   }

   internal class When_deserializing_a_snapshot_with_too_many_simulations : concern_for_ProjectSnapshotToSimulationMapper
   {
      private string _snapshotString;
      private PKSimProject _pkSimProject;

      protected override async Task Context()
      {
         await base.Context();
         _snapshotString = string.Empty;
         _pkSimProject = new PKSimProject();
         _pkSimProject.AddBuildingBlock(new IndividualSimulation().WithName("1"));
         _pkSimProject.AddBuildingBlock(new IndividualSimulation().WithName("2"));
         A.CallTo(() => _snapshotMapper.MapToModel(A<object>._, A<SnapshotContext>._)).Returns(_pkSimProject);
      }

      [Observation]
      public void the_exception_should_be_thrown()
      {
         The.Action(() => sut.MapFrom(_snapshotString)).ShouldThrowAn<OSPSuiteException>();
      }
   }
}
