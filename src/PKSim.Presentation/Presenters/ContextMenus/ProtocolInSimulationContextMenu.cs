using OSPSuite.Assets;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ProtocolInSimulationContextMenu : UsedBuildingBlockInSimulationContextMenu<Protocol>
   {
      public ProtocolInSimulationContextMenu(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Protocol templateProtocol, IContainer container)
         : base(simulation, usedBuildingBlock, templateProtocol, container)
      {
      }

      protected override ApplicationIcon RetrieveCommitIcon()
      {
         return ApplicationIcons.CommitRed;
      }
   }
}