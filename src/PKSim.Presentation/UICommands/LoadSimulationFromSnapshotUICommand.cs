using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class LoadSimulationFromSnapshotUICommand : IUICommand
   {
      private readonly ISnapshotTask _snapshotTask;
      private readonly SimulationMapper _simulationMapper;
      private readonly ISimulationTask _simulationTask;
      private readonly IPKSimProjectRetriever _projectRetriever;

      public LoadSimulationFromSnapshotUICommand(ISnapshotTask snapshotTask, SimulationMapper simulationMapper,
         ISimulationTask simulationTask, IPKSimProjectRetriever projectRetriever)
      {
         _snapshotTask = snapshotTask;
         _simulationMapper = simulationMapper;
         _simulationTask = simulationTask;
         _projectRetriever = projectRetriever;
      }

      public async void Execute()
      {
         await this.SecureAwait(x => x.loadSimulationsFromSnapshot());
      }

      private async Task loadSimulationsFromSnapshot()
      {
         var snapshots = await _snapshotTask.LoadSnapshot<Simulation>();
         var tasks = snapshots.Select(x => _simulationMapper.MapToModel(x, _projectRetriever.Current));
         var simulations = await Task.WhenAll(tasks);
         simulations.Each(x => _simulationTask.AddToProject(x));
      }
   }
}