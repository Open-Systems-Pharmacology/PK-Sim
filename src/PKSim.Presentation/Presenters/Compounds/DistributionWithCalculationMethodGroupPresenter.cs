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
//      private readonly ICompoundVSSPresenter _compoundVSSPresenter;

      public DistributionWithCalculationMethodGroupPresenter(IMultiplePanelView view, IOrganPermeabilityGroupPresenter organPermeabilityGroupPresenter, ICalculationMethodSelectionPresenterForCompound calculationMethodSelectionPresenter, ICompoundVSSPresenter compoundVSSPresenter)
         : base(view, calculationMethodSelectionPresenter, organPermeabilityGroupPresenter)
      {
//         _compoundVSSPresenter = compoundVSSPresenter;
//         AddSubPresenters(_compoundVSSPresenter);
//         _view.ActivateView(_compoundVSSPresenter.BaseView);
         _view.AddEmptyPlaceHolder();
      }

      public override void EditCompound(Compound compound)
      {
         base.EditCompound(compound);
//         _compoundVSSPresenter.EditCompound(compound);
      }

      protected override bool CategoryPredicate(CalculationMethodCategory calculationMethodCategory)
      {
         return calculationMethodCategory.NameIsOneOf(CoreConstants.Category.DistributionCellular, CoreConstants.Category.DistributionInterstitial, CoreConstants.Category.DiffusionIntCell);
      }
   }

}