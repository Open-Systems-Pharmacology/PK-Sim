using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface IApplicationSettingsView : IView<IApplicationSettingsPresenter>
   {
      void BindTo(IEnumerable<SpeciesDatabaseMapDTO> databaseMapDTOs);
      void BindTo(ApplicationSettingsDTO applicationSettings);
   }
}