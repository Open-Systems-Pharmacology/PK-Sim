using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPopulationTask : IBuildingBlockTask<Population>
   {
      void AddToProjectBasedOn(Individual individual);
   }
}