using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public interface IRandomPopulationEvent
   {
      RandomPopulation Population { get; }
   }
}