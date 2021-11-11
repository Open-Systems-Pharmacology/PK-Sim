using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IRenameBuildingBlockTask
   {
      /// <summary>
      ///    Renames the model used in the simulation based on the new name of the simulation
      /// </summary>
      /// <param name="simulation">Simulation to be renamed</param>
      /// <param name="newName">new simulation name</param>
      void RenameSimulation(Simulation simulation, string newName);

      /// <summary>
      ///    Update build the building block names of simulation using the given building block;
      /// </summary>
      void RenameBuildingBlock(IPKSimBuildingBlock templateBuildingBlock, string oldBuildingBlockName);
   }
}