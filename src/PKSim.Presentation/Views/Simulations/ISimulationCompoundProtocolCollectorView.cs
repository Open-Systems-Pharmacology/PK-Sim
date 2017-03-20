using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundProtocolCollectorView : ISimulationCompoundCollectorView
   {
      bool WarningVisible { get; set; }
      string Warning { get; set; }
      void AddProtocolChart(IView view);
   }
}