using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface IUserSettingsView : IView<IUserSettingsPresenter>
   {
      void BindTo(UserSettingsDTO userSettingsDTO);
      void RefreshAllIndividualList();
   }
}