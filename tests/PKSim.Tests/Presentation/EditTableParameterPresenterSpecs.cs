using FakeItEasy;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters.Charts;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditTableParameterPresenter : ContextSpecification<IEditTableParameterPresenter>
   {
      protected IEditTableParameterView _view;
      protected ITableParameterPresenter _tableParameterPresenter;
      private IFullPathDisplayResolver _fullPathDisplayResolver;
      protected ISimpleChartPresenter _chartPresenter;

      protected override void Context()
      {
         _view = A.Fake<IEditTableParameterView>();
         _tableParameterPresenter = A.Fake<ITableParameterPresenter>();
         _fullPathDisplayResolver = A.Fake<IFullPathDisplayResolver>();
         _chartPresenter = A.Fake<ISimpleChartPresenter>();
         sut = new EditTableParameterPresenter(_view, _tableParameterPresenter, _fullPathDisplayResolver, _chartPresenter);
      }
   }

   public class When_starting_the_edit_table_presenter : concern_for_EditTableParameterPresenter
   {
      [Observation]
      public void should_set_the_table_presenter_view_into_the_main_view()
      {
         A.CallTo(() => _view.AddView(_tableParameterPresenter.BaseView)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_chart_presenter_view_into_the_main_view()
      {
         A.CallTo(() => _view.AddChart(_chartPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_the_edited_table_was_changed : concern_for_EditTableParameterPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _tableParameterPresenter.EditedFormula).Returns(new TableFormula());
      }

      protected override void Because()
      {
         _tableParameterPresenter.StatusChanged += Raise.WithEmpty();
      }

      [Observation]
      public void should_refresh_the_table_chart()
      {
         A.CallTo(() => _chartPresenter.Plot(_tableParameterPresenter.EditedFormula)).MustHaveHappened();
      }
   }

   public class When_editing_a_parameter_that_is_read_only : concern_for_EditTableParameterPresenter
   {
      private IParameter _parameter;
      private bool _result;

      protected override void Context()
      {
         base.Context();
         _parameter = new Parameter { Editable = false };
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.Edit(_parameter);
      }

      [Observation]
      public void should_hide_the_cancel_button()
      {
         _view.CancelVisible.ShouldBeFalse();
      }

      [Observation]
      public void should_always_return_false_even_if_the_view_returns_ok()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_editing_a_parameter_that_is_editable_and_the_edit_is_canceled : concern_for_EditTableParameterPresenter
   {
      private IParameter _parameter;
      private bool _result;

      protected override void Context()
      {
         base.Context();
         _parameter = new Parameter { Editable = true };
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.Edit(_parameter);
      }

      [Observation]
      public void should_not_hide_the_cancel_button()
      {
         _view.CancelVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_editing_a_parameter_that_is_editable_and_the_edit_is_accepted : concern_for_EditTableParameterPresenter
   {
      private IParameter _parameter;
      private bool _result;

      protected override void Context()
      {
         base.Context();
         _parameter = new Parameter { Editable = true };
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.Edit(_parameter);
      }

      [Observation]
      public void should_not_hide_the_cancel_button()
      {
         _view.CancelVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true()
      {
         _result.ShouldBeTrue();
      }
   }
}
