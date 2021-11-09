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

      /// <summary>
      ///    Change the name of the compound used in the simulation. The <paramref name="targetSimulation"/> was created using the compound
      /// named <paramref name="newCompoundName"/> but might be using properties still referencing <paramref name="oldCompoundName"/>
      /// </summary>
      void SynchronizeCompoundNameIn(Simulation targetSimulation, string oldCompoundName, string newCompoundName);
   }
}