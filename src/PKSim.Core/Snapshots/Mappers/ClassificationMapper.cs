using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelClassification = OSPSuite.Core.Domain.Classification;
using SnapshotClassification = PKSim.Core.Snapshots.Classification;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ClassificationMapper : SnapshotMapperBase<ModelClassification, SnapshotClassification, SnapshotClassificationContext, ClassificationContext>
   {
      public override async Task<ModelClassification> MapToModel(SnapshotClassification snapshot, SnapshotClassificationContext context)
      {
         var classification = new ModelClassification
         {
            Name = snapshot.Name,
            ClassificationType = snapshot.ClassificationType
         };

         if (context.ContainsParentFor(snapshot))
         {
            var parent = await MapToModel(context.ParentFor(snapshot), context);
            classification.Parent = parent;
         }

         return classification;
      }

      public override Task<SnapshotClassification> MapToSnapshot(ModelClassification model, ClassificationContext context)
      {
         return mapTreeFrom(model, context.Classifications);
      }

      private async Task<SnapshotClassification> mapTreeFrom(ModelClassification classification, IReadOnlyList<ModelClassification> classifications)
      {
         var root = await SnapshotFrom(classification, x =>
         {
            x.Name = classification.Name;
         });

         var childClassifications = childClassificationsFrom(classification, classifications);

         root.Classifications = childClassifications.Any() ? await Task.WhenAll(childClassifications.Select(x => mapTreeFrom(x, classifications))) : null;

         root.ClassificationType = classification.ClassificationType;

         return root;
      }

      private static IReadOnlyList<ModelClassification> childClassificationsFrom(ModelClassification classification, IReadOnlyList<ModelClassification> classifications)
      {
         return classifications.Where(x => Equals(x.Parent, classification)).ToList();
      }
   }
}