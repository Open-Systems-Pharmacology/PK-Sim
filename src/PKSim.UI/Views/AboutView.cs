using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSPSuite.UI.Extensions;
using DevExpress.XtraEditors.Controls;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation;
using OSPSuite.Assets;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views
{
   public partial class AboutView : BaseModalView, IAboutView
   {
      private readonly Timer _timer;
      private IAboutPresenter _presenter;
      public string VersionInformation { get; set; }
      public string Product { get; set; }

      public AboutView(Shell shell) : base(shell)
      {
         InitializeComponent();
         Opacity = 0;
         _timer = new Timer {Interval = 60};
         _timer.Tick += (o, e) => timerTick();
      }

      protected override void SetActiveControl()
      {
         ActiveControl = ButtonOk;
      }

      public void AttachPresenter(IAboutPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         licenseAgreementLink.OpenLink += (o, e) => OnEvent(openLicense, e);
      }

      private void openLicense(OpenLinkEventArgs e)
      {
         e.EditValue = LicenseAgreementFilePath;
      }

      public override void Display()
      {
         lblProductInfo.Text = versionDescription();
         _timer.Start();
         base.Display();
      }

      private string versionDescription()
      {
         var sb = new StringBuilder($"<B>{Product}</B>");
         sb.AppendFormat("\t{0}\n", VersionInformation);
         sb.AppendLine();
         return sb.ToString();
      }

      private void timerTick()
      {
         try
         {
            Opacity += 0.04;
            if (Opacity >= 0.99)
               _timer.Stop();
         }
         catch
         {
            /*nothing to do in that case*/
         }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.AboutProduct(CoreConstants.ProductDisplayName);
         lblProductInfo.BackColor = Color.Transparent;
         lblProductInfo.AsDescription();
         CancelVisible = false;
         linkSite.Text = Constants.PRODUCT_SITE;
         licenseAgreementLink.Text = Captions.ReadLicenseAgreement;
         ExtraCaption = PKSimConstants.UI.CheckForUpdate;
         ExtraEnabled = true;
         ExtraVisible = true;
      }

      public string LicenseAgreementFilePath { get; set; }

      protected override void ExtraClicked()
      {
         _presenter.CheckForUpdate();
      }
   }
}