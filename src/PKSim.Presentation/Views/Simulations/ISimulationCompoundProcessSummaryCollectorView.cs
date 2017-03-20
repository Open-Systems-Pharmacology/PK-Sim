using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface ISimulationCompoundProcessSummaryCollectorView : ISimulationCompoundCollectorView
   {
      void AddInteractionView(IView view);
      bool ShowInteractionView { get; set; }
   }
}