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
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ExpressionProfileContextMenu : BuildingBlockContextMenu<ExpressionProfile>
   {
      public ExpressionProfileContextMenu(ExpressionProfile expressionProfile, IContainer container) : base(expressionProfile, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(ExpressionProfile expressionProfile)
      {
         var allMenuItems = new List<IMenuBarItem>();
         allMenuItems.AddRange(EditContextMenusFor<EditExpressionProfileCommand>(expressionProfile));
         allMenuItems.AddRange(ExportContextMenusFor(expressionProfile));
         allMenuItems.AddRange(DeleteContextMenusFor(expressionProfile));
         allMenuItems.AddRange(DebugContextMenusFor(expressionProfile));
         return allMenuItems;
      }

      protected override IEnumerable<IMenuBarItem> ExportContextMenusFor(ExpressionProfile expressionProfile) =>
         base.ExportContextMenusFor(expressionProfile).Concat(new[] { exportToPkml(expressionProfile) });

      private IMenuBarItem exportToPkml(ExpressionProfile expressionProfile)
      {
         return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.ExportToPKML)
            .WithCommandFor<ExportExpressionProfileToPKMLCommand, ExpressionProfile>(expressionProfile, _container)
            .WithIcon(ApplicationIcons.PKMLSave);
      }
   }

   public class ExpressionProfileTreeNodeContextMenuFactory : NodeContextMenuFactory<ExpressionProfile>
   {
      private readonly IContainer _container;

      public ExpressionProfileTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ExpressionProfile expressionProfile, IPresenterWithContextMenu<ITreeNode> presenter) =>
         new ExpressionProfileContextMenu(expressionProfile, _container);
   }
}