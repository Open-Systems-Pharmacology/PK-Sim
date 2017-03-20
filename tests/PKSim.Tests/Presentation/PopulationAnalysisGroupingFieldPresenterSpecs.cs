using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Validation;
using FakeItEasy;
using NUnit.Framework;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;


namespace PKSim.Presentation
{
   public abstract class concern_for_CreatePopulationAnalysisGroupingFieldPresenter : ContextSpecification<ICreatePopulationAnalysisGroupingFieldPresenter>
   {
      protected IGroupingDefinitionToGroupingDefinitionPresenterMapper _groupingDefinitionPresenterMapper;
      protected ICreatePopulationAnalysisGroupingFieldView _view;

      protected IPopulationAnalysisField _field;
      protected IPopulationDataCollector _populationDataCollector;
      protected PopulationAnalysis _populationAnalysis;
      protected readonly List<IPopulationAnalysisField> _existingFields = new List<IPopulationAnalysisField>();
      protected GroupingFieldDTO _groupingFieldDTO;
      private IEventPublisher _eventPublisher;

      protected override void Context()
      {
         _groupingDefinitionPresenterMapper = A.Fake<IGroupingDefinitionToGroupingDefinitionPresenterMapper>();
         _view = A.Fake<ICreatePopulationAnalysisGroupingFieldView>();
         _eventPublisher = A.Fake<IEventPublisher>();

         _field = A.Fake<PopulationAnalysisNumericField>();
         _populationAnalysis = A.Fake<PopulationAnalysis>();
         _field.PopulationAnalysis = _populationAnalysis;
         A.CallTo(() => _populationAnalysis.AllFields).Returns(_existingFields);
         _populationDataCollector = A.Fake<IPopulationDataCollector>();

         sut = new CreatePopulationAnalysisGroupingFieldPresenter(_view, _groupingDefinitionPresenterMapper,_eventPublisher);

         A.CallTo(() => _view.BindTo(A<GroupingFieldDTO>._))
            .Invokes(x => _groupingFieldDTO = x.GetArgument<GroupingFieldDTO>(0));
      }
   }

   public class When_creating_a_grouping_definition_for_a_numerical_field : concern_for_CreatePopulationAnalysisGroupingFieldPresenter
   {
      private PopulationAnalysisParameterField _existingField;

      protected override void Context()
      {
         base.Context();
         _field.Name = "NEW NAME";
         _existingField = new PopulationAnalysisParameterField {Name = "Existing Field"};
         _existingFields.Add(_existingField);
      }

      protected override void Because()
      {
         sut.CreateGrouping(_field, _populationDataCollector);
      }

      [Observation]
      public void should_ensure_that_existing_name_cannot_be_used()
      {
         _groupingFieldDTO.Validate(x => x.Name, _existingField.Name).IsEmpty.ShouldBeFalse();
      }

      [Observation]
      public void should_start_the_view_with_the_default_settings()
      {
         A.CallTo(() => _view.BindTo(A<GroupingFieldDTO>._)).MustHaveHappened();
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_caption_using_the_name_of_the_field_for_which_a_derived_field_is_created()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.CreateGroupingForField(_field.Name));
      }

