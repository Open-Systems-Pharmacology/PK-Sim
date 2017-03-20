using System.Collections.Generic;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views;

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
      string Description { set; }
      string ImportToolTip { set; }
   }
}