using System.Collections.Generic;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IEditOutputSchemaView : IView<IEditOutputSchemaPresenter>
   {
      void BindTo(IEnumerable<OutputIntervalDTO> allIntervals);
   }
}