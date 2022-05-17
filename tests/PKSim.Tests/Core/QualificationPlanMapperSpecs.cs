using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using QualificationPlan = PKSim.Core.Model.QualificationPlan;

namespace PKSim.Core
{
   public abstract class concern_for_QualificationPlanMapper : ContextSpecificationAsync<QualificationPlanMapper>
   {
      protected QualificationStepMapper _qualificationStepMapper;
      protected IObjectBaseFactory _objectBaseFactory;
      protected QualificationPlan _qualificationPlan;
      protected Snapshots.QualificationPlan _snapshot;
      protected IQualificationStep _qualificationStep;
      protected QualificationStep _qualificationStepSnapshot;
      protected PKSimProject _project;
      protected SnapshotContext _snapshotContext;

      protected override Task Context()
      {
         _qualificationStepMapper = A.Fake<QualificationStepMapper>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         sut = new QualificationPlanMapper(_qualificationStepMapper, _objectBaseFactory);

         _qualificationStep = A.Fake<IQualificationStep>();
         _qualificationPlan = new QualificationPlan {_qualificationStep}.WithName("QP");
         _qualificationStepSnapshot = new QualificationStep();

         A.CallTo(() => _qualificationStepMapper.MapToSnapshot(_qualificationStep)).Returns(_qualificationStepSnapshot);

         _project = new PKSimProject();
         _snapshotContext = new SnapshotContext(_project, ProjectVersions.Current);
         return _completed;
      }
   }

   public class When_mapping_a_qualification_plan_to_snapshot : concern_for_QualificationPlanMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_qualificationPlan);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_qualificationPlan.Name);
      }

      [Observation]
      public void should_have_mapped_the_qualification_step_to_snapshot()
      {
         _snapshot.Steps.ShouldContain(_qualificationStepSnapshot);
      }
   }

   public class When_mapping_a_qualification_plan_snapshot_to_qualification_plan : concern_for_QualificationPlanMapper
   {
      private QualificationPlan _newQualificationPlan;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_qualificationPlan);
         A.CallTo(() => _objectBaseFactory.Create<QualificationPlan>()).Returns(new QualificationPlan());
         A.CallTo(() => _qualificationStepMapper.MapToModel(_qualificationStepSnapshot, _snapshotContext)).Returns(_qualificationStep);
      }

      protected override async Task Because()
      {
         _newQualificationPlan = await sut.MapToModel(_snapshot, _snapshotContext);
      }

      [Observation]
      public void should_return_a_qualification_plan_having_the_expected_properties()
      {
         _newQualificationPlan.Name.ShouldBeEqualTo(_qualificationPlan.Name);
      }

      [Observation]
      public void should_have_mapped_the_qualification_steps_from_snapshot()
      {
         _newQualificationPlan.Steps.ShouldContain(_qualificationStep);
      }
   }
}