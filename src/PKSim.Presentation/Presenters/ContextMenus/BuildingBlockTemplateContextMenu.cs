using System;
using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   //TODO DELETE
   // public class BuildingBlockTemplateContextMenu : ContextMenu<Template, ITemplatePresenter>
   // {
   //    public BuildingBlockTemplateContextMenu(Template buildingBlockTemplate, ITemplatePresenter presenter)
   //       : base(buildingBlockTemplate, presenter)
   //    {
   //    }
   //
   //    protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(Template buildingBlockTemplate, ITemplatePresenter presenter)
   //    {
   //       //no context menu available for system items
   //       if (!presenter.CanEdit(buildingBlockTemplate))
   //          yield break;
   //
   //       yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Rename)
   //          .WithActionCommand(()=>presenter.Rename(buildingBlockTemplate))
   //          .WithIcon(ApplicationIcons.Rename);
   //
   //       yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
   //          .WithActionCommand(()=>presenter.Delete(buildingBlockTemplate))
   //          .WithIcon(ApplicationIcons.Delete);
   //    }
   // }
   //
   // public class BuildingBlockTemplateTreeNodeContextMenuFactory : NodeContextMenuFactory<Template>
   // {
   //    public override IContextMenu CreateFor(Template template, IPresenterWithContextMenu<ITreeNode> presenter)
   //    {
   //       return new BuildingBlockTemplateContextMenu(template, presenter.DowncastTo<ITemplatePresenter>());
   //    }
   //
   //    public override bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
   //    {
   //       return base.IsSatisfiedBy(treeNode, presenter) && presenter.IsAnImplementationOf<ITemplatePresenter>();
   //    }
   // }
}