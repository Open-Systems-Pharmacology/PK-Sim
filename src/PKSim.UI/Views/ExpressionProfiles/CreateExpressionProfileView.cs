using System.Drawing;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.ExpressionProfiles
{
   public partial class CreateExpressionProfileView : BaseModalContainerView, ICreateExpressionProfileView
   {
      private ICreateExpressionProfilePresenter _presenter;

      public CreateExpressionProfileView(Shell shell) : base(shell)
      {
         InitializeComponent();
         ClientSize = new Size(CoreConstants.UI.EXPRESSION_PROFILE_VIEW_WIDTH, CoreConstants.UI.EXPRESSION_PROFILE_VIEW_HEIGHT);
      }

      public void AttachPresenter(ICreateExpressionProfilePresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Icon = ApplicationIcons.ExpressionProfile.WithSize(IconSizes.Size16x16);
         Caption = PKSimConstants.UI.CreateExpressionProfile;
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView viewToAdd)
      {
         panel.FillWith(viewToAdd);
      }

   }
}