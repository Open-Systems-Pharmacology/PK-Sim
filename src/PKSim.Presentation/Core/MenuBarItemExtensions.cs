using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Core
{
   public static class MenuBarItemExtensions
   {
      public static IMenuBarButton WithUpdateCommandFor<TBuildingBlock>(this IMenuBarButton menuBarItem, Simulation simulation, TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock)
         where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var command = IoC.Resolve<UpdateBuildingBlockInSimulationUICommand<TBuildingBlock>>();
         command.TemplateBuildingBlock = templateBuildingBlock;
         command.Simulation = simulation;
         command.UsedBuildingBlock = usedBuildingBlock;
         return menuBarItem.WithCommand(command);
      }

      public static IMenuBarButton WithCommitCommandFor<TBuildingBlock>(this IMenuBarButton menuBarItem, Simulation simulation, TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock)
         where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var command = IoC.Resolve<CommitBuildingBlockFromSimulationUICommand<TBuildingBlock>>();
         command.TemplateBuildingBlock = templateBuildingBlock;
         command.Simulation = simulation;
         command.UsedBuildingBlock = usedBuildingBlock;
         return menuBarItem.WithCommand(command);
      }

      public static IMenuBarButton WithDiffCommandFor<TBuildingBlock>(this IMenuBarButton menuBarItem, Simulation simulation, TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock)
         where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var command = IoC.Resolve<BuildingBlockDiffUICommand<TBuildingBlock>>();
         command.TemplateBuildingBlock = templateBuildingBlock;
         command.Simulation = simulation;
         command.UsedBuildingBlock = usedBuildingBlock;
         return menuBarItem.WithCommand(command);
      }
   }
}