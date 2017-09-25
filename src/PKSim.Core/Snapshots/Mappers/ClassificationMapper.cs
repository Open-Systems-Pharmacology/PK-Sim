using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelClassification = OSPSuite.Core.Domain.Classification;
using SnapshotClassification = PKSim.Core.Snapshots.Classification;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ClassificationMapper : SnapshotMapperBase<ModelClassification, SnapshotClassification, object, ClassificationContext> 
   {
      public override Task<ModelClassification> MapToModel(SnapshotClassification snapshot, object context)
      {
         throw new NotImplementedException();
      }

      protected async Task<SnapshotClassification> MapClassificationToSnapshot(ModelClassification model)
      {
         if (model == null)
            return null;

         var snapshot = await SnapshotFrom(model, x =>
         {
            x.Name = model.Name;
         });
         return snapshot;
      }

      public override async Task<SnapshotClassification> MapToSnapshot(ModelClassification model, ClassificationContext context)
      {
         return await MapTreeFrom(model, context.Classifications, context.Classifiables);
      }

      protected async Task<SnapshotClassification> MapTreeFrom(ModelClassification classification, IReadOnlyList<ModelClassification> classifications, IReadOnlyList<ClassifiableContext> classifiables)
      {
         var root = await MapClassificationToSnapshot(classification);

         var childClassifications = childClassificationsFrom(classification, classifications);
         await mapChildClassifications(classifications, classifiables, childClassifications, root);

         root.Classifiables = childClassifiablesFrom(classification, classifiables).ToArray();

         return root;
      }

      private IReadOnlyList<string> childClassifiablesFrom(ModelClassification classification, IReadOnlyList<ClassifiableContext> classifiables)
      {
         return classifiables.Where(x => Equals(x.Parent, classification)).Select(x => x.Name).ToList();
      }

      private async Task mapChildClassifications(IReadOnlyList<ModelClassification> classifications, IReadOnlyList<ClassifiableContext> classifiables, IReadOnlyList<ModelClassification> childClassifications, Classification root)
      {
         if (childClassifications.Any())
         {
            var tasks = childClassifications.Select(x => MapTreeFrom(x, classifications, classifiables));
            root.Classifications = await Task.WhenAll(tasks);
         }
      }

      private static IReadOnlyList<ModelClassification> childClassificationsFrom(ModelClassification classification, IReadOnlyList<ModelClassification> classifications)
      {
         return classifications.Where(x => Equals(x.Parent, classification)).ToList();
      }
   }
}