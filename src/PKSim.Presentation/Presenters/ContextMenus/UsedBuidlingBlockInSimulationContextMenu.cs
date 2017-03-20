using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class UsedBuidlingBlockInSimulationContextMenu<TBuildingBlock> : ContextMenu where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected readonly Simulation _simulation;
      protected readonly UsedBuildingBlock _usedBuildingBlock;
      private readonly TBuildingBlock _templateBuildingBlock;

      public UsedBuidlingBlockInSimulationContextMenu(Simulation simulation, UsedBuildingBlock usedBuildingBlock, TBuildingBlock templateBuildingBlock)
      {
         _simulation = simulation;
         _usedBuildingBlock = usedBuildingBlock;
         _templateBuildingBlock = templateBuildingBlock;
         AllMenuItemsFor(_usedBuildingBlock, _templateBuildingBlock).Each(_view.AddMenuItem);
      }

      protected virtual IEnumerable<IMenuBarItem> AllMenuItemsFor(UsedBuildingBlock usedBuildingBlock, TBuildingBlock templateBuildingBlock)
      {
         if (CanUpdate)
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Update)
               .WithUpdateCommandFor(_simulation, templateBuildingBlock, usedBuildingBlock)
               .WithIcon(ApplicationIcons.Update);

         if (CanCommit)
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Commit)
               .WithCommitCommandFor(_simulation, templateBuildingBlock, usedBuildingBlock)
               .WithIcon(RetrieveCommitIcon());

         if (CanShowDiff)
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Diff)
               .WithDiffCommandFor(_simulation, templateBuildingBlock, usedBuildingBlock)
               .WithIcon(ApplicationIcons.Comparison);
      }

 
      protected virtual ApplicationIcon RetrieveCommitIcon()
      {
         return _templateBuildingBlock.StructureVersion == _usedBuildingBlock.StructureVersion
            ? ApplicationIcons.Commit
            : ApplicationIcons.CommitRed;
      }

      protected virtual bool CanUpdate
      {
         get { return _usedBuildingBlock.Altered || _templateBuildingBlock.Version != _usedBuildingBlock.Version; }
      }

      protected virtual bool CanCommit
      {
         get { return CanUpdate; }
      }

      protected virtual bool CanShowDiff
      {
         get { return CanUpdate; }
      }
   }
}