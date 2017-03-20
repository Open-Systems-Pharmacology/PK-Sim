using PKSim.Core.Model;
using PKSim.Presentation.Views;

namespace PKSim.Presentation.Presenters.Compounds
{
   public abstract class PermeabilityWithCalculationMethodsGroupPresenter<TPermeabilityPresenter> : MultiplePanelPresenter
      where TPermeabilityPresenter : IPermeabilityGroupPresenter
   {
      private readonly TPermeabilityPresenter _permeabilityGroupPresenter;
      private readonly ICalculationMethodSelectionPresenterForCompound _calculationMethodSelectionPresenter;

      protected PermeabilityWithCalculationMethodsGroupPresenter(IMultiplePanelView view, 
         ICalculationMethodSelectionPresenterForCompound calculationMethodSelectionPresenter, 
         TPermeabilityPresenter permeabilityGroupPresenter)
         : base(view, calculationMethodSelectionPresenter, permeabilityGroupPresenter)
      {
         _permeabilityGroupPresenter = permeabilityGroupPresenter;
         _calculationMethodSelectionPresenter = calculationMethodSelectionPresenter;
      }

      public virtual void EditCompound(Compound compound)
      {
         _permeabilityGroupPresenter.EditCompound(compound);
         _calculationMethodSelectionPresenter.Edit(compound, CategoryPredicate);

         if (!_calculationMethodSelectionPresenter.AnyCategories())
            _view.HideView(_calculationMethodSelectionPresenter.BaseView);
      }

      protected abstract bool CategoryPredicate(CalculationMethodCategory calculationMethodCategory);
   }
}