using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Presentation.Core;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_GroupingDefinitionToGroupingDefinitionPresenterMapper : ContextSpecification<IGroupingDefinitionToGroupingDefinitionPresenterMapper>
   {
      protected IApplicationController _applicationController;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         sut = new GroupingDefinitionToGroupingDefinitionPresenterMapper(_applicationController);
      }
   }

   public class When_returning_the_grouping_presenter_for_a_given_grouping_definition : concern_for_GroupingDefinitionToGroupingDefinitionPresenterMapper
   {
      private IFixedLimitsGroupingPresenter _fixedLimitsGroupingPresenter;
      private INumberOfBinsGroupingPresenter _numberOfBinsPresenter;
      private IValueMappingGroupingPresenter _valueMappingPresenter;

      protected override void Context()
      {
         base.Context();
         _fixedLimitsGroupingPresenter = A.Fake<IFixedLimitsGroupingPresenter>();
         _numberOfBinsPresenter = A.Fake<INumberOfBinsGroupingPresenter>();
         _valueMappingPresenter= A.Fake<IValueMappingGroupingPresenter>();
         A.CallTo(() => _applicationController.Start<IFixedLimitsGroupingPresenter>()).Returns(_fixedLimitsGroupingPresenter);
         A.CallTo(() => _applicationController.Start<INumberOfBinsGroupingPresenter>()).Returns(_numberOfBinsPresenter);
         A.CallTo(() => _applicationController.Start<IValueMappingGroupingPresenter>()).Returns(_valueMappingPresenter);
      }

      [Observation]
      public void should_return_the_fixed_limit_presenter_for_fixed_limit_grouping()
      {
         sut.MapFrom(GroupingDefinitions.FixedLimits).ShouldBeEqualTo(_fixedLimitsGroupingPresenter);
      }

      [Observation]
      public void should_return_the_number_of_bins_presenter_for_number_of_bins()
      {
         sut.MapFrom(GroupingDefinitions.NumberOfBins).ShouldBeEqualTo(_numberOfBinsPresenter);
      }

      [Observation]
      public void should_return_the_value_mapping_presenter_for_value_mapping()
      {
         sut.MapFrom(GroupingDefinitions.ValueMapping).ShouldBeEqualTo(_valueMappingPresenter);
      }
   }
}