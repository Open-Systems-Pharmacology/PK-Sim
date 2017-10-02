using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
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
      private Classification _subClassification;
      protected OSPSuite.Core.Domain.Classification _subModelClassification;

      protected override Task Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _classificationMapper = A.Fake<ClassificationMapper>();
         _snapshotMapper = A.Fake<ISnapshotMapper>();
         sut = new ClassificationSnapshotTask(_classificationMapper);

         _classifications = new List<OSPSuite.Core.Domain.Classification>();

         _modelClassification = new OSPSuite.Core.Domain.Classification().WithName("classification");
         _subModelClassification = new OSPSuite.Core.Domain.Classification().WithName("subModelClassification");
         _modelClassification.ClassificationType = ClassificationType.ObservedData;
         _subModelClassification.ClassificationType = ClassificationType.ObservedData;
         _snapshotClassification = new Classification().WithName("classification");
         _snapshotClassification.Classifiables = new[] { "subject" };
         _subClassification = new Classification().WithName("subClassification");
         _snapshotClassification.Classifications = new[] { _subClassification };

         _classifications.Add(_modelClassification);
         A.CallTo(() => _classificationMapper.MapToSnapshot(_modelClassification, A<ClassificationContext>._)).ReturnsAsync(_snapshotClassification);
         A.CallTo(() => _classificationMapper.MapToModel(_snapshotClassification, ClassificationType.ObservedData)).ReturnsAsync(_modelClassification);
         A.CallTo(() => _classificationMapper.MapToModel(_subClassification, ClassificationType.ObservedData)).ReturnsAsync(_subModelClassification);

         A.CallTo(() => _executionContext.Resolve<ISnapshotMapper>()).Returns(_snapshotMapper);

         return _completed;
      }
   }

   public class When_mapping_snapshots_to_classifications : concern_for_ClassificationSnapshotTask
   {
      private IReadOnlyCollection<DataRepository> _subjects;
      private PKSimProject _project;

      protected override Task Context()
      {
         base.Context();

         _project = new PKSimProject();
         _subjects = new List<DataRepository> { DomainHelperForSpecs.ObservedData().WithName("subject") };

         return _completed;
      }

      protected override async Task Because()
      {
         await sut.UpdateProjectClassifications<ClassifiableObservedData, DataRepository>(new[] { _snapshotClassification }, _project, _subjects, ClassificationType.ObservedData);
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

   public class When_mapping_classifications_to_snapshots : concern_for_ClassificationSnapshotTask
   {
      private Classification[] _result;

      protected override async Task Because()
      {
         _result = await sut.MapClassificationsToSnapshots(_classifications, new List<IClassifiableWrapper>());
      }

      [Observation]
      public void should_use_the_classification_mapper_to_map_snapshots()
      {
         _result.ShouldContain(_snapshotClassification);
      }
   }
}
