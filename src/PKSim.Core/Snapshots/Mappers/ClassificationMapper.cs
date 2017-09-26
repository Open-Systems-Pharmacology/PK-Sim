using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
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

      public override Task<SnapshotClassification> MapToSnapshot(ModelClassification model, ClassificationContext context)
      {
         return mapTreeFrom(model, context.Classifications, context.Classifiables);
      }

      private async Task<SnapshotClassification> mapClassificationToSnapshot(ModelClassification model)
      {
         return await SnapshotFrom(model, x =>
         {
            x.Name = model.Name;
         });
      }

      private async Task<SnapshotClassification> mapTreeFrom(ModelClassification classification, IReadOnlyList<ModelClassification> classifications, IReadOnlyList<IClassifiableWrapper> classifiables)
      {
         var root = await mapClassificationToSnapshot(classification);

         var childClassifications = childClassificationsFrom(classification, classifications);
         root.Classifications = await mapChildClassifications(classifications, classifiables, childClassifications);

         root.Classifiables = childClassifiablesFrom(classification, classifiables).ToArray();

         return root;
      }

      private IReadOnlyList<string> childClassifiablesFrom(ModelClassification classification, IReadOnlyList<IClassifiableWrapper> classifiables)
      {
         return classifiables.Where(x => Equals(x.Parent, classification)).AllNames();
      }

      private async Task<SnapshotClassification[]> mapChildClassifications(IReadOnlyList<ModelClassification> classifications, IReadOnlyList<IClassifiableWrapper> classifiables, IReadOnlyList<ModelClassification> childClassifications)
      {
         if (childClassifications.Any())
         {
            var tasks = childClassifications.Select(x => mapTreeFrom(x, classifications, classifiables));
            return await Task.WhenAll(tasks);
         }
         return null;
      }

      private static IReadOnlyList<ModelClassification> childClassificationsFrom(ModelClassification classification, IReadOnlyList<ModelClassification> classifications)
      {
         return classifications.Where(x => Equals(x.Parent, classification)).ToList();
      }
   }
}