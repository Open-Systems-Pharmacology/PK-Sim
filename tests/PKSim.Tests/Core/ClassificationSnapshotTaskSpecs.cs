using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using Classification = PKSim.Core.Snapshots.Classification;
using DataRepository = OSPSuite.Core.Domain.Data.DataRepository;

namespace PKSim.Core
{
   public abstract class concern_for_ClassificationSnapshotTask : ContextSpecificationAsync<ClassificationSnapshotTask>
   {
      private IExecutionContext _executionContext;
      protected ClassificationMapper _classificationMapper;
      protected List<OSPSuite.Core.Domain.Classification> _classifications;
      protected OSPSuite.Core.Domain.Classification _modelClassification;
      protected Classification _snapshotClassification;
      protected ISnapshotMapper _snapshotMapper;

      protected override Task Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _classificationMapper = A.Fake<ClassificationMapper>();
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         sut = new ClassificationSnapshotTask(_classificationMapper, _executionContext);

         _classifications = new List<OSPSuite.Core.Domain.Classification>();

         _modelClassification = new OSPSuite.Core.Domain.Classification();
         _snapshotClassification = new Classification();

         _classifications.Add(_modelClassification);
         A.CallTo(() => _classificationMapper.MapToSnapshot(_modelClassification, A<ClassificationContext>._)).ReturnsAsync(_snapshotClassification);
         A.CallTo(() => _classificationMapper.MapToModel(_snapshotClassification, A<SnapshotClassificationContext>._)).ReturnsAsync(_modelClassification);
         A.CallTo(() => _executionContext.Resolve<ISnapshotMapper>()).Returns(_snapshotMapper);

         return _completed;
      }
   }

   public class When_mapping_classifiables_for_snapshots : concern_for_ClassificationSnapshotTask
   {
      private ClassifiableObservedData[] _result;
      private Classifiable _snapshotClassifiable;
      private IReadOnlyCollection<IObjectBase> _subjects;
      private IReadOnlyCollection<IClassification> _modelClassifications;
      private ClassifiableObservedData _modelClassifiable;
      private DataRepository _dataRepository;
      private OSPSuite.Core.Domain.Classification _parentClassification;

      protected override Task Context()
      {
         base.Context();

         _snapshotClassifiable = new Classifiable {ClassificationPath = "hi", Name = "repositoryName"};
         _dataRepository = DomainHelperForSpecs.ObservedData().WithName("repositoryName");
         _subjects = new[] {_dataRepository};
         _parentClassification = new OSPSuite.Core.Domain.Classification().WithName("hi");
         _modelClassifications = new List<IClassification> {_parentClassification};

         _modelClassifiable = new ClassifiableObservedData();

         A.CallTo(() => _snapshotMapper.MapToModel(_snapshotClassifiable)).ReturnsAsync(_modelClassifiable);
         return _completed;
      }

      protected override async Task Because()
      {
         _result = await sut.ClassifiablesForSnapshots<ClassifiableObservedData>(new[] {_snapshotClassifiable}, _subjects, _modelClassifications);
      }

      [Observation]
      public void the_parent_classification_should_be_configured_correctly()
      {
         _modelClassifiable.Parent.ShouldBeEqualTo(_parentClassification);
      }

      [Observation]
      public void the_classifiable_subject_should_be_configured_correctly()
      {
         _modelClassifiable.Subject.ShouldBeEqualTo(_dataRepository);
      }

      [Observation]
      public void the_result_must_contain_a_classifiable_with_subject()
      {
         _result.ShouldContain(_modelClassifiable);
      }
   }

   public class When_mapping_snapshots_to_classifications : concern_for_ClassificationSnapshotTask
   {
      private OSPSuite.Core.Domain.Classification[] _result;

      protected override async Task Because()
      {
         _result = await sut.ClassificationsForSnapshots(new[] { _snapshotClassification });
      }

      [Observation]
      public void the_snapshot_mapper_should_be_used_to_map_models()
      {
         _result.ShouldContain(_modelClassification);
      }
   }

   public class When_mapping_classifications_to_snapshots : concern_for_ClassificationSnapshotTask
   {
      private Classification[] _result;

      protected override async Task Because()
      {
         _result = await sut.MapClassificationsToSnapshots(_classifications);
      }

      [Observation]
      public void should_use_the_classification_mapper_to_map_snapshots()
      {
         _result.ShouldContain(_snapshotClassification);
      }
   }
}
