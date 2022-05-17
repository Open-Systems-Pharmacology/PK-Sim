using System.Collections.Generic;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
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
         return AllStandardMenuItemsFor<EditExpressionProfileCommand>(expressionProfile);
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