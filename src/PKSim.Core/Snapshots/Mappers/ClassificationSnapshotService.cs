using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using ModelClassification = OSPSuite.Core.Domain.Classification;
using SnapshotClassification = PKSim.Core.Snapshots.Classification;


namespace PKSim.Core.Snapshots.Mappers
{
   public interface IClassificationSnapshotService
   {
      /// <summary>
      /// Returns the Classifiable for the snapshots provided.
      /// </summary>
      /// <typeparam name="T">TThe type of the classifiable being created</typeparam>
      /// <param name="classifiables">The snapshots that are being converted</param>
      /// <param name="subjects">The list of subjects which will be used to create the classifiable. These are matched by name to the classifiable</param>
      /// <param name="classifications">The classifiable parent will be set to the classification from this list with the matching path</param>
      /// <returns>The classifiables that are created</returns>
      Task<T[]> ClassifiablesForSnapshots<T>(Classifiable[] classifiables, IReadOnlyCollection<IObjectBase> subjects, IReadOnlyCollection<IClassification> classifications) where T : IClassifiableWrapper;

      Task<ModelClassification[]> ClassificationsForSnapshots(SnapshotClassification[] snapshots);
      Task<SnapshotClassification[]> MapClassificationsToSnapshots(IReadOnlyList<ModelClassification> classifications);
   }

   public class ClassificationSnapshotService : IClassificationSnapshotService
   {
      private readonly ClassificationMapper _classificationMapper;
      private readonly IExecutionContext _executionContext;

      public ClassificationSnapshotService(ClassificationMapper classificationMapper, IExecutionContext executionContext)
      {
         _classificationMapper = classificationMapper;
         _executionContext = executionContext;
      }

      public async Task<ModelClassification[]> ClassificationsForSnapshots(SnapshotClassification[] snapshots)
      {
         var context = createClassificationContext(snapshots);
         var tasks = snapshots.SelectMany(x => leavesFrom(x).Select(classification => _classificationMapper.MapToModel(classification, context)));

         return await Task.WhenAll(tasks);
      }

      public async Task<T[]> ClassifiablesForSnapshots<T>(Classifiable[] classifiables, IReadOnlyCollection<IObjectBase> subjects, IReadOnlyCollection<IClassification> classifications) where T : IClassifiableWrapper
      {
         var classifiedSnapshots = classifiables.Where(x => x.ClassificationPath != null);
         var tasks = classifiedSnapshots.Select(x =>
         {
            var subject = subjects.FirstOrDefault(objectBase => string.Equals(objectBase.Name, x.Name));
            var parentClassification = classifications.FirstOrDefault(classification => string.Equals(classification.Path, x.ClassificationPath));
            return createModelForClassifiable<T>(x, subject, parentClassification);
         });

         return await Task.WhenAll(tasks);
      }

      public Task<SnapshotClassification[]> MapClassificationsToSnapshots(IReadOnlyList<ModelClassification> classifications)
      {
         var context = new ClassificationContext
         {
            Classifications = classifications
         };

         var rootClassifications = findRoots(context.Classifications);

         var tasks = rootClassifications.Select(x => _classificationMapper.MapToSnapshot(x, context));

         return Task.WhenAll(tasks);
      }

      private IEnumerable<T> findRoots<T>(IEnumerable<T> classifications) where T : IClassifiable
      {
         return classifications.Where(x => x.Parent == null);
      }

      private async Task<T> createModelForClassifiable<T>(Classifiable snapshot, IWithId subject, IClassification parentClassification) where T : IClassifiableWrapper
      {
         var snapshotMapper = _executionContext.Resolve<ISnapshotMapper>();
         var modelForClassifiable = (await snapshotMapper.MapToModel(snapshot)).DowncastTo<T>();
         modelForClassifiable.UpdateSubject(subject);
         modelForClassifiable.Parent = parentClassification;
         return modelForClassifiable;
      }

      private SnapshotClassificationContext createClassificationContext(SnapshotClassification[] classifications)
      {
         var childParentCache = new SnapshotClassificationContext();
         classifications.Each(classification => contextFor(classification, childParentCache));
         return childParentCache;
      }

      private void contextFor(SnapshotClassification classification, SnapshotClassificationContext context)
      {
         classification.Classifications?.Each(child =>
         {
            contextFor(child, context);
            context.AddClassificationWithParent(child, classification);
         });
      }

      private IEnumerable<SnapshotClassification> leavesFrom(SnapshotClassification classification)
      {
         var children = classification.Classifications;
         return children != null && children.Any() ? children.SelectMany(leavesFrom) : new[] { classification };
      }
   }
}