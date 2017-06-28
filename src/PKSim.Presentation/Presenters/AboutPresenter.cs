using PKSim.Assets;
using OSPSuite.Core.Services;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters
{
   public interface IAboutPresenter : IDisposablePresenter
   {
      void CheckForUpdate();
   }

   public class AboutPresenter : AbstractDisposablePresenter<IAboutView, IAboutPresenter>, IAboutPresenter
   {
      private readonly IPKSimConfiguration _configuration;
      private readonly IVersionChecker _versionChecker;
      private readonly IDialogCreator _dialogCreator;

      public AboutPresenter(IAboutView view, IPKSimConfiguration configuration, IVersionChecker versionChecker, IDialogCreator dialogCreator) : base(view)
      {
         _configuration = configuration;
         _versionChecker = versionChecker;
         _dialogCreator = dialogCreator;
         view.LicenseAgreementFilePath = _configuration.LicenseAgreementFilePath;
      }

      public override void Initialize()
      {
         _view.Product = CoreConstants.ProductDisplayName;
         _view.VersionInformation = $"Version {_configuration.FullVersion}";
         _view.Display();
      }

      public void CheckForUpdate()
      {
         if (_versionChecker.NewVersionIsAvailable())
            _dialogCreator.MessageBoxInfo(PKSimConstants.Information.NewVersionIsAvailable(_versionChecker.LatestVersion, Constants.PRODUCT_SITE_DOWNLOAD).RemoveHtml());
         else
            _dialogCreator.MessageBoxInfo(PKSimConstants.UI.ProductIsUptodate(CoreConstants.PRODUCT_NAME_WITH_TRADEMARK));
      }
   }
}