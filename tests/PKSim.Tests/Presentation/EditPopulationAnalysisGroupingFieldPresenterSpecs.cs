using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditPopulationAnalysisGroupingFieldPresenter : ContextSpecification<IEditPopulationAnalysisGroupingFieldPresenter>
   {
      protected IGroupingDefinitionToGroupingDefinitionPresenterMapper _presenterMapper;
      protected IEditPopulationAnalysisGroupingFieldView _view;
      protected PopulationAnalysisGroupingField _groupingField;
      protected IPopulationDataCollector _populationDataCollector;
      protected GroupingDefinition _groupingDefinition;
      protected IGroupingDefinitionPresenter _groupingDefinitionPresenter;
      protected IPopulationAnalysisField _referenceField;

      protected override void Context()
      {
         _view = A.Fake<IEditPopulationAnalysisGroupingFieldView>();
         _groupingDefinition = A.Fake<GroupingDefinition>();
         _presenterMapper = A.Fake<IGroupingDefinitionToGroupingDefinitionPresenterMapper>();
         _groupingField = A.Fake<PopulationAnalysisGroupingField>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _groupingDefinitionPresenter = A.Fake<IGroupingDefinitionPresenter>();
         _groupingField.PopulationAnalysis = A.Fake<PopulationAnalysis>();
         _referenceField = A.Fake<IPopulationAnalysisField>();
         sut = new EditPopulationAnalysisGroupingFieldPresenter(_view, _presenterMapper);

         A.CallTo(() => _groupingField.ReferencedFieldName).Returns("ParameterField");
         A.CallTo(() => _groupingField.GroupingDefinition).Returns(_groupingDefinition);
         A.CallTo(() => _presenterMapper.MapFrom(_groupingDefinition)).Returns(_groupingDefinitionPresenter);
         A.CallTo(() => _groupingField.PopulationAnalysis.FieldByName(_groupingField.ReferencedFieldName)).Returns(_referenceField);
      }
   }

   public class When_editing_a_grouping_field : concern_for_EditPopulationAnalysisGroupingFieldPresenter
   {
      protected override void Because()
      {
         sut.Edit(_groupingField, _populationDataCollector);
      }

      [Observation]
      public void should_retrieve_a_presenter_that_can_handle_the_edition_of_the_grouping_definition()
      {
         A.CallTo(() => _presenterMapper.MapFrom(_groupingDefinition)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_view_of_the_grouping_presenter_into_the_main_view()
      {
         A.CallTo(() => _view.SetGroupingView(_groupingDefinitionPresenter.BaseView)).MustHaveHappened();
      }

      [Observation]
      public void should_initialize_the_presenter_with_the_referenced_fields_and_the_population()
      {
         A.CallTo(() => _groupingDefinitionPresenter.InitializeWith(_referenceField, _populationDataCollector)).MustHaveHappened();
      }

      [Observation]
      public void should_start_the_edit_process_for_the_grouping_definition_and_the_referenced_field()
      {
         A.CallTo(() => _groupingDefinitionPresenter.Edit(_groupingDefinition)).MustHaveHappened();
      }
   }

   public class When_editing_a_grouping_field_and_the_user_confirms_the_edition : concern_for_EditPopulationAnalysisGroupingFieldPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         sut.Edit(_groupingField, _populationDataCollector);
      }

      [Observation]
      public void should_save_the_grouping_definition()
      {
         A.CallTo(() => _groupingDefinitionPresenter.UpdateGroupingDefinition()).MustHaveHappened();
      }
   }

   public class When_editing_a_grouping_field_and_the_cancels_the_action : concern_for_EditPopulationAnalysisGroupingFieldPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         sut.Edit(_groupingField, _populationDataCollector);
      }

      [Observation]
      public void should_not_the_grouping_definition()
      {
         A.CallTo(() => _groupingDefinitionPresenter.UpdateGroupingDefinition()).MustNotHaveHappened();
      }
   }
}