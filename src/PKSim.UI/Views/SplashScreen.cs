using System;
using System.Windows.Forms;
using OSPSuite.Assets;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Views.Main;

namespace PKSim.UI.Views
{
   public partial class SplashScreen : BaseView, ISplashView
   {
      public SplashScreen()
      {
         InitializeComponent();
         this.labelCopyright.Text = $"Copyright © 2002-{DateTime.Now.Year} - Open Systems Pharmacology Community";
         FormBorderStyle = FormBorderStyle.None;
         StartPosition = FormStartPosition.CenterScreen;
         ShowInTaskbar = false;
         Opacity = 0.98;
         progressBarControl.Properties.ShowTitle = true;
         Text = PKSimConstants.UI.LoadingApplication;
      }

      public void AttachPresenter(ISplashViewPresenter presenter)
      {
      }

      public string StatusInfo
      {
         set => labelStatus.Text = $"{value}";
      }

      public double ProgressValue
      {
         set => progressBarControl.EditValue = value;
      }

      public override void InitializeResources()
      {
         labelStatus.Text = string.Empty;
         Icon = ApplicationIcons.PKSim.WithSize(IconSizes.Size48x48);
      }
   }
}