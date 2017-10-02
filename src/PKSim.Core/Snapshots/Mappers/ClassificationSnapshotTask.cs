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
      ///   Maps the <paramref name="snapshots"/> into classifications of type <paramref name="classificationType"/> and adds them to the <paramref name="project"/>.
      ///   Also added to the project will be classifiables contained in the classifications and those will have the subjects
      ///   configured correctly from the list of <paramref name="subjects"/>.
      /// </summary>
      /// <typeparam name="TClassifiable">This is the type of classifiable that is created for each classifiable in each newly created classification /></typeparam>
      /// <typeparam name="TSubject">This is the type of the subject for the classifiable</typeparam>
      Task UpdateProjectClassifications<TClassifiable, TSubject>(SnapshotClassification[] snapshots, PKSimProject project, IReadOnlyCollection<TSubject> subjects, ClassificationType classificationType) 
         where TClassifiable : Classifiable<TSubject>, new() where TSubject : IObjectBase;

      Task<SnapshotClassification[]> MapClassificationsToSnapshots(IReadOnlyList<ModelClassification> classifications, IReadOnlyList<IClassifiableWrapper> classifiables);
   }

   public class ClassificationSnapshotTask : IClassificationSnapshotTask
   {
      private readonly ClassificationMapper _classificationMapper;

      public ClassificationSnapshotTask(ClassificationMapper classificationMapper)
      {
         _classificationMapper = classificationMapper;
      }

      public Task UpdateProjectClassifications<TClassifiable, TSubject>(SnapshotClassification[] snapshots, PKSimProject project, IReadOnlyCollection<TSubject> subjects, ClassificationType classificationType) 
         where TClassifiable : Classifiable<TSubject>, new() where TSubject : IObjectBase
      {
         var tasks = snapshots.Select(snapshot => updateProjectFor<TClassifiable, TSubject>(project, snapshot, subjects, classificationType));

         return Task.WhenAll(tasks);
      }

      private async Task updateProjectFor<TClassifiable, TSubject>(PKSimProject project, SnapshotClassification snapshot, IReadOnlyCollection<TSubject> subjects, ClassificationType classificationType, ModelClassification parent = null) 
         where TClassifiable : Classifiable<TSubject>, new() where TSubject : IObjectBase
      {
         var model = await _classificationMapper.MapToModel(snapshot, classificationType);
         model.Parent = parent;
         project.AddClassification(model);

         snapshot.Classifiables?.Each(snapshotClassifiable =>
         {
            var subject = subjects.FirstOrDefault(x => string.Equals(x.Name, snapshotClassifiable));
            var classifiable = project.GetOrCreateClassifiableFor<TClassifiable, TSubject>(subject);
            classifiable.Parent = model;
         });

         if (snapshot.Classifications != null && snapshot.Classifications.Any())
         {
            var tasks = snapshot.Classifications.Select(x => updateProjectFor<TClassifiable, TSubject>(project, x, subjects, classificationType, model));
            await Task.WhenAll(tasks);
         }
      }

      public Task<SnapshotClassification[]> MapClassificationsToSnapshots(IReadOnlyList<ModelClassification> classifications, IReadOnlyList<IClassifiableWrapper> classifiables)
      {
         var context = new ClassificationContext
         {
            Classifications = classifications,
            Classifiables = classifiables
         };

         var rootClassifications = findRoots(context.Classifications);
         
         var tasks = rootClassifications.Select(x => _classificationMapper.MapToSnapshot(x, context));

         return Task.WhenAll(tasks);
      }

      private IEnumerable<T> findRoots<T>(IEnumerable<T> classifications) where T : IClassifiable
      {
         return classifications.Where(x => x.Parent == null);
      }
   }
}