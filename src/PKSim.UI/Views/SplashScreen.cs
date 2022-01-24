using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using PKSim.Assets;
using OSPSuite.Assets;
using OSPSuite.UI.Extensions;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Views.Main;
using OSPSuite.UI.Views;
using PKSim.Assets.Images;
using PKSim.UI.Extensions;

namespace PKSim.UI.Views
{
   public partial class SplashScreen : BaseView, ISplashView
   {
      //should hold a reference to allow progress information
      private ISplashViewPresenter _presenter;

      public SplashScreen()
      {
         InitializeComponent();

         BackgroundImage = ApplicationImages.Splash;
         FormBorderStyle = FormBorderStyle.None;
         StartPosition = FormStartPosition.CenterScreen;
         ShowInTaskbar = false;
         TopMost = true;
         Opacity = 0.9;
         progressBar.Properties.ShowTitle = true;
         Text = PKSimConstants.UI.LoadingApplication;
         buttonHide.Click += (o, e) => OnEvent(() => Visible = false); 
      }

      public string StatusInfo
      {
         set => lblProgress.Text = $"{value}";
      }

      public double ProgressValue
      {
         set => progressBar.EditValue = value;
      }

      public void AttachPresenter(ISplashViewPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         lblProgress.Text = string.Empty;
         Icon = ApplicationIcons.PKSim.WithSize(IconSizes.Size48x48);
         buttonHide.Text = "x";
         buttonHide.ButtonStyle = BorderStyles.NoBorder;
         buttonHide.AdjustButtonWithImageOnly();
         buttonHide.AllowFocus = false;
      }
   }
}