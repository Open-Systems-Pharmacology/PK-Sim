using System.Collections.Generic;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface ISimpleMoleculeView : IObjectBaseView
   {
      IEnumerable<string> AvailableProteins { set; }
   }
}