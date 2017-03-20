using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Main;

namespace PKSim.Presentation.Views.Main
{
    public interface ISplashView : IView<ISplashViewPresenter>
    {
        string StatusInfo {  set; }
        double ProgressValue {  set; }
    }
}