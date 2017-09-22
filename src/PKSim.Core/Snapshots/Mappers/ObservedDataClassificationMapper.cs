using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using ModelClassification = OSPSuite.Core.Domain.Classification;
using SnapshotClassification = PKSim.Core.Snapshots.ObservedDataClassification;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObservedDataClassificationMapper : BaseClassificationMapper<SnapshotClassification, object, ObservedDataClassificationContext, ClassifiableObservedData, ObservedDataClassifiable>
   {
      private readonly ObservedDataClassifiableMappper _observedDataClassifiableMappper;

      public ObservedDataClassificationMapper(ObservedDataClassifiableMappper observedDataClassifiableMappper)
      {
         _observedDataClassifiableMappper = observedDataClassifiableMappper;
      }

      protected override Task<ObservedDataClassifiable> MapClassifiableToSnapshot(ClassifiableObservedData classifiable)
      {
         return _observedDataClassifiableMappper.MapToSnapshot(classifiable);
      }

      public override Task<ModelClassification> MapToModel(SnapshotClassification snapshot, object context)
      {
         throw new NotImplementedException();
      }
   }
}