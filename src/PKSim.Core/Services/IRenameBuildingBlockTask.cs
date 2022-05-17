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
      ///    Renames the building block to <paramref name="newName" /> and ensures that all dependent objects are
      ///    updated accordingly
      /// </summary>
      /// <param name="templateBuildingBlock">Building block to be renamed</param>
      /// <param name="newName">new building block name name</param>
      void RenameBuildingBlock(IPKSimBuildingBlock templateBuildingBlock, string newName);
   }
}