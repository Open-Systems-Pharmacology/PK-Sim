using System.Data;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IParameterDebugView : IModalView<IParameterDebugPresenter>,IModalView<IParameterValuesDebugPresenter>
   {
      void BindTo(DataTable dataTable);
   }
}