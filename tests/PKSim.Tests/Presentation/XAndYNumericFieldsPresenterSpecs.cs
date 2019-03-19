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
   public abstract class concern_for_XAndYNumericFieldsPresenter : ContextSpecification<IXAndYNumericFieldsPresenter>
   {
      protected IXAndYNumericFieldsView _view;
      private IPopulationDataCollector _populationDataCollector;
      protected PopulationPivotAnalysis _populationAnalysis;
      private IEventPublisher _eventPublisher;
      protected XandYFieldsSelectionDTO _dto;
      protected List<IPopulationAnalysisField> _allDataFields;
      protected List<IPopulationAnalysisField> _allNumericFields;
      protected IPopulationAnalysisField _field1;
      protected IPopulationAnalysisField _field2;
      protected IPopulationAnalysisField _field3;

      protected override void Context()
      {
         _view = A.Fake<IXAndYNumericFieldsView>();
         _eventPublisher = A.Fake<IEventPublisher>();
         sut = new XAndYNumericFieldsPresenter(_view, _eventPublisher);

         //common context for all tests
         A.CallTo(() => _view.BindTo(A<XandYFieldsSelectionDTO>._))
            .Invokes(x => _dto = x.GetArgument<XandYFieldsSelectionDTO>(0));

         _field1 = A.Fake<PopulationAnalysisNumericField>();
         _field2 = A.Fake<PopulationAnalysisNumericField>();
         _field3 = A.Fake<PopulationAnalysisNumericField>();

         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _populationAnalysis = A.Fake<PopulationPivotAnalysis>();

         _allDataFields = new List<IPopulationAnalysisField>();
         _allNumericFields = new List<IPopulationAnalysisField>();

         A.CallTo(() => _populationAnalysis.AllFieldsOn(PivotArea.DataArea)).Returns(_allDataFields);
         A.CallTo(() => _populationAnalysis.All(typeof (INumericValueField), false)).Returns(_allNumericFields);

         sut.StartAnalysis(_populationDataCollector, _populationAnalysis);
      }
   }

   public class When_refreshing_an_analysis_containing_defined_X_and_Y_values_as_data : concern_for_XAndYNumericFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _allDataFields.AddRange(new[] {_field1, _field3});
         _allNumericFields.AddRange(new[] {_field1, _field2, _field3});
      }

      protected override void Because()
      {
         sut.RefreshAnalysis();
      }

      [Observation]
      public void should_retrieve_all_numeric_fields_defined_in_the_analysis_having_the_predefined_type()
      {
         A.CallTo(() => _populationAnalysis.All(typeof (INumericValueField), false)).MustHaveHappened();
      }

      [Observation]
      public void should_use_the_predefined_X_and_Y_fields_in_the_view()
      {
         _dto.X.ShouldBeEqualTo(_field1);
         _dto.Y.ShouldBeEqualTo(_field3);
      }
   }

   public class When_refreshing_an_analysis_containing_only_X_values_as_data : concern_for_XAndYNumericFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _allDataFields.AddRange(new[] {_field3});
         _allNumericFields.AddRange(new[] {_field1, _field2, _field3});
      }

      protected override void Because()
      {
         sut.RefreshAnalysis();
      }

      [Observation]
      public void should_use_the_defined_field_for_X_and_set_an_undefined_value_for_Y()
      {
         _dto.X.ShouldBeEqualTo(_field3);
         _dto.Y.ShouldBeAnInstanceOf<NullNumericField>();
      }
   }

   public class When_refreshing_an_analysis_containing_no_data_values : concern_for_XAndYNumericFieldsPresenter
   {
      protected override void Context()
      {
         base.Context();
         _allNumericFields.AddRange(new[] {_field1, _field2, _field3});
      }

      protected override void Because()
      {
         sut.RefreshAnalysis();
      }

      [Observation]
      public void should_use_the_default_fields_for_X_and_Y()
      {
         _dto.X.ShouldBeEqualTo(_field1);
         _dto.Y.ShouldBeEqualTo(_field2);
      }

      [Observation]
      public void should_have_updated_the_analysis_to_reflect_this_changed()
      {
         A.CallTo(() => _populationAnalysis.SetPosition(_field1,PivotArea.DataArea,0)).MustHaveHappened();
         A.CallTo(() => _populationAnalysis.SetPosition(_field2,PivotArea.DataArea,1)).MustHaveHappened();
      }
   }

   public class When_refreshing_an_analysis_containing_no_numeric_values_whatsoever : concern_for_XAndYNumericFieldsPresenter
   {
      protected override void Because()
      {
         sut.RefreshAnalysis();
      }

      [Observation]
      public void should_set_an_undefined_value_for_X_and_Y()
      {
         _dto.X.ShouldBeAnInstanceOf<NullNumericField>();
         _dto.Y.ShouldBeAnInstanceOf<NullNumericField>();
      }
   }

   public class When_retrieving_the_list_of_all_available_fields : concern_for_XAndYNumericFieldsPresenter
   {
      private List<IPopulationAnalysisField> _availableFields;

      protected override void Context()
      {
         base.Context();
         _allNumericFields.AddRange(new[] {_field1, _field2, _field3});
         sut.RefreshAnalysis();
      }

      protected override void Because()
      {
         _availableFields = sut.AllAvailableFields().ToList();
      }

      [Observation]
      public void should_return_all_numeric_fields_and_the_no_selection_entry_field()
      {
         _availableFields.Count.ShouldBeEqualTo(_allNumericFields.Count + 1);
         _availableFields.ShouldContain(_field1, _field2, _field3);
         _availableFields[0].ShouldBeAnInstanceOf<NullNumericField>();
      }
   }
}