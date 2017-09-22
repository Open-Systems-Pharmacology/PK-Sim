using System.Threading.Tasks;
using ModelClassifiable = OSPSuite.Core.Domain.ClassifiableObservedData;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObservedDataClassifiableMappper : SnapshotMapperBase<ModelClassifiable, ObservedDataClassifiable>
   {
      private readonly ObservedDataMapper _observedDataMapper;

      public ObservedDataClassifiableMappper(ObservedDataMapper observedDataMapper)
      {
         _observedDataMapper = observedDataMapper;
      }
      public override async Task<ObservedDataClassifiable> MapToSnapshot(ModelClassifiable classifiable)
      {
         var snapshot = await SnapshotFrom(classifiable, x =>
         {
            x.Name = classifiable.Name;
         });

         snapshot.Repository = await _observedDataMapper.MapToSnapshot(classifiable.Repository);
         return snapshot;
      }

      public override Task<ModelClassifiable> MapToModel(ObservedDataClassifiable snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}