using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Presenters.Snapshots;
using SnapshotSimulation = PKSim.Core.Snapshots.Simulation;

namespace PKSim.Presentation
{
   public abstract class concern_for_LoadSimulationFromSnapshotPresenter : ContextSpecificationAsync<ILoadSimulationFromSnapshotPresenter>
   {
      protected ILoadFromSnapshotView _view;
      protected ISnapshotTask _snapshotTask;
      protected IDialogCreator _dialogCreator;
      protected ProjectMapper _projectMapper;
      protected IPKSimProjectRetriever _projectRetriever;
      protected LoadFromSnapshotDTO _loadFromSnapshotDTO;
      protected PKSimProject _project;
      protected SnapshotSimulation _simulationSnapshot;
      protected readonly string _snapshotFile = "SnapshotFile";

      protected override Task Context()
      {
         _view = A.Fake<ILoadFromSnapshotView>();
         _snapshotTask = A.Fake<ISnapshotTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _projectMapper = A.Fake<ProjectMapper>();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         _project = new PKSimProject();
         _simulationSnapshot = new SnapshotSimulation();

         A.CallTo(() => _view.BindTo(A<LoadFromSnapshotDTO>._))
            .Invokes(x => _loadFromSnapshotDTO = x.GetArgument<LoadFromSnapshotDTO>(0));

         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_snapshotFile);
         A.CallTo(() => _projectRetriever.Current).Returns(_project);
         A.CallTo(() => _snapshotTask.LoadSnapshotsAsync<SnapshotSimulation>(_snapshotFile)).Returns(new[] { _simulationSnapshot });

         sut = new LoadSimulationFromSnapshotPresenter(_view, A.Fake<ILogPresenter>(), _snapshotTask, _dialogCreator, A.Fake<IObjectTypeResolver>(),
            A.Fake<IOSPSuiteLogger>(), A.Fake<IEventPublisher>(), _projectMapper, _projectRetriever);

         sut.SelectFile();
         return _completed;
      }
   }

   public class When_loading_a_simulation_from_snapshot_that_should_be_run : concern_for_LoadSimulationFromSnapshotPresenter
   {
      private Simulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _simulation = new IndividualSimulation();
         _loadFromSnapshotDTO.RunSimulations = true;
         A.CallTo(() => _projectMapper.MapSimulationsToModels(A<SnapshotSimulation[]>._, A<bool>._, A<SnapshotContext>._)).Returns(new[] { _simulation });
      }

      protected override Task Because()
      {
         return sut.StartAsync();
      }

      [Observation]
      public void should_run_the_simulations_and_map_their_analyses_in_the_context_of_the_current_project()
      {
         A.CallTo(() => _projectMapper.MapSimulationsToModels(
               A<SnapshotSimulation[]>.That.Matches(x => x.SequenceEqual(new[] { _simulationSnapshot })),
               true,
               A<SnapshotContext>.That.Matches(x => x.Project == _project)))
            .MustHaveHappened();
      }

      [Observation]
      public void should_have_loaded_the_simulation()
      {
         sut.ModelIsDefined.ShouldBeTrue();
      }
   }

   public class When_loading_a_simulation_from_snapshot_that_cannot_be_mapped : concern_for_LoadSimulationFromSnapshotPresenter
   {
      protected override async Task Context()
      {
         await base.Context();
         A.CallTo(() => _projectMapper.MapSimulationsToModels(A<SnapshotSimulation[]>._, A<bool>._, A<SnapshotContext>._)).Returns(new List<Simulation>());
      }

      protected override Task Because()
      {
         return sut.StartAsync();
      }

      [Observation]
      public void should_not_define_a_loaded_simulation()
      {
         sut.ModelIsDefined.ShouldBeFalse();
      }
   }
}
