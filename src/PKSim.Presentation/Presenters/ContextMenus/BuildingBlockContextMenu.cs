using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

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
         allMenuItems.AddRange(DebugContextMenusFor(buildingBlock));
         return allMenuItems;
      }

      protected IEnumerable<IMenuBarItem> DeleteContextMenusFor(TBuildingBlock buildingBlock)
      {
         yield return DeleteMenuFor(buildingBlock)
            .AsGroupStarter();
      }

      protected IEnumerable<IMenuBarItem> DebugContextMenusFor(TBuildingBlock buildingBlock)
      {
         yield return ParameterValueDebugMenuFor(buildingBlock)
            .AsGroupStarter();

         yield return ExportSnapshotMenuFor(buildingBlock);

         yield return ExportMarkdownMenuFor(buildingBlock);
      }

      protected IEnumerable<IMenuBarItem> ExportContextMenusFor(TBuildingBlock buildingBlock)
      {
         yield return SaveAsUserTemplateMenuFor(buildingBlock)
            .AsGroupStarter();

         yield return SaveAsSystemTemplateMenuFor(buildingBlock);

         yield return ExportToPDFMenuFor(buildingBlock);

         yield return AddToJournalMenuFor(buildingBlock);
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
            .WithCommandFor<SaveBuildingBlockAsTemplateCommand<TBuildingBlock>, IReadOnlyList<TBuildingBlock>>(new[] {buildingBlock,})
            .WithIcon(ApplicationIcons.SaveAsTemplate);
      }

      protected IMenuBarItem SaveAsSystemTemplateMenuFor(TBuildingBlock buildingBlock)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveAsSytemTemplate)
            .WithCommandFor<SaveBuildingBlockAsSystemTemplateCommand<TBuildingBlock>, IReadOnlyList<TBuildingBlock>>(new[] {buildingBlock,})
            .WithIcon(ApplicationIcons.SaveAsTemplate)
            .ForDeveloper();
      }

      protected IMenuBarItem ParameterValueDebugMenuFor(TBuildingBlock buildingBlock)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.AsDeveloperOnly("Parameter Value Export"))
            .WithCommandFor<ParameterValueForDebugCommand, IPKSimBuildingBlock>(buildingBlock)
            .ForDeveloper();
      }

      protected IMenuBarItem AddToJournalMenuFor(TBuildingBlock buildingBlock) => GenericMenu.AddToJournal(buildingBlock);

      protected IMenuBarItem ExportToPDFMenuFor(TBuildingBlock buildingBlock) => GenericMenu.ExportToPDFMenuFor(buildingBlock);

      protected IMenuBarItem ExportSnapshotMenuFor(TBuildingBlock buildingBlock) => GenericMenu.ExportSnapshotMenuFor(buildingBlock);

      protected IMenuBarItem ExportMarkdownMenuFor(TBuildingBlock buildingBlock) => GenericMenu.ExportMarkdownMenuFor(buildingBlock);
   }
}