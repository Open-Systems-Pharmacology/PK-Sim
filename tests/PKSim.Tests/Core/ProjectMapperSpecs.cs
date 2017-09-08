using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using Project = PKSim.Core.Snapshots.Project;

namespace PKSim.Core
{
   public abstract class concern_for_ProjectMapper : ContextSpecification<ProjectMapper>
   {
      protected PKSimProject _project;
      private Individual _individual;
      private Compound _compound;
      private IndividualSimulation _simulation;
      protected Project _snapshot;
      private ISnapshotMapper _snapshotMapper;
      protected Snapshots.Compound _compoundSnapshot;
      protected Snapshots.Individual _individualSnapshot;
      private IExecutionContext _executionContext;

      protected override void Context()
      {
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         _executionContext = A.Fake<IExecutionContext>();
         sut = new ProjectMapper(_executionContext);
         A.CallTo(() => _executionContext.Resolve<ISnapshotMapper>()).Returns(_snapshotMapper);
         _individual = new Individual().WithName("IND");
         _compound = new Compound().WithName("COMP");
//         _simulation = new IndividualSimulation().WithName("IND_SIM");

         _project = new PKSimProject();
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_compound);
         //         _project.AddBuildingBlock(_simulation);

         _compoundSnapshot = new Snapshots.Compound();
         _individualSnapshot = new Snapshots.Individual();
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_compound)).Returns(_compoundSnapshot);
         A.CallTo(() => _snapshotMapper.MapToSnapshot(_individual)).Returns(_individualSnapshot);
      }
   }

   public class When_exporting_a_project_to_snapshot : concern_for_ProjectMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_project);
      }

      [Observation]
      public void should_retrieve_the_snapshot_for_all_underlying_building_blocks()
      {
         _snapshot.Compounds.ShouldContain(_compoundSnapshot);
         _snapshot.Individuals.ShouldContain(_individualSnapshot);
      }
   }
}