      [Observation]
      public void the_returned_field_name_should_be_the_one_entered_by_the_user()
      {
         _groupingFieldDTO.Name = "A GREAT NAME";
         sut.FieldName.ShouldBeEqualTo(_groupingFieldDTO.Name);
      }
   }

   public class When_the_grouping_definition_presenter_is_being_notified_that_the_user_selected_a_new_grouping_method : concern_for_CreatePopulationAnalysisGroupingFieldPresenter
   {
      private IGroupingDefinitionPresenter _groupingDefinitionPresenter;

      protected override void Context()
      {
         base.Context();
         sut.CreateGrouping(_field, _populationDataCollector);
         _groupingDefinitionPresenter = A.Fake<IGroupingDefinitionPresenter>();
         A.CallTo(() => _groupingDefinitionPresenterMapper.MapFrom(GroupingDefinitions.NumberOfBins)).Returns(_groupingDefinitionPresenter);
      }

      protected override void Because()
      {
         //Simulate change of definition
         _groupingFieldDTO.GroupingDefinitionItem = GroupingDefinitions.NumberOfBins;
         sut.SelectedGroupingChanged();
      }

      [Observation]
      public void should_retrieve_the_presenter_defined_for_this_grouping_method_and_initialize_it()
      {
         A.CallTo(() => _groupingDefinitionPresenter.InitializeWith(_field, _populationDataCollector)).MustHaveHappened();
      }
   }

   public class When_the_grouping_definition_presenter_is_asked_if_it_can_be_closed : concern_for_CreatePopulationAnalysisGroupingFieldPresenter
   {
      private IGroupingDefinitionPresenter _groupingDefinitionPresenter;

      protected override void Context()
      {
         base.Context();
         _groupingDefinitionPresenter = A.Fake<IGroupingDefinitionPresenter>();
         A.CallTo(() => _groupingDefinitionPresenterMapper.MapFrom(GroupingDefinitions.FixedLimits)).Returns(_groupingDefinitionPresenter);
         sut.CreateGrouping(_field, _populationDataCollector);
      }

      [Observation]
      [TestCase(true, true, false)]
      [TestCase(false, false, true)]
      [TestCase(true, false, false)]
      [TestCase(false, true, false)]
      public void should_return_false_if_the_active_groupind_definition_presenter_has_any_errors(bool viewHasError, bool activePresenterHasError, bool canClose)
      {
         A.CallTo(() => _groupingDefinitionPresenter.CanClose).Returns(!activePresenterHasError);
         A.CallTo(() => _view.HasError).Returns(viewHasError);
         sut.CanClose.ShouldBeEqualTo(canClose);
      }
   }

   public class When_the_active_grouping_definition_presenter_is_notifying_some_change : concern_for_CreatePopulationAnalysisGroupingFieldPresenter
   {
      private IGroupingDefinitionPresenter _groupingDefinitionPresenter;
      private bool _raised;

      protected override void Context()
      {
         base.Context();
         _groupingDefinitionPresenter = A.Fake<IGroupingDefinitionPresenter>();
         A.CallTo(() => _groupingDefinitionPresenterMapper.MapFrom(GroupingDefinitions.FixedLimits)).Returns(_groupingDefinitionPresenter);
         sut.CreateGrouping(_field, _populationDataCollector);
         sut.StatusChanged += (o, e) => { _raised = true; };
      }

      protected override void Because()
      {
         _groupingDefinitionPresenter.StatusChanged += Raise.WithEmpty();
      }

      [Observation]
      public void it_should_forward_the_change_notification()
      {
         _raised.ShouldBeTrue();
      }
   }

   public class When_starting_a_grouping_for_a_numeric_field : concern_for_CreatePopulationAnalysisGroupingFieldPresenter
   {
      private List<GroupingDefinitionItem> _availableGrouping;
      private IPopulationAnalysisField _numericField;

      protected override void Context()
      {
         base.Context();
         _numericField = A.Fake<PopulationAnalysisNumericField>();
         _numericField.PopulationAnalysis = _populationAnalysis;
         sut.CreateGrouping(_numericField, _populationDataCollector);
      }

      protected override void Because()
      {
         _availableGrouping = sut.AvailableGroupings.ToList();
      }

      [Observation]
      public void should_allow_the_grouping_definition_for_numeric_type_only()
      {
         _availableGrouping.ShouldOnlyContain(GroupingDefinitions.FixedLimits,  GroupingDefinitions.NumberOfBins);
      }
   }

   public class When_starting_a_grouping_for_a_non_numeric_field : concern_for_CreatePopulationAnalysisGroupingFieldPresenter
   {
      private List<GroupingDefinitionItem> _availableGrouping;
      private IPopulationAnalysisField _covariateField;

      protected override void Context()
      {
         base.Context();
         _covariateField = A.Fake<PopulationAnalysisCovariateField>();
         _covariateField.PopulationAnalysis = _populationAnalysis;
         sut.CreateGrouping(_covariateField, _populationDataCollector);
      }

      protected override void Because()
      {
         _availableGrouping = sut.AvailableGroupings.ToList();
      }

      [Observation]
      public void should_allow_the_grouping_definition_for_non_numeric_type_only()
      {
         _availableGrouping.ShouldOnlyContain(GroupingDefinitions.ValueMapping);
      }
   }
}