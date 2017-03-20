using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Formulations;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Formulations;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_TableFormulationPresenter : ContextSpecification<ITableFormulationPresenter>
   {
      protected IMultiParameterEditPresenter _formulationParametersPresenter;
      protected ITableFormulationParameterPresenter _tableParameterPresenter;
      protected ITableFormulationView _view;

      protected override void Context()
      {
         _formulationParametersPresenter = A.Fake<IMultiParameterEditPresenter>();
         _view = A.Fake<ITableFormulationView>();
         _tableParameterPresenter = A.Fake<ITableFormulationParameterPresenter>();
         sut = new TableFormulationPresenter(_view, _tableParameterPresenter, _formulationParametersPresenter);
      }
   }

   public class When_the_table_formulation_presenter_is_editing_a_table_formulation : concern_for_TableFormulationPresenter
   {
      private Formulation _formulation;
      private IParameter _tableParameter;
      private IParameter _otherParameter;
      private List<IParameter> _allParameters;

      protected override void Context()
      {
         base.Context();
         _tableParameter = new Parameter().WithName(CoreConstants.Parameter.FRACTION_DOSE);
         _otherParameter = new Parameter().WithName("OTHER PARAMETERS");
         _formulation = new Formulation {_otherParameter, _tableParameter};
         A.CallTo(() => _formulationParametersPresenter.Edit(A<IEnumerable<IParameter>>._))
            .Invokes(x => _allParameters = x.GetArgument<IEnumerable<IParameter>>(0).ToList());
      }

      protected override void Because()
      {
         sut.Edit(_formulation);
      }

      [Observation]
      public void should_add_the_table_view_and_the_parameter_view_to_the_presenter_view()
      {
         A.CallTo(() => _view.AddParametersView(_formulationParametersPresenter.BaseView)).MustHaveHappened();
         A.CallTo(() => _view.AddTableView(_tableParameterPresenter.BaseView)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_table_parameter_with_the_table_parameter_presenter()
      {
         A.CallTo(() => _tableParameterPresenter.Edit(_tableParameter)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_other_parameters_with_the_edit_parameter_presenter()
      {
         _allParameters.ShouldOnlyContain(_otherParameter);
      }
   }

   public class When_the_table_formulation_presenter_is_notified_that_the_table_formula_has_changed : concern_for_TableFormulationPresenter
   {
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         sut.TableFormulaChanged += (o, e) =>
         {
            _eventRaised = true;
         };
      }

      protected override void Because()
      {
         _tableParameterPresenter.StatusChanged += Raise.With(EventArgs.Empty);
      }

      [Observation]
      public void should_raise_the_table_formula_changed_event()
      {
         _eventRaised.ShouldBeTrue();
      }
   }

}