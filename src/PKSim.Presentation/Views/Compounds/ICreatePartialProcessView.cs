using System.Collections.Generic;

using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICreatePartialProcessView<TPresenter> : IModalView<TPresenter>, ICreateProcessView where TPresenter : IDisposablePresenter
   {
      string MoleculeCaption { set; }
      IEnumerable<string> AllAvailableProteins { set; }
      void BindTo(PartialProcessDTO partialProcessDTO);
   }

   public interface ICreateEnzymaticProcessView : ICreatePartialProcessView<ICreateEnzymaticProcessPresenter>
   {
      void UpdateAvailableCompounds(IEnumerable<string> availableCompoundNames);
   }

   public interface ICreatePartialProcessView : ICreatePartialProcessView<ICreatePartialProcessPresenter>
   {

   }
}