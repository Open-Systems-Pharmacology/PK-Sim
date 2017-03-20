using System.Collections.Generic;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface IEnzymaticCompoundProcessView : IView<IEnzymaticCompoundProcessPresenter>, IProcessView<EnzymaticProcessDTO>
   {


      /// <summary>
      /// Sets the list of compound that can be used as metabolite
      /// </summary>
      void UpdateAvailableCompounds(IEnumerable<string> availableCompoundNames);
   }
}