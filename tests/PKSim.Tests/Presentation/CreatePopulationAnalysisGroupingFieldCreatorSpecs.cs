using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisGroupingFieldCreator : ContextSpecification<IPopulationAnalysisGroupingFieldCreator>
   {
      protected IApplicationController _applicationController;
      protected IPopulationAnalysisField _field;
      protected IPopulationDataCollector _populationDataCollector;
      protected IPopulationAnalysisFieldFactory _populationAnalysisFieldFactory;
      protected ICreatePopulationAnalysisGroupingFieldPresenter _groupingFieldPresenter;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _populationAnalysisFieldFactory = A.Fake<IPopulationAnalysisFieldFactory>();
         _field = A.Fake<IPopulationAnalysisField>();
         sut = new PopulationAnalysisGroupingFieldCreator(_applicationController, _populationAnalysisFieldFactory);


         _groupingFieldPresenter = A.Fake<ICreatePopulationAnalysisGroupingFieldPresenter>();
         A.CallTo(() => _applicationController.Start<ICreatePopulationAnalysisGroupingFieldPresenter>()).Returns(_groupingFieldPresenter);
      }
   }

   public class When_creating_a_grouping_field_for_a_given_analysis_field : concern_for_PopulationAnalysisGroupingFieldCreator
   {
      private PopulationAnalysisGroupingField _result;
      private GroupingDefinition _groupingDefinition;
      private PopulationAnalysisGroupingField _populationAnalysisGroupingField;

      protected override void Context()
      {
         base.Context();
         _groupingDefinition = A.Fake<GroupingDefinition>();
         _populationAnalysisGroupingField = A.Fake<PopulationAnalysisGroupingField>();
         A.CallTo(() => _groupingFieldPresenter.CreateGrouping(_field, _populationDataCollector)).Returns(_groupingDefinition);
         A.CallTo(() => _populationAnalysisFieldFactory.CreateGroupingField(_groupingDefinition, _field)).Returns(_populationAnalysisGroupingField);
         A.CallTo(() => _groupingFieldPresenter.FieldName).Returns("SUPER NAME");
      }

      protected override void Because()
      {
         _result = sut.CreateGroupingFieldFor(_field, _populationDataCollector);
      }

      [Observation]
      public void should_ask_the_user_to_select_the_grouping_to_use_for_the_analysis()
      {
         A.CallTo(() => _applicationController.Start<ICreatePopulationAnalysisGroupingFieldPresenter>()).MustHaveHappened();
      }

      [Observation]
      public void should_return_the_grouping_field_initialized_with_the_grouping_defined_by_the_user()
      {
         _result.ShouldBeEqualTo(_populationAnalysisGroupingField);
      }

      [Observation]
      public void should_set_the_name_of_the_field_according_to_the_user_inputs()
      {
         _result.Name.ShouldBeEqualTo("SUPER NAME");
      }
   }

   public class When_the_user_cancels_the_creation_of_a_grouping_field : concern_for_PopulationAnalysisGroupingFieldCreator
   {
      private PopulationAnalysisGroupingField _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _groupingFieldPresenter.CreateGrouping(_field, _populationDataCollector)).Returns(null);
      }

      protected override void Because()
      {
         _result = sut.CreateGroupingFieldFor(_field, _populationDataCollector);
      }

      [Observation]
      public void should_return_a_null_value_signifying_that_the_action_was_cancelled()
      {
         _result.ShouldBeNull();
      }
   }
}