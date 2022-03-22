using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Views.Snapshots;

namespace PKSim.Presentation.Presenters.Snapshots
{
   public interface ILoadSimulationFromSnapshotPresenter : ILoadFromSnapshotPresenter<Simulation>
   {
      /// <summary>
      ///    Simulation loaded from selected snapshot file. It is null if the user cancels the action or if the file was not
      ///    loaded properly
      /// </summary>
      Simulation LoadSimulation();
   }

   public class LoadSimulationFromSnapshotPresenter : LoadFromSnapshotPresenter<Simulation>, ILoadSimulationFromSnapshotPresenter
   {
      private readonly SimulationMapper _simulationMapper;
      private readonly IPKSimProjectRetriever _projectRetriever;

      public LoadSimulationFromSnapshotPresenter(
         ILoadFromSnapshotView view,
         ILogPresenter logPresenter,
         ISnapshotTask snapshotTask,
         IDialogCreator dialogCreator,
         IObjectTypeResolver objectTypeResolver,
         IOSPSuiteLogger logger,
         IEventPublisher eventPublisher,
         SimulationMapper simulationMapper,
         IPKSimProjectRetriever projectRetriever
      ) : base(view, logPresenter, snapshotTask, dialogCreator, objectTypeResolver, logger, eventPublisher)
      {
         _simulationMapper = simulationMapper;
         _projectRetriever = projectRetriever;
      }

      public Simulation LoadSimulation()
      {
         var models = LoadModelFromSnapshot();
         return models?.FirstOrDefault();
      }

      protected override async Task<IEnumerable<Simulation>> LoadModelAsync(string snapshotFile)
      {
         var snapshots = await _snapshotTask.LoadSnapshotsAsync<PKSim.Core.Snapshots.Simulation>(snapshotFile);
         var simulationContext = new SimulationContext(_projectRetriever.Current, run: true);
         var tasks = snapshots.Select(x => _simulationMapper.MapToModel(x, simulationContext));
         return await Task.WhenAll(tasks);
      }
   }
}