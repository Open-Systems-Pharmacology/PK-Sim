using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters.Parameters;

namespace PKSim.Presentation.Views.Parameters
{
   public interface ITableParameterView : IView<ITableParameterPresenter>
   {
      void Clear();
      void BindTo(IEnumerable<ValuePointDTO> allPoints);
      void EditPoint(ValuePointDTO pointToEdit);
      bool ImportVisible { set; }
      string YCaption { set; }
      string XCaption { set; }
      bool Editable { get; set; }
      string Description { get; set; }
      string ImportToolTip { get; set; }
   }
}