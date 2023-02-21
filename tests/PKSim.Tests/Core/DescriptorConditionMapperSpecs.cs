using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_DescriptorConditionMapper : ContextSpecificationAsync<DescriptorConditionMapper>
   {
      protected IOSPSuiteLogger _logger;
      protected InContainerCondition _inContainer;
      protected MatchAllCondition _matchAllCondition;
      protected MatchTagCondition _notMatchCondition;
      protected NotInContainerCondition _notInContainer;
      protected NotMatchTagCondition _notMatchAllCondition;
      protected InParentCondition _inParentCondition;

      protected override Task Context()
      {
         _logger = A.Fake<IOSPSuiteLogger>();
         sut = new DescriptorConditionMapper(_logger);

         _inContainer = new InContainerCondition("CONT");
         _notInContainer = new NotInContainerCondition("NOT_CONT");
         _matchAllCondition = new MatchAllCondition();
         _notMatchAllCondition = new NotMatchTagCondition("NOT_MATCH");
         _notMatchCondition = new MatchTagCondition("MATCH");
         _inParentCondition = new InParentCondition();

         return _completed;
      }
   }

   public class When_mapping_some_valid_condition_descriptor_to_snapshot : concern_for_DescriptorConditionMapper
   {
      private DescriptorCondition _inContainerSnapshot;
      private DescriptorCondition _notInContainerSnapshot;
      private DescriptorCondition _matchAllConditionSnapshot;
      private DescriptorCondition _notMatchAllConditionSnapshot;
      private DescriptorCondition _notMatchConditionSnapshot;
      private DescriptorCondition _inParentConditionSnapshot;

      protected override async Task Because()
      {
         _inContainerSnapshot = await sut.MapToSnapshot(_inContainer);
         _notInContainerSnapshot = await sut.MapToSnapshot(_notInContainer);
         _matchAllConditionSnapshot = await sut.MapToSnapshot(_matchAllCondition);
         _notMatchAllConditionSnapshot = await sut.MapToSnapshot(_notMatchAllCondition);
         _notMatchConditionSnapshot = await sut.MapToSnapshot(_notMatchCondition);
         _inParentConditionSnapshot = await sut.MapToSnapshot(_inParentCondition);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _inContainerSnapshot.Tag.ShouldBeEqualTo(_inContainer.Tag);
         _notInContainerSnapshot.Tag.ShouldBeEqualTo(_notInContainer.Tag);
         _matchAllConditionSnapshot.Tag.ShouldBeEqualTo(_matchAllCondition.Tag);
         _notMatchAllConditionSnapshot.Tag.ShouldBeEqualTo(_notMatchAllCondition.Tag);
         _notMatchConditionSnapshot.Tag.ShouldBeEqualTo(_notMatchCondition.Tag);
         _inParentConditionSnapshot.Tag.ShouldBeEqualTo(_inParentCondition.Tag);
      }
   }

   public class When_mapping_some_valid_condition_descriptor_snapshot_to_condition : concern_for_DescriptorConditionMapper
   {
      private DescriptorCondition _inContainerSnapshot;
      private DescriptorCondition _notInContainerSnapshot;
      private DescriptorCondition _matchAllConditionSnapshot;
      private DescriptorCondition _notMatchAllConditionSnapshot;
      private DescriptorCondition _notMatchConditionSnapshot;
      private DescriptorCondition _inParentConditionSnapshot;

      private ITagCondition _newInContainer;
      private ITagCondition _newNotInContainer;
      private ITagCondition _newMatchAllCondition;
      private ITagCondition _newNotMatchAllCondition;
      private ITagCondition _newNotMatchCondition;
      private ITagCondition _newInParentCondition;

      protected override async Task Context()
      {
         await base.Context();
         _inContainerSnapshot = await sut.MapToSnapshot(_inContainer);
         _notInContainerSnapshot = await sut.MapToSnapshot(_notInContainer);
         _matchAllConditionSnapshot = await sut.MapToSnapshot(_matchAllCondition);
         _notMatchAllConditionSnapshot = await sut.MapToSnapshot(_notMatchAllCondition);
         _notMatchConditionSnapshot = await sut.MapToSnapshot(_notMatchCondition);
         _inParentConditionSnapshot = await sut.MapToSnapshot(_inParentCondition);
      }

      protected override async Task Because()
      {
         _newInContainer = await sut.MapToModel(_inContainerSnapshot, new SnapshotContext());
         _newNotInContainer = await sut.MapToModel(_notInContainerSnapshot, new SnapshotContext());
         _newMatchAllCondition = await sut.MapToModel(_matchAllConditionSnapshot, new SnapshotContext());
         _newNotMatchAllCondition = await sut.MapToModel(_notMatchAllConditionSnapshot, new SnapshotContext());
         _newNotMatchCondition = await sut.MapToModel(_notMatchConditionSnapshot, new SnapshotContext());
         _newInParentCondition = await sut.MapToModel(_inParentConditionSnapshot, new SnapshotContext());
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _newInContainer.ShouldBeEqualTo(_inContainer);
         _newNotInContainer.ShouldBeEqualTo(_notInContainer);
         _newMatchAllCondition.ShouldBeEqualTo(_matchAllCondition);
         _newNotMatchAllCondition.ShouldBeEqualTo(_notMatchAllCondition);
         _newNotMatchCondition.ShouldBeEqualTo(_notMatchCondition);
         _newInParentCondition.ShouldBeEqualTo(_inParentCondition);
      }
   }


   public class When_mapping_some_invalid_unknown_descriptor_to_snapshot : concern_for_DescriptorConditionMapper
   {
      private ITagCondition _invalid;
      private DescriptorCondition _result;

      protected override async Task Context()
      {
         await base.Context();
         _invalid = A.Fake<ITagCondition>();
      }

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_invalid);
      }

      [Observation]
      public void should_log_the_error()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }

   public class When_mapping_some_invalid_condition_descriptor_snapshot_to_condition : concern_for_DescriptorConditionMapper
   {
      private DescriptorCondition _invalid;
      private ITagCondition _result;

      protected override async Task Context()
      {
         await base.Context();
         _invalid = new DescriptorCondition {Type = "UNKNOWN"};
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_invalid, new SnapshotContext());
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }

      [Observation]
      public void should_log_the_error()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }
}