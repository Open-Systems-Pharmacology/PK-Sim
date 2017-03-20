using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IBuildingBlockInSimulationSynchronizer
   {
      /// <summary>
      ///    For used building blocks referencing templates, we ensure that the building block used is a clone of the template
      ///    building block (so that if an alternative was used in the compound,
      ///    this alternative is availagble to the user)
      /// </summary>
      void UpdateUsedBuildingBlockBasedOnTemplateIn(Simulation simulation);
   }

   public class BuildingBlockInSimulationSynchronizer : IBuildingBlockInSimulationSynchronizer
   {
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly ICloner _cloner;
      private readonly ICompoundPropertiesUpdater _compoundPropertiesUpdater;

      public BuildingBlockInSimulationSynchronizer(IBuildingBlockInSimulationManager buildingBlockInSimulationManager, ICloner cloner, ICompoundPropertiesUpdater compoundPropertiesUpdater)
      {
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _cloner = cloner;
         _compoundPropertiesUpdater = compoundPropertiesUpdater;
      }

      public void UpdateUsedBuildingBlockBasedOnTemplateIn(Simulation simulation)
      {
         //only required for compound at the moment since they are the only one that can actually get out of sync.
         foreach (var usedBuildingBlock in simulation.UsedBuildingBlocksInSimulation<Compound>())
         {
            var templateBuildingBlock = _buildingBlockInSimulationManager.TemplateBuildingBlockUsedBy(usedBuildingBlock);

            //not using a template building block so we simply keep this guy
            if (Equals(templateBuildingBlock, usedBuildingBlock.BuildingBlock))
               continue;

            usedBuildingBlock.BuildingBlock = _cloner.Clone(templateBuildingBlock);
         }

         _compoundPropertiesUpdater.UpdateCompoundPropertiesIn(simulation);
      }
   }
}