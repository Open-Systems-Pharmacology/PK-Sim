using PKSim.Presentation.DTO.Formulations;
using PKSim.Presentation.Presenters.Formulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Formulations
{
   public interface IFormulationSettingsView : IView<IFormulationSettingsPresenter>
   {
      void BindTo(FormulationDTO formulationDTO);
      void AddParameterView(IView view);
      void AddChartView(IView view);
      bool FormulationTypeVisible { get; set; }
      bool ChartVisible { get; set; }
   }
}