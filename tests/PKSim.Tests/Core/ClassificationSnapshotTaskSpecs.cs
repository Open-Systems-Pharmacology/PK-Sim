using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_ClassificationSnapshotTask : ContextSpecificationAsync<ClassificationSnapshotTask>
   {
      private IExecutionContext _executionContext;
      protected ClassificationMapper _classificationMapper;
      protected List<Classification> _classifications;
      protected Classification _modelClassification;
      protected Snapshots.Classification _snapshotClassification;
      protected ISnapshotMapper _snapshotMapper;
      private Snapshots.Classification _subClassification;
      protected Classification _subModelClassification;

      protected override Task Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _classificationMapper = A.Fake<ClassificationMapper>();
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         sut = new ClassificationSnapshotTask(_classificationMapper);

         _classifications = new List<Classification>();

         _modelClassification = new Classification().WithName("classification");
         _subModelClassification = new Classification().WithName("subModelClassification");
         _modelClassification.ClassificationType = ClassificationType.ObservedData;
         _subModelClassification.ClassificationType = ClassificationType.ObservedData;
         _snapshotClassification = new Snapshots.Classification().WithName("classification");
         _snapshotClassification.Classifiables = new[] {"subject"};
         _subClassification = new Snapshots.Classification().WithName("subClassification");
         _snapshotClassification.Classifications = new[] {_subClassification};

         _classifications.Add(_modelClassification);
         A.CallTo(() => _classificationMapper.MapToSnapshot(_modelClassification, A<ClassificationContext>._)).Returns(_snapshotClassification);
         A.CallTo(() => _classificationMapper.MapToModel(_snapshotClassification, ClassificationType.ObservedData)).Returns(_modelClassification);
         A.CallTo(() => _classificationMapper.MapToModel(_subClassification, ClassificationType.ObservedData)).Returns(_subModelClassification);

         A.CallTo(() => _executionContext.Resolve<ISnapshotMapper>()).Returns(_snapshotMapper);

         return _completed;
      }
   }

   public class When_updating_project_classifications_from_snapshot : concern_for_ClassificationSnapshotTask
   {
      private IReadOnlyCollection<DataRepository> _subjects;
      private PKSimProject _project;

      protected override async Task Context()
      {
         await base.Context();

         _project = new PKSimProject();
         _subjects = new List<DataRepository> {DomainHelperForSpecs.ObservedData().WithName("subject")};
      }

      protected override async Task Because()
      {
         await sut.UpdateProjectClassifications<ClassifiableObservedData, DataRepository>(new[] {_snapshotClassification}, _project, _subjects);
      }

      [Observation]
      public void the_heirarchy_should_be_configured()
      {
         _subModelClassification.Parent.ShouldBeEqualTo(_modelClassification);
      }

      [Observation]
      public void the_snapshot_mapper_should_be_used_to_map_models()
      {
         _project.AllClassificationsByType(ClassificationType.ObservedData).ShouldContain(_modelClassification);
         _project.AllClassificationsByType(ClassificationType.ObservedData).ShouldContain(_subModelClassification);
      }
   }

   public class When_updating_project_classsification_for_non_existing_classification_snapshot : concern_for_ClassificationSnapshotTask
   {
      private IReadOnlyCollection<DataRepository> _subjects;
      private PKSimProject _project;
      private DataRepository _observedData;
      private ClassifiableObservedData _originalClassifiable;

      protected override async Task Context()
      {
         await base.Context();

         _project = new PKSimProject();
         _observedData = DomainHelperForSpecs.ObservedData().WithName("subject");
         _originalClassifiable = _project.GetOrCreateClassifiableFor<ClassifiableObservedData, DataRepository>(_observedData);
         _subjects = new List<DataRepository> {_observedData};
      }

      protected override async Task Because()
      {
         await sut.UpdateProjectClassifications<ClassifiableObservedData, DataRepository>(null, _project, _subjects);
      }

      [Observation]
      public void should_not_crash_nor_update_the_existing_classifiable()
      {
         _project.GetOrCreateClassifiableFor<ClassifiableObservedData, DataRepository>(_observedData).ShouldBeEqualTo(_originalClassifiable);
      }
   }

   public class When_mapping_project_classifications_to_snapshots : concern_for_ClassificationSnapshotTask
   {
      private Snapshots.Classification[] _result;
      private PKSimProject _project;
      private DataRepository _obsData;

      protected override async Task Context()
      {
         await base.Context();
         _obsData = DomainHelperForSpecs.ObservedData().WithName("subject");
         _project = new PKSimProject();
         _project.AddClassifiable(new ClassifiableObservedData {Subject = _obsData});
         _project.AddClassification(_modelClassification);
      }

      protected override async Task Because()
      {
         _result = await sut.MapClassificationsToSnapshots<ClassifiableObservedData>(_project);
      }

      [Observation]
      public void should_use_the_classification_mapper_to_map_snapshots()
      {
         _result.ShouldContain(_snapshotClassification);
      }
   }
}