using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IDistributionWithCalculationMethodGroupPresenter : ICompoundParameterGroupPresenter
   {
   }

   //Code commented out until VSS implementation is right
   //see YT issue 47-6691 Hide VSS Calculation for all species as feature was not specified properly
   public class DistributionWithCalculationMethodGroupPresenter : PermeabilityWithCalculationMethodsGroupPresenter<IOrganPermeabilityGroupPresenter>, IDistributionWithCalculationMethodGroupPresenter
   {

      public DistributionWithCalculationMethodGroupPresenter(IMultiplePanelView view, IOrganPermeabilityGroupPresenter organPermeabilityGroupPresenter, ICalculationMethodSelectionPresenterForCompound calculationMethodSelectionPresenter)
         : base(view, calculationMethodSelectionPresenter, organPermeabilityGroupPresenter)
      {
         _view.AddEmptyPlaceHolder();
      }

      protected override bool CategoryPredicate(CalculationMethodCategory calculationMethodCategory)
      {
         return calculationMethodCategory.NameIsOneOf(CoreConstants.Category.DistributionCellular, CoreConstants.Category.DistributionInterstitial, CoreConstants.Category.DiffusionIntCell);
      }
   }

}