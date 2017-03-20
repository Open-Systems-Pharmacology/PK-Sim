using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IModelObserverQuery
   {
      /// <summary>
      ///    Return all the observers defined for the given molecule name and the model properties
      /// </summary>
      IObserverBuildingBlock AllObserversFor(IMoleculeBuildingBlock moleculeBuildingBlock, Simulation simulation);
   }
}