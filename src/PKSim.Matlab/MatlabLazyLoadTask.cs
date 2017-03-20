using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Matlab
{
   public class MatlabLazyLoadTask : ILazyLoadTask
   {
      public void Load<TObject>(TObject objectToLoad) where TObject : class, ILazyLoadable
      {
         //nothing to do
      }

      public void LoadResults<TSimulation>(TSimulation simulation) where TSimulation : Simulation
      {
         //nothing to do
      }

      public void LoadResults(IPopulationDataCollector populationDataCollector)
      {
         //nothing to do
      }
   }
}