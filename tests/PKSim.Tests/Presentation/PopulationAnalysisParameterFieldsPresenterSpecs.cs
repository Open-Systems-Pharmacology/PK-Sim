using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisParameterFieldsPresenter : ContextSpecification<IPopulationAnalysisParameterFieldsPresenter>
   {
      private IEntitiesInContainerRetriever _quantitiesRetriever;
      private IPopulationAnalysesContextMenuFactory _contextMenuFactory;
      protected PopulationPivotAnalysis _populationPivotAnalysis;
      private IPopulationDataCollector _populationDataCollector;
      private PathCache<IParameter> _parameterCache;
      protected IParameter _parameter;
      private IPopulationAnalysisFieldFactory _populationAnalysisFieldFactory;
      protected PopulationAnalysisParameterField _parameterField;
      protected IPopulationAnalysisFieldsView _view;
      private IEventPublisher _eventPublisher;
      private IPopulationAnalysisGroupingFieldCreator _populationAnalysisGroupingFieldCreator;
      private IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;
      protected IDialogCreator _dialogCreator;
      protected PopulationAnalysisDerivedField _derivedField;
      private IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper _fieldDTOMapper;
      protected const string _parameterPath = "ParameterPath";

      protected override void Context()
      {
         _view = A.Fake<IPopulationAnalysisFieldsView>();
         _quantitiesRetriever = A.Fake<IEntitiesInContainerRetriever>();
         _contextMenuFactory = A.Fake<IPopulationAnalysesContextMenuFactory>();
         _populationAnalysisFieldFactory = A.Fake<IPopulationAnalysisFieldFactory>();
         _populationAnalysisTemplateTask = A.Fake<IPopulationAnalysisTemplateTask>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _fieldDTOMapper= A.Fake<IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper>();
         _parameterCache = new PathCacheForSpecs<IParameter>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _populationAnalysisGroupingFieldCreator = A.Fake<IPopulationAnalysisGroupingFieldCreator>();
         sut = new PopulationAnalysisParameterFieldsPresenter(_view, _contextMenuFactory, _quantitiesRetriever, _populationAnalysisFieldFactory,
            _eventPublisher, _populationAnalysisGroupingFieldCreator, _populationAnalysisTemplateTask, _dialogCreator, _fieldDTOMapper);

         A.CallTo(_quantitiesRetriever).WithReturnType<PathCache<IParameter>>().Returns(_parameterCache);
         _populationPivotAnalysis = new PopulationPivotAnalysis();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);

         _parameter = A.Fake<IParameter>();
         _parameterField = new PopulationAnalysisParameterField {ParameterPath = _parameterPath, Name = "ParameterField"};
         A.CallTo(() => _populationAnalysisFieldFactory.CreateFor(_parameter)).Returns(_parameterField);

         _derivedField = A.Fake<PopulationAnalysisDerivedField>();
         A.CallTo(() => _derivedField.ReferencedFieldNames).Returns(new[] {_parameterField.Name});

         _parameterCache.Add(_parameterPath, _parameter);
      }
   }

   public class When_adding_a_parameter_to_the_population_analyses : concern_for_PopulationAnalysisParameterFieldsPresenter
   {
      protected override void Because()
      {
         sut.AddParameter(_parameter);
      }

      [Observation]
      public void should_create_a_population_analyses_field_for_the_given_parameter_and_add_it_to_the_analyses()
      {
         var parameterField = _populationPivotAnalysis.All<PopulationAnalysisParameterField>().First();
         parameterField.ShouldBeEqualTo(_parameterField);
      }
   }

   public class When_removing_a_field_from_which_derived_field_are_depending : concern_for_PopulationAnalysisParameterFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _populationPivotAnalysis.Add(_parameterField);
         _populationPivotAnalysis.Add(_derivedField);
      }

      protected override void Because()
      {
         sut.RemoveField(_parameterField);
      }

      [Observation]
      public void should_warn_the_user_that_all_dervied_fields_will_be_deleted_as_well()
      {
         A.CallTo(_dialogCreator).MustHaveHappened();
      }
   }

   public class When_removing_a_field_from_which_derived_field_are_depending_and_the_user_cancels_deleting : concern_for_PopulationAnalysisParameterFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _populationPivotAnalysis.Add(_parameterField);
         _populationPivotAnalysis.Add(_derivedField);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.No);
      }

      protected override void Because()
      {
         sut.RemoveField(_parameterField);
      }

      [Observation]
      public void should_not_remove_the_fields()
      {
         _populationPivotAnalysis.Has(_parameterField).ShouldBeTrue();
         _populationPivotAnalysis.Has(_derivedField).ShouldBeTrue();
      }
   }

   public class When_removing_a_field_from_which_derived_field_are_depending_and_the_user_accepts_the_deletion_of_derived_fields : concern_for_PopulationAnalysisParameterFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _populationPivotAnalysis.Add(_parameterField);
         _populationPivotAnalysis.Add(_derivedField);
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         sut.RemoveField(_parameterField);
      }

      [Observation]
      public void should_have_removed_the_field_and_the_deried_fields()
      {
         _populationPivotAnalysis.Has(_parameterField).ShouldBeFalse();
         _populationPivotAnalysis.Has(_derivedField).ShouldBeFalse();
      }
   }

   public class When_notified_that_a_parameter_field_was_selected : concern_for_PopulationAnalysisParameterFieldsPresenter
   {
      private IPopulationAnalysisField _selectedField;
      private bool _raised;
      private IParameter _selectedParameter;

      protected override void Context()
      {
         base.Context();
         _selectedField = new PopulationAnalysisParameterField {ParameterPath = _parameterPath};
         sut.ParameterFieldSelected += (o, e) =>
         {
            _raised = true;
            _selectedParameter = e.Parameter;
         };
         sut.AddParameter(_parameter);
      }

      protected override void Because()
      {
         sut.FieldSelected(_selectedField);
      }

      [Observation]
      public void should_raise_the_parameter_selected_event()
      {
         _raised.ShouldBeTrue();
         _selectedParameter.ShouldBeEqualTo(_parameter);
      }

      [Observation]
      public void should_enable_the_grouping_element()
      {
         _view.CreateGroupingButtonEnabled.ShouldBeTrue();
      }
   }

   public class When_notified_that_a_covariate_field_was_selected : concern_for_PopulationAnalysisParameterFieldsPresenter
   {
      private IPopulationAnalysisField _selectedField;
      private bool _raised;
      private string _covariate;

      protected override void Context()
      {
         base.Context();
         _selectedField = new PopulationAnalysisCovariateField {Covariate = "TOTO"};
         sut.CovariateFieldSelected += (o, e) =>
         {
            _raised = true;
            _covariate = e.Covariate;
         };
         sut.AddParameter(_parameter);
      }

      protected override void Because()
      {
         sut.FieldSelected(_selectedField);
      }

      [Observation]
      public void should_raise_the_covariate_selected_event()
      {
         _raised.ShouldBeTrue();
         _covariate.ShouldBeEqualTo("TOTO");
      }

      [Observation]
      public void should_enable_the_grouping_element()
      {
         _view.CreateGroupingButtonEnabled.ShouldBeTrue();
      }
   }

   public class When_notified_that_a_derived_field_was_selected : concern_for_PopulationAnalysisParameterFieldsPresenter
   {
      private IPopulationAnalysisField _selectedField;

      private bool _raised;

      protected override void Context()
      {
         base.Context();
         _selectedField = new PopulationAnalysisGroupingField(new NumberOfBinsGroupingDefinition("Field") { NumberOfBins = 5 });
         sut.DerivedFieldSelected += (o, e) =>
         {
            _raised = true;
            _derivedField = e.DerivedField;
         };
      }

      protected override void Because()
      {
         sut.FieldSelected(_selectedField);
      }

      [Observation]
      public void should_disable_the_grouping_element()
      {
         _view.CreateGroupingButtonEnabled.ShouldBeFalse();
      }

      [Observation]
      public void should_raise_the_derived_field_selected_event()
      {
         _raised.ShouldBeTrue();
         _derivedField.ShouldBeEqualTo(_selectedField);
      }
   }

   public class When_asked_for_the_selected_field : concern_for_PopulationAnalysisParameterFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         var parameterField = A.Fake<PopulationAnalysisParameterField>();
         parameterField.ParameterPath = _parameterPath;
         _view.SelectedField = new PopulationAnalysisFieldDTO(parameterField);
      }

      [Observation]
      public void should_return_the_field_selected_by_the_user()
      {
         sut.SelectedParameter.ShouldBeEqualTo(_parameter);
      }
   }
}