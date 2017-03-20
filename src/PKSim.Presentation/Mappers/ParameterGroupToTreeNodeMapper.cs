using System;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Mappers
{
   public interface IParameterGroupToTreeNodeMapper : IMapper<IGroup, ITreeNode>
   {
      ITreeNode MapForPopulationFrom(IGroup group);
   }

   public class ParameterGroupToTreeNodeMapper : IParameterGroupToTreeNodeMapper
   {
      private readonly ITreeNodeFactory _treeNodeFactory;

      public ParameterGroupToTreeNodeMapper(ITreeNodeFactory treeNodeFactory)
      {
         _treeNodeFactory = treeNodeFactory;
      }

      public ITreeNode MapFrom(IGroup group)
      {
         return mapFrom(group, x => x.DisplayName);
      }

      public ITreeNode MapForPopulationFrom(IGroup group)
      {
         return mapFrom(group, x => x.PopDisplayName);
      }

      private ITreeNode mapFrom(IGroup group, Func<IGroup, string> display)
      {
         ITreeNode node = _treeNodeFactory.CreateFor(group);
         node.Text = display(group);
         node.Icon = ApplicationIcons.IconByName(group.IconName);

         group.Children.Where(g => g.Visible)
            .OrderBy(g => g.Sequence)
            .ThenBy(display)
            .Each(childGroup => node.AddChild(mapFrom(childGroup, display)));

         return node;
      }
   }
}