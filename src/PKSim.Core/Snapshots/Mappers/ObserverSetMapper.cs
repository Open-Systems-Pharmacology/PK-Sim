using System.Threading.Tasks;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotObserverSet = PKSim.Core.Snapshots.ObserverSet;
using ModelObserverSet = PKSim.Core.Model.ObserverSet;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObserverSetMapper : ObjectBaseSnapshotMapperBase<ModelObserverSet, SnapshotObserverSet>
   {
      private readonly ObserverMapper _observerMapper;
      private readonly IObserverSetFactory _observerSetFactory;

      public ObserverSetMapper(ObserverMapper observerMapper, IObserverSetFactory observerSetFactory)
      {
         _observerMapper = observerMapper;
         _observerSetFactory = observerSetFactory;
      }

      public override async Task<SnapshotObserverSet> MapToSnapshot(ModelObserverSet observerSet)
      {
         var snapshot = await SnapshotFrom(observerSet);
         snapshot.Observers = await _observerMapper.MapToSnapshots(observerSet.Observers);
         return snapshot;
      }

      public override async Task<ModelObserverSet> MapToModel(SnapshotObserverSet snapshot)
      {
         var observerSet = _observerSetFactory.Create();
         MapSnapshotPropertiesToModel(snapshot, observerSet);
         var observers = await _observerMapper.MapToModels(snapshot.Observers);
         observers?.Each(x => observerSet.Add(x));
         return observerSet;
      }
   }
}