using System;
using System.Linq;
using System.Threading.Tasks;
using SnapshotOutputSelections = PKSim.Core.Snapshots.OutputSelections;
using ModelOutputSelections = OSPSuite.Core.Domain.OutputSelections;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputSelectionsMapper : SnapshotMapperBase<ModelOutputSelections, SnapshotOutputSelections>
   {
      public override async Task<SnapshotOutputSelections> MapToSnapshot(ModelOutputSelections outputSelections)
      {
         var snapshot = await SnapshotFrom(outputSelections);
         snapshot.AddRange(outputSelections.AllOutputs.Select(x => x.Path));
         return snapshot;
      }

      public override Task<ModelOutputSelections> MapToModel(SnapshotOutputSelections snapshot)
      {
         throw new NotImplementedException();
      }
   }
}