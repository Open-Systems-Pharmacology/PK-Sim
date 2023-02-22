using OSPSuite.Assets;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class PopulationInSimulationContextMenu : UsedBuildingBlockInSimulationContextMenu<Population>
   {
      public PopulationInSimulationContextMenu(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Population templatePopulation, IContainer container)
         : base(simulation, usedBuildingBlock, templatePopulation, container)
      {
      }

      protected override ApplicationIcon RetrieveCommitIcon()
      {
         return ApplicationIcons.CommitRed;
      }
   }
}