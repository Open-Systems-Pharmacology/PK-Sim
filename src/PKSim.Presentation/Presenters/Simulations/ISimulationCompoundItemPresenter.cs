using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundItemPresenter : ISubPresenter
   {
      void EditSimulation( PKSim.Core.Model.Simulation simulation, PKSim.Core.Model.Compound compound);
      void SaveConfiguration();
   }
}