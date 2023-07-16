using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class SimulationContextMenu<TSimulation> : BuildingBlockContextMenu<TSimulation> where TSimulation : Simulation
   {
      protected SimulationContextMenu(TSimulation simulation, IContainer container) : base(simulation, container)
      {
      }

      protected abstract IEnumerable<IMenuBarItem> ExportMenuItemsSpecificToType(TSimulation simulation);

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(TSimulation simulation)
      {
         return commonItemsForSimulations(simulation)
            .Union(ActionMenuItemsSpecificToType(simulation))
            .Union(commonExportItemsFormSimulation(simulation))
            .Union(ExportMenuItemsSpecificToType(simulation))
            .Union(deleteMenuFor(simulation))
            .Union(DebugMenuFor(simulation));
      }

      protected virtual IEnumerable<IMenuBarItem> ActionMenuItemsSpecificToType(TSimulation populationSimulation)
      {
         yield break;
      }

      protected virtual IEnumerable<IMenuBarItem> DebugMenuFor(TSimulation simulation)
      {
         yield break;
      }

      private IEnumerable<IMenuBarItem> deleteMenuFor(TSimulation simulation)
      {
         yield return DeleteMenuFor(simulation)
            .AsGroupStarter();
      }

      protected IMenuBarItem ExportSimulationToCppMenuItem(TSimulation simulation)
      {
         return CreateMenuButton.WithCaption(MenuNames.ExportForCpp)
            .WithCommandFor<ExportSimulationToCppUICommand, Simulation>(simulation, _container)
            .ForDeveloper();
      }

      protected IMenuBarItem ExportODEForMatlabMenuItem(TSimulation simulation)
      {
         return CreateMenuButton.WithCaption(MenuNames.AsDeveloperOnly(MenuNames.ExportODEForMatlab))
            .WithCommandFor<ExportODEForMatlabUICommand, Simulation>(simulation, _container)
            .WithIcon(ApplicationIcons.Matlab)
            .ForDeveloper();
      }

      protected IMenuBarItem ExportODEForRMenuItem(TSimulation simulation)
      {
         return CreateMenuButton.WithCaption(MenuNames.AsDeveloperOnly(MenuNames.ExportODEForR))
            .WithCommandFor<ExportODEForRUICommand, Simulation>(simulation, _container)
            .WithIcon(ApplicationIcons.R)
            .ForDeveloper();
      }

      private IEnumerable<IMenuBarItem> commonItemsForSimulations(TSimulation simulation)
      {
         yield return GenericMenu.EditMenuFor<EditSimulationCommand, Simulation>(simulation, _container);
         yield return RenameMenuFor(simulation);

         yield return DescriptionMenuFor(simulation)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Clone)
            .WithCommandFor<CloneSimulationCommand, Simulation>(simulation, _container)
            .WithIcon(ApplicationIcons.SimulationClone)
            .AsDisabledIf(simulation.IsImported)
            .AsGroupStarter();

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Configure)
            .AsDisabledIf(simulation.IsImported)
            .WithIcon(ApplicationIcons.SimulationConfigure)
            .WithCommandFor<ConfigureSimulationCommand, Simulation>(simulation, _container);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Run)
            .WithCommandFor<RunSimulationCommand, Simulation>(simulation, _container)
            .WithIcon(ApplicationIcons.Run)
            .AsGroupStarter();
      }

      private IEnumerable<IMenuBarItem> commonExportItemsFormSimulation(TSimulation simulation)
      {
         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToMoBi)
            .WithCommandFor<ExportSimulationToMoBiCommand, Simulation>(simulation, _container)
            .WithIcon(ApplicationIcons.MoBi)
            .AsGroupStarter()
            .AsDisabledIf(simulation.IsImported);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveToMoBiSimulation)
            .WithCommandFor<SaveSimulationToMoBiFileCommand, Simulation>(simulation, _container)
            .WithIcon(ApplicationIcons.PKMLSave)
            .AsDisabledIf(simulation.IsImported);

         yield return AddToJournalMenuFor(simulation);
      }
   }

   public abstract class SimulationTreeNodeContextMenuFactory<TSimulation> : IContextMenuSpecificationFactory<ITreeNode> where TSimulation : Simulation
   {
      public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return CreateFor(SimulationFrom(treeNode), presenter);
      }

      protected abstract IContextMenu CreateFor(TSimulation simulation, IPresenterWithContextMenu<ITreeNode> presenter);

      public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return SimulationFrom(treeNode) != null;
      }

      protected TSimulation SimulationFrom(ITreeNode treeNode)
      {
         var simulationNode = treeNode as SimulationNode;
         return simulationNode?.Tag.Simulation as TSimulation;
      }
   }
}