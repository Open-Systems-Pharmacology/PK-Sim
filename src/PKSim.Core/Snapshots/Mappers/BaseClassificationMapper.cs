using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots.Mappers
{
   public abstract class BaseClassificationMapper<TSnapshot, TModelContext, TSnapshotContext, TClassifiable, TSnapshotClassifiable> : SnapshotMapperBase<Classification, TSnapshot, TModelContext, TSnapshotContext> 
      where TSnapshot : ClassificationSnapshotBase<TSnapshot, TSnapshotClassifiable>, new()
      where TClassifiable : IClassifiable
      where TSnapshotContext : BaseClassificationContext<Classification, TClassifiable>
   {
      protected abstract Task<TSnapshotClassifiable> MapClassifiableToSnapshot(TClassifiable classifiable);

      protected async Task<TSnapshot> MapClassificationToSnapshot(Classification model)
      {
         if (model == null)
            return null;

         var snapshot = await SnapshotFrom(model, x =>
         {
            x.Name = model.Name;
            x.ClassificationType = model.ClassificationType.ToString();
         });
         return snapshot;
      }

      public override async Task<TSnapshot> MapToSnapshot(Classification model, TSnapshotContext context)
      {
         return await MapTreeFrom(model, context.Classifications, context.Classifiables);
      }

      protected async Task<TSnapshot> MapTreeFrom(Classification classification, IReadOnlyList<Classification> classifications, IReadOnlyList<TClassifiable> classifiables)
      {
         var root = await MapClassificationToSnapshot(classification);

         var childClassifications = childrenFrom(classification, classifications);
         await mapChildClassifications(classifications, classifiables, childClassifications, root);

         var childClassifiables = childrenFrom(classification, classifiables);
         await mapChildClassifiables(childClassifiables, root);

         return root;
      }

      private async Task mapChildClassifiables(IReadOnlyList<TClassifiable> childClassifiables, TSnapshot root)
      {
         if (childClassifiables.Any())
         {
            var tasks = childClassifiables.Select(MapClassifiableToSnapshot);
            root.Classifiables = await Task.WhenAll(tasks);
         }
      }

      private async Task mapChildClassifications(IReadOnlyList<Classification> classifications, IReadOnlyList<TClassifiable> classifiables, IReadOnlyList<Classification> childClassifications, TSnapshot root)
      {
         if (childClassifications.Any())
         {
            var tasks = childClassifications.Select(x => MapTreeFrom(x, classifications, classifiables));
            root.Classifications = await Task.WhenAll(tasks);
         }
      }

      private static IReadOnlyList<T> childrenFrom<T>(Classification classification, IReadOnlyList<T> classifications) where T: IClassifiable
      {
         return classifications.Where(x => Equals(x.Parent, classification)).ToList();
      }
   }
}