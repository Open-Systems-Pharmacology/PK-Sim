using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface IEditExpressionProfilePresenter : IEditBuildingBockPresenter<ExpressionProfile>
   {
   }

   public class EditExpressionProfilePresenter : SingleStartContainerPresenter<IEditExpressionProfileView, IEditExpressionProfilePresenter, ExpressionProfile, IExpressionProfileItemPresenter>, IEditExpressionProfilePresenter
   {
      private ExpressionProfile _expressionProfile;

      public EditExpressionProfilePresenter(IEditExpressionProfileView view, ISubPresenterItemManager<IExpressionProfileItemPresenter> subPresenterItemManager) :
         base(view, subPresenterItemManager, ExpressionProfileItems.All)
      {
      }

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditExpressionProfile(_expressionProfile.Name);
         _view.ApplicationIcon = ApplicationIcons.IconByName(_expressionProfile.Icon);
      }

      public override object Subject => _expressionProfile;

      public override void Edit(ExpressionProfile expressionProfile)
      {
         _expressionProfile = expressionProfile;
         _subPresenterItemManager.AllSubPresenters.Each(x => x.Edit(_expressionProfile));
         _subPresenterItemManager.PresenterAt(ExpressionProfileItems.Molecules).IsEditMode = true;
         UpdateCaption();
         _view.Display();
      }
   }
}