using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisFieldsArrangementPresenter : ContextSpecification<IPopulationAnalysisFieldsArrangementPresenter>
   {
      protected IPopulationAnalysisFieldListPresenter _allFieldsPresenter;
      protected IPopulationAnalysisFieldListPresenter _rowFieldsPresenter;
      protected IPopulationAnalysisFieldListPresenter _colorFieldsPresenter;
      protected IPopulationAnalysisFieldListPresenter _symbolFieldsPresenter;
      protected IEventPublisher _eventPublisher;
      private IPopulationAnalysisFieldsArrangementView _view;
      protected IPopulationDataCollector _populationDataCollector;
      protected PopulationPivotAnalysis _populationAnalysis;

      protected override void Context()
      {
         _allFieldsPresenter = A.Fake<IPopulationAnalysisFieldListPresenter>();
         _rowFieldsPresenter = A.Fake<IPopulationAnalysisFieldListPresenter>();
         _colorFieldsPresenter = A.Fake<IPopulationAnalysisFieldListPresenter>();
         _symbolFieldsPresenter = A.Fake<IPopulationAnalysisFieldListPresenter>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _view = A.Fake<IPopulationAnalysisFieldsArrangementView>();
         sut = new PopulationAnalysisFieldsArrangementPresenter(_view, _allFieldsPresenter, _rowFieldsPresenter, _colorFieldsPresenter,_symbolFieldsPresenter, _eventPublisher);

         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _populationAnalysis = A.Fake<PopulationPivotAnalysis>();
      }
   }

   public class When_creating_a_field_arrangement_presenter : concern_for_PopulationAnalysisFieldsArrangementPresenter
   {
      [Observation]
      public void should_be_able_to_set_the_description_of_each_field_area_presenter()
      {
         sut.UpdateDescription(PivotArea.FilterArea, "TOTO");
         A.CallTo(() => _allFieldsPresenter.UpdateDescription("TOTO")).MustHaveHappened();
      }

      [Observation]
      public void should_have_set_the_area_in_all_area_presenters()
      {
         _allFieldsPresenter.Area.ShouldBeEqualTo(PivotArea.FilterArea);
         _rowFieldsPresenter.Area.ShouldBeEqualTo(PivotArea.RowArea);
         _colorFieldsPresenter.Area.ShouldBeEqualTo(PivotArea.ColorArea);
         _symbolFieldsPresenter.Area.ShouldBeEqualTo(PivotArea.SymbolArea);
      }

      [Observation]
      public void should_have_updated_the_maximum_number_of_item_count_for_color_and_symbol_to_one()
      {
         _colorFieldsPresenter.MaximNumberOfAllowedFields.ShouldBeEqualTo(1);
         _symbolFieldsPresenter.MaximNumberOfAllowedFields.ShouldBeEqualTo(1);
      }
   }

   public class When_notified_that_some_fields_were_moved_from_an_area_to_another : concern_for_PopulationAnalysisFieldsArrangementPresenter
   {
      private List<IPopulationAnalysisField> _fields;
      private IPopulationAnalysisField _field1;
      private IPopulationAnalysisField _field2;
      private FieldsMovedInPopulationAnalysisEvent _event;

      protected override void Context()
      {
         base.Context();
         _field1 = A.Fake<IPopulationAnalysisField>();
         _field2 = A.Fake<IPopulationAnalysisField>();
         _fields = new List<IPopulationAnalysisField> {_field1, _field2};
         sut.StartAnalysis(_populationDataCollector, _populationAnalysis);
         A.CallTo(() => _eventPublisher.PublishEvent(A<FieldsMovedInPopulationAnalysisEvent>._))
            .Invokes(x => _event = x.GetArgument<FieldsMovedInPopulationAnalysisEvent>(0));
      }

      protected override void Because()
      {
         _rowFieldsPresenter.FieldsMoved += Raise.With(new FieldsMovedEventArgs(_fields, null,PivotArea.RowArea));
      }

      [Observation]
      public void should_change_the_target_area_for_the_field_that_were_moved()
      {
         A.CallTo(() => _populationAnalysis.SetPosition(_field1, PivotArea.RowArea, A<int>._)).MustHaveHappened();
         A.CallTo(() => _populationAnalysis.SetPosition(_field2, PivotArea.RowArea, A<int>._)).MustHaveHappened();
      }

      [Observation]
      public void should_refresh_the_analysis()
      {
         A.CallTo(() => _allFieldsPresenter.RefreshAnalysis()).MustHaveHappened();
         A.CallTo(() => _rowFieldsPresenter.RefreshAnalysis()).MustHaveHappened();
         A.CallTo(() => _colorFieldsPresenter.RefreshAnalysis()).MustHaveHappened();
         A.CallTo(() => _symbolFieldsPresenter.RefreshAnalysis()).MustHaveHappened();
      }

      [Observation]
      public void should_notify_a_move_fields_event()
      {
         _event.ShouldNotBeNull();
         _event.PopulationAnalysisFields.ShouldOnlyContain(_field1, _field2);
      }
   }

   public class When_notified_that_some_fields_were_moved_onto_a_speecifc_target : concern_for_PopulationAnalysisFieldsArrangementPresenter
   {
      private List<IPopulationAnalysisField> _fields;
      private IPopulationAnalysisField _field1;
      private IPopulationAnalysisField _field2;
      private IPopulationAnalysisField _target;

      protected override void Context()
      {
         base.Context();
         _field1 = A.Fake<IPopulationAnalysisField>();
         _field2 = A.Fake<IPopulationAnalysisField>();
         _target = A.Fake<IPopulationAnalysisField>();
         _fields = new List<IPopulationAnalysisField> {_field1, _field2};
         A.CallTo(() => _populationAnalysis.GetAreaIndex(_target)).Returns(3);
         sut.StartAnalysis(_populationDataCollector, _populationAnalysis);
      }

      protected override void Because()
      {
         _rowFieldsPresenter.FieldsMoved += Raise.With(new FieldsMovedEventArgs(_fields, _target, PivotArea.RowArea));
      }

      [Observation]
      public void should_change_the_target_area_for_the_field_that_were_moved_and_set_the_index_based_on_the_index_of_the_target()
      {
         A.CallTo(() => _populationAnalysis.SetPosition(_field1, PivotArea.RowArea, 3 + 1)).MustHaveHappened();
         A.CallTo(() => _populationAnalysis.SetPosition(_field2, PivotArea.RowArea, 3 + 2)).MustHaveHappened();
      }

      [Observation]
      public void should_change_the_index_of_the_target_to_be_after_the_moved_fields()
      {
         A.CallTo(() => _populationAnalysis.SetPosition(_target, PivotArea.RowArea, 3 + 3)).MustHaveHappened();
      }
   }
}