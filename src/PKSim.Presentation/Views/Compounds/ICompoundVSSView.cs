using System.Collections.Generic;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundVSSView : IView<ICompoundVSSPresenter>, IResizableView
   {
      void BindTo(IEnumerable<VSSValueDTO> allVSSValues);
   }
}