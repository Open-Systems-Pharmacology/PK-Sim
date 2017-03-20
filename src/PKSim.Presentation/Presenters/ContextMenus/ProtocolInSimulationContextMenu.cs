using OSPSuite.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ProtocolInSimulationContextMenu : UsedBuidlingBlockInSimulationContextMenu<Protocol>
   {
      public ProtocolInSimulationContextMenu(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Protocol templateProtocol)
         : base(simulation, usedBuildingBlock, templateProtocol)
      {
      }

      protected override ApplicationIcon RetrieveCommitIcon()
      {
         return ApplicationIcons.CommitRed;
      }
   }
}