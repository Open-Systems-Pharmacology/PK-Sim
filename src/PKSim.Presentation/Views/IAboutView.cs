using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface IAboutView : IModalView<IAboutPresenter>
   {
      string VersionInformation { set; }
      string Product { set; }
      string LicenseAgreementFilePath { set; }
   }
}