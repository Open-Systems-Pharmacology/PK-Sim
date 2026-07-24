using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus;

public abstract class BuildingBlockNodeContextMenuFactory<TBuildingBlock> : IContextMenuSpecificationFactory<ITreeNode>
   where TBuildingBlock : class, IPKSimBuildingBlock
{
   public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter) =>
      CreateFor(BuildingBlockFrom(treeNode), presenter);

   public abstract IContextMenu CreateFor(TBuildingBlock buildingBlock, IPresenterWithContextMenu<ITreeNode> presenter);

   public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter) => BuildingBlockFrom(treeNode) != null;

   protected static TBuildingBlock BuildingBlockFrom(ITreeNode treeNode)
   {
      var tag = treeNode.TagAsObject;
      var subject = (tag as IClassifiableWrapper)?.WrappedObject ?? tag;
      return subject as TBuildingBlock;
   }
}

public abstract class MultipleBuildingBlockNodeContextMenuFactory<TBuildingBlock> : MultipleNodeContextMenuFactory<TBuildingBlock>
{
   protected override IReadOnlyList<TBuildingBlock> AllTagsFor(IReadOnlyList<ITreeNode> treeNodes) =>
      treeNodes.Select(subjectFor).Where(x => x != null).OfType<TBuildingBlock>().ToList();

   private static object subjectFor(ITreeNode treeNode)
   {
      var tag = treeNode.TagAsObject;
      return (tag as IClassifiableWrapper)?.WrappedObject ?? tag;
   }
}
