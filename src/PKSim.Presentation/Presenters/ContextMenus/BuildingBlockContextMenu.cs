using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class BuildingBlockContextMenu<TBuildingBlock> : ContextMenu<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected BuildingBlockContextMenu(TBuildingBlock buildingBlock)
         : base(buildingBlock)
      {
      }

      protected virtual IEnumerable<IMenuBarItem> AllStandardMenuItemsFor<TCommand>(TBuildingBlock buildingBlock) where TCommand : IEditBuildingBlockUICommand<TBuildingBlock>
      {
         var allMenuItems = new List<IMenuBarItem>();
         allMenuItems.AddRange(EditContextMenusFor<TCommand>(buildingBlock));
         allMenuItems.AddRange(ExportAndDeleteContextMenusFor(buildingBlock));
         return allMenuItems;
      }

      protected IEnumerable<IMenuBarItem> ExportAndDeleteContextMenusFor(TBuildingBlock buildingBlock)
      {
         var allMenuItems = new List<IMenuBarItem>();
         allMenuItems.AddRange(ExportContextMenusFor(buildingBlock));
         allMenuItems.AddRange(DeleteContextMenusFor(buildingBlock));
         return allMenuItems;
      }

      protected IEnumerable<IMenuBarItem> DeleteContextMenusFor(TBuildingBlock buildingBlock)
      {
         yield return DeleteMenuFor(buildingBlock)
            .AsGroupStarter();

         yield return ParameterValueDebugMenuFor(buildingBlock);
      }

      protected IEnumerable<IMenuBarItem> ExportContextMenusFor(TBuildingBlock buildingBlock)
      {
         yield return SaveAsUserTemplateMenuFor(buildingBlock)
            .AsGroupStarter();

         yield return SaveAsSystemTemplateMenuFor(buildingBlock);

         yield return ExportToPDFMenuFor(buildingBlock);
         
         yield return AddToJournalMenuFor(buildingBlock);

         yield return ExportSnapshotMenuFor(buildingBlock);
      }

      protected IEnumerable<IMenuBarItem> EditContextMenusFor<TCommand>(TBuildingBlock buildingBlock) where TCommand : IEditBuildingBlockUICommand<TBuildingBlock>
      {
         yield return EditMenuFor<TCommand>(buildingBlock);

         yield return RenameMenuFor(buildingBlock);

         yield return DescriptionMenuFor(buildingBlock);
    
         yield return CloneMenuFor(buildingBlock)
            .AsGroupStarter();
      }

      protected IMenuBarItem RenameMenuFor(TBuildingBlock buildingBlock)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
            .WithCommandFor<RenameBuildingBlockUICommand, IPKSimBuildingBlock>(buildingBlock)
            .WithIcon(ApplicationIcons.Rename);
      }

      protected IMenuBarItem EditMenuFor<TCommand>(TBuildingBlock buildingBlock) where TCommand : IEditBuildingBlockUICommand<TBuildingBlock>
      {
         return GenericMenu.EditMenuFor<TCommand, TBuildingBlock>(buildingBlock);
      }

      protected IMenuBarItem DescriptionMenuFor(TBuildingBlock buildingBlock)
      {
         return GenericMenu.EditDescriptionMenuFor(buildingBlock);
      }

      protected IMenuBarItem CloneMenuFor(TBuildingBlock buildingBlock)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Clone)
            .WithCommandFor<CloneBuildingBlockCommand<TBuildingBlock>, TBuildingBlock>(buildingBlock)
            .WithIcon(ApplicationIcons.Clone);
      }

      protected IMenuBarItem DeleteMenuFor(TBuildingBlock buildingBlock)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithCommandFor<DeleteBuildingBlockUICommand, IPKSimBuildingBlock>(buildingBlock)
            .WithIcon(ApplicationIcons.Delete);
      }

      protected IMenuBarItem SaveAsUserTemplateMenuFor(TBuildingBlock buildingBlock)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveAsTemplate)
            .WithCommandFor<SaveBuildingBlockAsTemplateCommand<TBuildingBlock>, TBuildingBlock>(buildingBlock)
            .WithIcon(ApplicationIcons.SaveAsTemplate);
      }

      protected IMenuBarItem SaveAsSystemTemplateMenuFor(TBuildingBlock buildingBlock)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveAsSytemTemplate)
            .WithCommandFor<SaveBuildingBlockAsSystemTemplateCommand<TBuildingBlock>, TBuildingBlock>(buildingBlock)
            .WithIcon(ApplicationIcons.SaveAsTemplate)
            .ForDeveloper();
      }

      protected IMenuBarItem ParameterValueDebugMenuFor(TBuildingBlock buildingBlock)
      {
         return CreateMenuButton.WithCaption("Parameter Value Export (Debug only)")
            .WithCommandFor<ParameterValueForDebugCommand, IPKSimBuildingBlock>(buildingBlock)
            .ForDeveloper();
      }

      protected IMenuBarItem AddToJournalMenuFor(TBuildingBlock buildingBlock)
      {
         return GenericMenu.AddToJournal(buildingBlock);
      }

      protected IMenuBarItem ExportToPDFMenuFor(TBuildingBlock buildingBlock)
      {
         return GenericMenu.ExportToPDFMenuFor(buildingBlock);
      }

      protected IMenuBarItem ExportSnapshotMenuFor(TBuildingBlock buildingBlock)
      {
         return GenericMenu.ExportSnapshotMenuFor(buildingBlock);
      }

   }
}