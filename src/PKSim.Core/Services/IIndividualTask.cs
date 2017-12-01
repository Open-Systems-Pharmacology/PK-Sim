using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IIndividualTask : IBuildingBlockTask<Individual>
   {
      void ScaleIndividual(Individual individualToScale);
   }
}