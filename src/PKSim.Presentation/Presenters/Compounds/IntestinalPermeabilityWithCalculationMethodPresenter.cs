using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IIntestinalPermeabilityWithCalculationMethodPresenter : ICompoundParameterGroupPresenter
   {
   }

   public class IntestinalPermeabilityWithCalculationMethodPresenter : 
      PermeabilityWithCalculationMethodsGroupPresenter<IIntestinalPermeabilityGroupPresenter>, 
      IIntestinalPermeabilityWithCalculationMethodPresenter
   {
      public IntestinalPermeabilityWithCalculationMethodPresenter(
         IMultiplePanelView view, 
         IIntestinalPermeabilityGroupPresenter intestinalPermeabilityGroupPresenter, 
         ICalculationMethodSelectionPresenterForCompound calculationMethodSelectionPresenter) : base(view, calculationMethodSelectionPresenter, intestinalPermeabilityGroupPresenter)
      {
         _view.AddEmptyPlaceHolder();
      }

      protected override bool CategoryPredicate(CalculationMethodCategory calculationMethodCategory)
      {
         return calculationMethodCategory.NameIsOneOf(CoreConstants.Category.IntestinalPermeability);
      }
   }
}