using System.Drawing;
using System.Windows.Forms;
using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Views.Main;
using OSPSuite.UI.Views;
using PKSim.Assets.Images;

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
         ClientSize = new Size(BackgroundImage.Size.Width, BackgroundImage.Size.Height);
         ShowInTaskbar = false;
         TopMost = true;
         Opacity = 0.9;
         progressBar.Properties.ShowTitle = true;
         Text = PKSimConstants.UI.LoadingApplication;
      }

      public string StatusInfo
      {
         set => lblProgress.Text = $"\t{value}";
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
      }
   }
}