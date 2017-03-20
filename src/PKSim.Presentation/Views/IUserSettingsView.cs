using PKSim.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
   public interface IUserSettingsView : IView<IUserSettingsPresenter>
   {
      void BindTo(IUserSettings userSettings);
      void RefreshAllIndividualList();
   }
}