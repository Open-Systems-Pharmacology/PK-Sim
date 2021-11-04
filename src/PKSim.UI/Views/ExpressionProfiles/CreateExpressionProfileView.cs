using System.Drawing;
using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.ExpressionProfiles
{
   public partial class CreateExpressionProfileView : BuildingBlockContainerView, ICreateExpressionProfileView
   {
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
   }
}