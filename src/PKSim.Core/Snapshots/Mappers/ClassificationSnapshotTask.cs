using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using ModelClassification = OSPSuite.Core.Domain.Classification;
using SnapshotClassification = PKSim.Core.Snapshots.Classification;

namespace PKSim.Core.Snapshots.Mappers
{
   public interface IClassificationSnapshotTask
   {
      /// <summary>
      ///    Maps the <paramref name="snapshots" /> into classifications  and adds them to the <paramref name="project" />.
      ///    Also added to the project will be classifiables contained in the classifications and those will have the subjects
      ///    configured correctly from the list of <paramref name="subjects" />.
      /// </summary>
      /// <typeparam name="TClassifiable">
      ///    This is the type of classifiable that is created for each classifiable in each newly
      ///    created classification />
      /// </typeparam>
      /// <typeparam name="TSubject">This is the type of the subject for the classifiable</typeparam>
      Task UpdateProjectClassifications<TClassifiable, TSubject>(SnapshotClassification[] snapshots, PKSimProject project, IReadOnlyCollection<TSubject> subjects)
         where TClassifiable : Classifiable<TSubject>, new() where TSubject : IObjectBase;

      Task<SnapshotClassification[]> MapClassificationsToSnapshots<TClassifiable>(PKSimProject project) where TClassifiable : class, IClassifiableWrapper, new();
   }

   public class ClassificationSnapshotTask : IClassificationSnapshotTask
   {
      private readonly ClassificationMapper _classificationMapper;

      public ClassificationSnapshotTask(ClassificationMapper classificationMapper)
      {
         _classificationMapper = classificationMapper;
      }

      public Task UpdateProjectClassifications<TClassifiable, TSubject>(SnapshotClassification[] snapshots, PKSimProject project, IReadOnlyCollection<TSubject> subjects)
         where TClassifiable : Classifiable<TSubject>, new() where TSubject : IObjectBase
      {
         if (snapshots == null)
            return Task.FromResult(false);

         var tasks = snapshots.Select(snapshot => updateProjectFor<TClassifiable, TSubject>(project, snapshot, subjects));
         return Task.WhenAll(tasks);
      }

      private async Task updateProjectFor<TClassifiable, TSubject>(PKSimProject project, SnapshotClassification snapshot, IReadOnlyCollection<TSubject> subjects, ModelClassification parent = null)
         where TClassifiable : Classifiable<TSubject>, new() where TSubject : IObjectBase
      {
         var classification = await _classificationMapper.MapToModel(snapshot, classificationTypeFor<TClassifiable>());
         classification.Parent = parent;
         project.AddClassification(classification);

         snapshot.Classifiables?.Each(snapshotClassifiable =>
         {
            var subject = subjects.FindByName(snapshotClassifiable);
            var classifiable = project.GetOrCreateClassifiableFor<TClassifiable, TSubject>(subject);
            classifiable.Parent = classification;
         });

         if (snapshot.Classifications != null)
         {
            var tasks = snapshot.Classifications.Select(x => updateProjectFor<TClassifiable, TSubject>(project, x, subjects, classification));
            await Task.WhenAll(tasks);
         }
      }

      public Task<SnapshotClassification[]> MapClassificationsToSnapshots<TClassifiable>(PKSimProject project) where TClassifiable : class, IClassifiableWrapper, new()
      {
         var classifications = project.AllClassificationsByType(classificationTypeFor<TClassifiable>()).OfType<ModelClassification>().ToList();
         var classifiables = project.AllClassifiablesByType<TClassifiable>();

         var context = new ClassificationContext
         {
            Classifications = classifications,
            Classifiables = classifiables
         };

         var rootClassifications = findRoots(classifications);

         return _classificationMapper.MapToSnapshots(rootClassifications, context);
      }

      private IEnumerable<T> findRoots<T>(IEnumerable<T> classifications) where T : IClassifiable => classifications.Where(x => x.Parent == null);

      private ClassificationType classificationTypeFor<T>() where T : IClassifiable, new()
      {
         var classifiable = new T();
         return classifiable.ClassificationType;
      }
   }
}