using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_IntestinalPermeabilityWithCalculationMethodPresenter : ContextSpecification<IIntestinalPermeabilityWithCalculationMethodPresenter>
   {
      protected IIntestinalPermeabilityGroupPresenter _intestinalPermeabilityPresenter;
      protected ICalculationMethodSelectionPresenterForCompound _calculationMethodSelectionPresenter;
      private IMultiplePanelView _view;

      protected override void Context()
      {
         _intestinalPermeabilityPresenter = A.Fake<IIntestinalPermeabilityGroupPresenter>();
         _calculationMethodSelectionPresenter = A.Fake<ICalculationMethodSelectionPresenterForCompound>();
         _view = A.Fake<IMultiplePanelView>();
         sut = new IntestinalPermeabilityWithCalculationMethodPresenter(_view, _intestinalPermeabilityPresenter, _calculationMethodSelectionPresenter);
      }
   }

   public class When_editing_the_intestinal_permeability_parameters_defined_in_a_given_compound : concern_for_IntestinalPermeabilityWithCalculationMethodPresenter
   {
      private Compound _compound;
      private Func<CalculationMethodCategory, bool> _predicate;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         A.CallTo(() => _calculationMethodSelectionPresenter.Edit(A<Compound>._, A<Func<CalculationMethodCategory, bool>>._))
            .Invokes(x => _predicate = x.GetArgument<Func<CalculationMethodCategory, bool>>(1));
      }

      protected override void Because()
      {
         sut.EditCompound(_compound);
      }

      [Observation]
      public void should_edit_the_calculation_methods_for_intestinal_permeability_only()
      {
         _predicate(new CalculationMethodCategory().WithName(CoreConstants.Category.IntestinalPermeability)).ShouldBeTrue();
      }

      [Observation]
      public void should_edit_the_intestinal_permeability()
      {
         A.CallTo(() => _intestinalPermeabilityPresenter.EditCompound(_compound)).MustHaveHappened();
      }
   }
}