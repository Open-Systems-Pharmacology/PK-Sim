using OSPSuite.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class PopulationInSimulationContextMenu : UsedBuidlingBlockInSimulationContextMenu<Population>
   {
      public PopulationInSimulationContextMenu(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Population templatePopulation)
         : base(simulation, usedBuildingBlock, templatePopulation)
      {
      }

      protected override ApplicationIcon RetrieveCommitIcon()
      {
         return ApplicationIcons.CommitRed;
      }
   }
}