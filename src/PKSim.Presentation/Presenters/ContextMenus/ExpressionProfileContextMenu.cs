using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ExpressionProfileContextMenu : BuildingBlockContextMenu<ExpressionProfile>
   {
      public ExpressionProfileContextMenu(ExpressionProfile expressionProfile) : base(expressionProfile)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ExpressionProfile expressionProfile)
      {
         var allMenuItems = new List<IMenuBarItem>();
         allMenuItems.AddRange(EditContextMenusFor<EditExpressionProfileCommand>(expressionProfile));
         allMenuItems.AddRange(extendedExportMenus(expressionProfile));
         allMenuItems.AddRange(DeleteContextMenusFor(expressionProfile));
         allMenuItems.AddRange(DebugContextMenusFor(expressionProfile));
         return allMenuItems;
      }

      private IEnumerable<IMenuBarItem> extendedExportMenus(ExpressionProfile expressionProfile)
      {
         yield return SaveAsUserTemplateMenuFor(expressionProfile)
            .AsGroupStarter();

         yield return SaveAsSystemTemplateMenuFor(expressionProfile);

         yield return AddToJournalMenuFor(expressionProfile);

         yield return ExportToPkml(expressionProfile);
      }

      private IMenuBarItem ExportToPkml(ExpressionProfile expressionProfile)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToPKML)
            .WithCommandFor<ExportExpressionProfileToPkmlCommand, ExpressionProfile>(expressionProfile)
            .WithIcon(ApplicationIcons.PKMLSave);
      }
   }

   public class ExpressionProfileTreeNodeContextMenuFactory : NodeContextMenuFactory<ExpressionProfile>
   {
      public override IContextMenu CreateFor(ExpressionProfile expressionProfile, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ExpressionProfileContextMenu(expressionProfile);
      }
   }
}