using System.Collections.Generic;

using PKSim.Presentation.DTO.Applications;
using PKSim.Presentation.Presenters.Applications;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Applications
{
   public interface IApplicationParametersView : IView<IApplicationParametersPresenter>
   {
      void BindTo(IEnumerable<ApplicationDTO> allApplications);
   }
}