using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using ModelClassification = OSPSuite.Core.Domain.Classification;
using SnapshotClassification = PKSim.Core.Snapshots.Classification;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ClassificationMapper : SnapshotMapperBase<ModelClassification, SnapshotClassification, ClassificationType, ClassificationContext>
   {
      public override Task<ModelClassification> MapToModel(SnapshotClassification snapshot, ClassificationType classificationType)
      {
         var classification = new ModelClassification
         {
            ClassificationType = classificationType,
            Name = snapshot.Name
         };

         return Task.FromResult(classification);
      }

      public override Task<SnapshotClassification> MapToSnapshot(ModelClassification model, ClassificationContext context)
      {
         return mapTreeFrom(model, context);
      }

      private async Task<SnapshotClassification> mapTreeFrom(ModelClassification classification, ClassificationContext context)
      {
         var root = await SnapshotFrom(classification, x =>
         {
            x.Name = classification.Name;
         });

         var childClassifications = childClassificationsFrom(classification, context.Classifications);
         var childClassifiables = childClassifiablesFrom(classification, context.Classifiables);

         root.Classifications = childClassifications.Any() ? await Task.WhenAll(childClassifications.Select(x => mapTreeFrom(x, context))) : null;
         root.Classifiables = childClassifiables.Any() ? childClassifiables.ToArray() : null;

         return root;
      }

      private IReadOnlyList<string> childClassifiablesFrom(ModelClassification classification, IReadOnlyList<IClassifiableWrapper> classifiables)
      {
         return childrenFrom(classification, classifiables).Select(x => x.Name).ToList();
      }

      private static IReadOnlyList<ModelClassification> childClassificationsFrom(ModelClassification classification, IReadOnlyList<ModelClassification> classifications)
      {
         return childrenFrom(classification, classifications).ToList();
      }

      private static IEnumerable<T> childrenFrom<T>(ModelClassification classification, IReadOnlyList<T> classifications)  where T : IClassifiable
      {
         return classifications.Where(x => Equals(x.Parent, classification));
      }
   }
}