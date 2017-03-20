using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationSubjectConfigurationView : IView<ISimulationSubjectConfigurationPresenter>
   {
      void BindTo(SimulationSubjectDTO simulationSubjectDTO);
      bool AllowAgingVisible { get; set; }
   }
}