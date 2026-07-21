using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Core;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class UsedCompoundInSimulationContextMenu : UsedBuildingBlockInSimulationContextMenu<Compound>
   {
      public UsedCompoundInSimulationContextMenu(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Compound compound, IContainer container)
         : base(simulation, usedBuildingBlock, compound, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(UsedBuildingBlock usedBuildingBlock, Compound templateBuildingBlock)
      {
         foreach (var item in base.AllMenuItemsFor(usedBuildingBlock, templateBuildingBlock))
         {
            yield return item;
         }

         if (_simulation is IndividualSimulation && _simulation.HasUncommittedChangesForCompound(templateBuildingBlock.Name))
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.CommitSimulationParametersToCompounds)
               .WithCommitSimulationParametersCommandFor(_simulation, templateBuildingBlock)
               .WithIcon(ApplicationIcons.Commit);
      }
   }
}
