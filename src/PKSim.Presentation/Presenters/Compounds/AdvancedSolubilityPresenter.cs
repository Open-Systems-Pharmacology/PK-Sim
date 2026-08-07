using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Views;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IAdvancedSolubilityPresenter : ICompoundParameterGroupPresenter
   {
   }

   public class AdvancedSolubilityPresenter : MultiplePanelPresenter, IAdvancedSolubilityPresenter
   {
      private readonly IAdvancedSolubilityConstantsGroupPresenter _constantsGroupPresenter;
      private readonly IBileSaltPartitionCoefficientGroupPresenter _bileSaltPartitionCoefficientGroupPresenter;

      public AdvancedSolubilityPresenter(
         IMultiplePanelView view,
         IAdvancedSolubilityConstantsGroupPresenter constantsGroupPresenter,
         IBileSaltPartitionCoefficientGroupPresenter bileSaltPartitionCoefficientGroupPresenter)
         : base(view, constantsGroupPresenter, bileSaltPartitionCoefficientGroupPresenter)
      {
         _constantsGroupPresenter = constantsGroupPresenter;
         _bileSaltPartitionCoefficientGroupPresenter = bileSaltPartitionCoefficientGroupPresenter;
         _view.Note = PKSimConstants.UI.CompoundAdvancedSolubilityParametersNote;
         _view.AddEmptyPlaceHolder();
      }

      public void EditCompound(Compound compound)
      {
         _constantsGroupPresenter.EditCompound(compound);
         _bileSaltPartitionCoefficientGroupPresenter.EditCompound(compound);
      }
   }
}
