using System.Threading.Tasks;
using SnapshotObserver = PKSim.Core.Snapshots.Observer;
using ModelObserver = OSPSuite.Core.Domain.Builder.IObserverBuilder;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObserverMapper : ObjectBaseSnapshotMapperBase<ModelObserver, SnapshotObserver>
   {
      public override Task<SnapshotObserver> MapToSnapshot(ModelObserver model)
      {
         throw new System.NotImplementedException();
      }

      public override Task<ModelObserver> MapToModel(SnapshotObserver snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}