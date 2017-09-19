using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.UICommands;
using OSPSuite.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public static class GenericMenu
   {
      public static IMenuBarItem EditDescriptionMenuFor<T>(T objectBase) where T : IObjectBase
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Description)
            .WithIcon(ApplicationIcons.Description)
            .WithCommandFor<EditDescriptionUICommand, IObjectBase>(objectBase);
      }

      public static IMenuBarItem ExportToPDFMenuFor<T>(T objectToExport) where T : class
      {
         return CreateMenuButton.WithCaption(MenuNames.ExportToPDF)
            .WithCommandFor<ExportToPDFCommand<T>, T>(objectToExport)
            .WithIcon(ApplicationIcons.PDF);
      }

      public static IMenuBarItem ExportSnapshotMenuFor<T>(T objectToExport) where T : class, IObjectBase
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSnapshot)
            .WithCommandFor<ExportSnapshotUICommand<T>, T>(objectToExport)
            //TODO ICON
            .WithIcon(ApplicationIcons.PDF);
      }

      public static IMenuBarItem ExportSnapshotMenuFor<T, TContext>(T objectToExport, TContext context) where T : class, IObjectBase
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportSnapshot)
            .WithCommandFor<ExportSnapshotUICommand<T>, T>(objectToExport)
            //TODO ICON
            .WithIcon(ApplicationIcons.PDF);
      }

      public static IMenuBarItem ExportCollectionToPDFMenuFor<T>()
      {
         return CreateMenuButton.WithCaption(MenuNames.ExportToPDF)
            .WithCommand<ExportCollectionToPDFCommand<T>>()
            .WithIcon(ApplicationIcons.PDF);
      }

      public static IMenuBarItem LoadBuildingBlockFromSnapshot<T>() where T:class, IPKSimBuildingBlock
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadFromSnapshot)
            .WithCommand<LoadBuildingBlockFromSnapshotUICommand<T>>()
            //TODO ICON
            .WithIcon(ApplicationIcons.LoadFromTemplate);
      }

      public static IMenuBarItem EditMenuFor<TCommand, T>(T objectToEdit) where T : class where TCommand : IObjectUICommand<T>
      {
         return CreateMenuButton.WithCaption(MenuNames.Edit)
            .WithCommandFor<TCommand, T>(objectToEdit)
            .WithIcon(ApplicationIcons.Edit);
      }

      public static IMenuBarItem AddToJournal<T>(T objectBase) where T : IObjectBase
      {
         return ObjectBaseCommonContextMenuItems.AddToJournal(objectBase);
      }

      public static IMenuBarItem RenameMenuFor<T>(T objectToRename) where T : IWithName
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
            .WithCommandFor<RenameObjectUICommand, IWithName>(objectToRename)
            .WithIcon(ApplicationIcons.Rename);
      }
   }
}