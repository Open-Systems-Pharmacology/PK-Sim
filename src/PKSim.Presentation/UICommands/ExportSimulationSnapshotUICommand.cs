using System.Threading.Tasks;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.UICommands;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Presentation.UICommands
{
   public class ExportSimulationSnapshotUICommand : ObjectUICommand<Simulation>
   {
      private readonly SimulationMapper _simulationMapper;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IExecutionContext _context;
      private readonly ISnapshotTask _snapshotTask;

      public ExportSimulationSnapshotUICommand(SimulationMapper simulationMapper, IPKSimProjectRetriever projectRetriever, 
         IExecutionContext context, ISnapshotTask snapshotTask)
      {
         _simulationMapper = simulationMapper;
         _projectRetriever = projectRetriever;
         _context = context;
         _snapshotTask = snapshotTask;
      }

      protected override async void PerformExecute()
      {
         await this.SecureAwait(x => x.exportSimulationToSnapshot());
      }

      private async Task exportSimulationToSnapshot()
      {
         _context.Load(Subject);

         var snapshotObject = await _simulationMapper.MapToSnapshot(Subject, _projectRetriever.Current);

         await _snapshotTask.ExportSnapshot(snapshotObject);
      }
   }
}