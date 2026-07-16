using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Services;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;

namespace PKSim.CLI
{
   public abstract class concern_for_JsonSimulationRunner : ContextSpecificationAsync<JsonSimulationRunner>
   {
      protected JsonRunOptions _runOptions;
      protected ISnapshotTask _snapshotTask;
      protected IOSPSuiteLogger _logger;
      protected ISimulationExporter _simulationExporter;
      private ICoreWorkspace _workspace;

      protected override Task Context()
      {
         _runOptions = new JsonRunOptions
         {
            InputFolder = DomainHelperForSpecs.DataFolder,
            OutputFolder = "c:/tests/temp"
         };
         _snapshotTask = A.Fake<ISnapshotTask>();
         _logger = A.Fake<IOSPSuiteLogger>();
         _workspace = A.Fake<ICoreWorkspace>();
         _simulationExporter = A.Fake<ISimulationExporter>();
         sut = new JsonSimulationRunner(_simulationExporter, _logger, _snapshotTask, _workspace);
         
         var project = A.Fake<PKSimProject>();
         A.CallTo(() => project.All<Simulation>()).Returns(new List<Simulation>());
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshotFileAsync(A<string>._, false)).Returns(project);
         
         return Task.CompletedTask;
      }
   }

   public class When_loading_snapshot_for_json_run : concern_for_JsonSimulationRunner
   {
      protected override Task Because()
      {
         return sut.RunBatchAsync(_runOptions);
      }

      [Observation]
      public void the_snapshot_is_imported_without_running_simulations()
      {
         A.CallTo(() => _snapshotTask.LoadProjectFromSnapshotFileAsync(A<string>._, false)).MustHaveHappened();
      }
   }
}
