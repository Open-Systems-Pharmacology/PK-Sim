using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Descriptors;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using ILogger = OSPSuite.Core.Services.ILogger;

namespace PKSim.Core
{
   public abstract class concern_for_DescriptorConditionMapper : ContextSpecificationAsync<DescriptorConditionMapper>
   {
      protected ILogger _logger;
      protected InContainerCondition _inContainer;
      protected MatchAllCondition _matchAllCondition;
      protected MatchTagCondition _notMatchCondition;
      protected NotInContainerCondition _notInContainer;
      protected NotMatchTagCondition _notMatchAllCondition;

      protected override Task Context()
      {
         _logger = A.Fake<ILogger>();
         sut = new DescriptorConditionMapper(_logger);

         _inContainer = new InContainerCondition("CONT");
         _notInContainer = new NotInContainerCondition("NOT_CONT");
         _matchAllCondition = new MatchAllCondition();
         _notMatchAllCondition = new NotMatchTagCondition("NOT_MATCH");
         _notMatchCondition = new MatchTagCondition("MATCH");

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

      protected override async Task Because()
      {
         _inContainerSnapshot = await sut.MapToSnapshot(_inContainer);
         _notInContainerSnapshot = await sut.MapToSnapshot(_notInContainer);
         _matchAllConditionSnapshot = await sut.MapToSnapshot(_matchAllCondition);
         _notMatchAllConditionSnapshot = await sut.MapToSnapshot(_notMatchAllCondition);
         _notMatchConditionSnapshot = await sut.MapToSnapshot(_notMatchCondition);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _inContainerSnapshot.Tag.ShouldBeEqualTo(_inContainer.Tag);
         _notInContainerSnapshot.Tag.ShouldBeEqualTo(_notInContainer.Tag);
         _matchAllConditionSnapshot.Tag.ShouldBeEqualTo(_matchAllCondition.Tag);
         _notMatchAllConditionSnapshot.Tag.ShouldBeEqualTo(_notMatchAllCondition.Tag);
         _notMatchConditionSnapshot.Tag.ShouldBeEqualTo(_notMatchCondition.Tag);
      }
   }

   public class When_mapping_some_valid_condition_descriptor_snapshot_to_condition : concern_for_DescriptorConditionMapper
   {
      private DescriptorCondition _inContainerSnapshot;
      private DescriptorCondition _notInContainerSnapshot;
      private DescriptorCondition _matchAllConditionSnapshot;
      private DescriptorCondition _notMatchAllConditionSnapshot;
      private DescriptorCondition _notMatchConditionSnapshot;
      private IDescriptorCondition _newInContainer;
      private IDescriptorCondition _newNotInContainer;
      private IDescriptorCondition _newMatchAllCondition;
      private IDescriptorCondition _newNotMatchAllCondition;
      private IDescriptorCondition _newNotMatchCondition;

      protected override async Task Context()
      {
         await base.Context();
         _inContainerSnapshot = await sut.MapToSnapshot(_inContainer);
         _notInContainerSnapshot = await sut.MapToSnapshot(_notInContainer);
         _matchAllConditionSnapshot = await sut.MapToSnapshot(_matchAllCondition);
         _notMatchAllConditionSnapshot = await sut.MapToSnapshot(_notMatchAllCondition);
         _notMatchConditionSnapshot = await sut.MapToSnapshot(_notMatchCondition);
      }

      protected override async Task Because()
      {
         _newInContainer = await sut.MapToModel(_inContainerSnapshot);
         _newNotInContainer = await sut.MapToModel(_notInContainerSnapshot);
         _newMatchAllCondition = await sut.MapToModel(_matchAllConditionSnapshot);
         _newNotMatchAllCondition = await sut.MapToModel(_notMatchAllConditionSnapshot);
         _newNotMatchCondition = await sut.MapToModel(_notMatchConditionSnapshot);
      }

      [Observation]
      public void should_return_a_valid_snapshot()
      {
         _newInContainer.ShouldBeEqualTo(_inContainer);
         _newNotInContainer.ShouldBeEqualTo(_notInContainer);
         _newMatchAllCondition.ShouldBeEqualTo(_matchAllCondition);
         _newNotMatchAllCondition.ShouldBeEqualTo(_notMatchAllCondition);
         _newNotMatchCondition.ShouldBeEqualTo(_notMatchCondition);
      }
   }


   public class When_mapping_some_invalid_unknown_descriptor_to_snapshot : concern_for_DescriptorConditionMapper
   {
      private IDescriptorCondition _invalid;
      private DescriptorCondition _result;

      protected override async Task Context()
      {
         await base.Context();
         _invalid = A.Fake<IDescriptorCondition>();;
      }

      protected override async Task Because()
      {
         _result = await sut.MapToSnapshot(_invalid);
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

   public class When_mapping_some_invalid_condition_descriptor_snapshot_to_condition : concern_for_DescriptorConditionMapper
   {
      private DescriptorCondition _invalid;
      private IDescriptorCondition _result;

      protected override async Task Context()
      {
         await base.Context();
         _invalid = new DescriptorCondition {Type = "UNKNOWN"};
      }

      protected override async Task Because()
      {
         _result = await sut.MapToModel(_invalid);
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