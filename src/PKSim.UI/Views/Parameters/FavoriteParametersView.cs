using DevExpress.XtraEditors;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.UI.Views.Parameters
{
   public partial class FavoriteParametersView : BaseUserControl, IFavoriteParametersView
   {
      private IFavoriteParametersPresenter _presenter;

      public FavoriteParametersView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IFavoriteParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddParametersView(IView view)
      {
         panelParameters.FillWith(view);
      }

      public bool UpEnabled
      {
         get => buttonMoveUp.Enabled;
         set => buttonMoveUp.Enabled = value;
      }

      public bool DownEnabled
      {
         get => buttonMoveDown.Enabled;
         set => buttonMoveDown.Enabled = value;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         buttonMoveUp.Click += (o, e) => OnEvent(_presenter.MoveUp);
         buttonMoveDown.Click += (o, e) => OnEvent(_presenter.MoveDown);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         buttonMoveUp.InitWithImage(ApplicationIcons.Up, imageLocation: ImageLocation.MiddleCenter, toolTip: PKSimConstants.UI.MoveUp);
         buttonMoveDown.InitWithImage(ApplicationIcons.Down, imageLocation: ImageLocation.MiddleCenter, toolTip: PKSimConstants.UI.MoveDown);
         layoutItemButtonMoveUp.AdjustButtonSizeWithImageOnly();
         layoutItemButtonMoveDown.AdjustButtonSizeWithImageOnly();
      }
   }
}