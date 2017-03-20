using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_MultipleNumericFieldsPresenter : ContextSpecification<IMultipleNumericFieldsPresenter>
   {
      protected IMultipleNumericFieldsView _view;
      protected IEventPublisher _eventPublisher;
      protected PopulationPivotAnalysis _populationAnalysis;
      protected PopulationAnalysisNumericField _field1;
      protected PopulationAnalysisNumericField _field2;
      protected PopulationAnalysisNumericField _field3;
      protected IEnumerable<FieldSelectionDTO> _fieldsDTO;
      protected List<IPopulationAnalysisField> _allAllowedFields;
      protected Type _allowedType = typeof (PopulationAnalysisNumericField);
      protected List<IPopulationAnalysisField> _allDataFields;

      protected override void Context()
      {
         _view = A.Fake<IMultipleNumericFieldsView>();
         _eventPublisher = A.Fake<IEventPublisher>();
         sut = new MultipleNumericFieldsPresenter(_view, _eventPublisher);

         _populationAnalysis = A.Fake<PopulationPivotAnalysis>();
         sut.StartAnalysis(A.Fake<IPopulationDataCollector>(), _populationAnalysis);

         _field1 = new PopulationAnalysisParameterField();
         _field2 = new PopulationAnalysisParameterField();
         _field3 = new PopulationAnalysisParameterField();

         _allAllowedFields = new List<IPopulationAnalysisField>();
         _allDataFields = new List<IPopulationAnalysisField>();

         sut.AllowedType = _allowedType;
         A.CallTo(() => _view.BindTo(A<IEnumerable<FieldSelectionDTO>>._))
            .Invokes(x => _fieldsDTO = x.GetArgument<IEnumerable<FieldSelectionDTO>>(0));

         A.CallTo(() => _populationAnalysis.All(_allowedType, false)).Returns(_allAllowedFields);
         A.CallTo(() => _populationAnalysis.AllFieldsOn(PivotArea.DataArea, _allowedType)).Returns(_allDataFields);
      }
   }

   public class When_notified_that_a_field_was_getting_the_focus : concern_for_MultipleNumericFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.StartAnalysis(A.Fake<IPopulationDataCollector>(), _populationAnalysis);
      }

      [Observation]
      public void should_disable_the_up_and_down_button_if_the_field_is_not_selected()
      {
         sut.RefreshAnalysis();
         sut.SelectedItemChanged();
         _view.UpEnabled.ShouldBeFalse();
         _view.DownEnabled.ShouldBeFalse();
      }

      [Observation]
      public void should_disable_the_up_button_and_enable_the_down_button_if_the_field_is_the_first_in_the_list()
      {
         _allDataFields.AddRange(new[] {_field1, A.Fake<PopulationAnalysisNumericField>()});
         sut.RefreshAnalysis();
         _view.SelectedItem = _fieldsDTO.First();
         sut.SelectedItemChanged();
         _view.UpEnabled.ShouldBeFalse();
         _view.DownEnabled.ShouldBeTrue();
      }

      [Observation]
      public void should_disable_the_down_button_and_enable_the_up_button_if_the_field_is_the_last_in_the_list()
      {
         _allDataFields.AddRange(new[] {A.Fake<PopulationAnalysisNumericField>(), _field1});
         sut.RefreshAnalysis();
         _view.SelectedItem = _fieldsDTO.Last();
         sut.SelectedItemChanged();
         _view.UpEnabled.ShouldBeTrue();
         _view.DownEnabled.ShouldBeFalse();
      }
   }

   public class When_moving_the_selected_field_up : concern_for_MultipleNumericFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _allAllowedFields.AddRange(new[] {_field1, _field2, _field3});
         _allDataFields.AddRange(new[] {_field1, _field3});
         sut.RefreshAnalysis();
      }

      protected override void Because()
      {
         //move the dto for field 3
         sut.MoveUp(_fieldsDTO.First(x => x.PopulationAnalysisField == _field3));
      }

      [Observation]
      public void should_find_the_index_of_the_previous_selected_field_and_swap_them_out()
      {
         A.CallTo(() => _populationAnalysis.SetPosition(_field3, A<PivotArea>._, 0)).MustHaveHappened();
         A.CallTo(() => _populationAnalysis.SetPosition(_field1, A<PivotArea>._, 1)).MustHaveHappened();
      }
   }

   public class When_moving_the_selected_field_down : concern_for_MultipleNumericFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _allAllowedFields.AddRange(new[] {_field1, _field2, _field3});
         _allDataFields.AddRange(new[] {_field1, _field3});
         sut.RefreshAnalysis();
      }

      protected override void Because()
      {
         //move the dto for field 1
         sut.MoveDown(_fieldsDTO.First(x => x.PopulationAnalysisField == _field1));
      }

      [Observation]
      public void should_find_the_index_of_the_previous_selected_field_and_swap_them_out()
      {
         A.CallTo(() => _populationAnalysis.SetPosition(_field1, A<PivotArea>._, 1)).MustHaveHappened();
         A.CallTo(() => _populationAnalysis.SetPosition(_field3, A<PivotArea>._, 0)).MustHaveHappened();
      }
   }
}