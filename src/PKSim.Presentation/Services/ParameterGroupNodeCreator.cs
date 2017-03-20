using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Mappers;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Services
{
   public interface IParameterGroupNodeCreator
   {
      ITreeNode MapForPopulationFrom(IGroup group, IReadOnlyCollection<IParameter> allParameters);

      ITreeNode MapFrom(IGroup group, IReadOnlyCollection<IParameter> allParameters);
   }

   public class ParameterGroupNodeCreator : IParameterGroupNodeCreator
   {
      private readonly IParameterGroupToTreeNodeMapper _parameterGroupToTreeNodeMapper;
      private readonly IDynamicGroupExpander _dynamicGroupExpander;

      public ParameterGroupNodeCreator(IParameterGroupToTreeNodeMapper parameterGroupToTreeNodeMapper, IDynamicGroupExpander dynamicGroupExpander)
      {
         _parameterGroupToTreeNodeMapper = parameterGroupToTreeNodeMapper;
         _dynamicGroupExpander = dynamicGroupExpander;
      }

      public ITreeNode MapForPopulationFrom(IGroup group, IReadOnlyCollection<IParameter> allParameters)
      {
         return mapGroup(group, allParameters, _parameterGroupToTreeNodeMapper.MapForPopulationFrom, addDynamicNode: false, removeEmptyGroups: false);
      }

      public ITreeNode MapFrom(IGroup group, IReadOnlyCollection<IParameter> allParameters)
      {
         return mapGroup(group, allParameters, _parameterGroupToTreeNodeMapper.MapFrom, addDynamicNode: true, removeEmptyGroups: true);
      }

      private ITreeNode mapGroup(IGroup group, IReadOnlyCollection<IParameter> allParameters, Func<IGroup, ITreeNode> mapper, bool addDynamicNode, bool removeEmptyGroups)
      {
         var groupNode = mapper(group);
         if (removeEmptyGroups)
         {
            var allGroupsWithParameters = allParameters.Select(x => x.GroupName).Distinct().ToList();
            removeParameterLessNodes(groupNode, allGroupsWithParameters);
         }

         if (addDynamicNode)
            groupNode.AllLeafNodes.ToList().Each(n => _dynamicGroupExpander.AddDynamicGroupNodesTo(n, allParameters));

         return groupNode;
      }

      private ITreeNode removeParameterLessNodes(ITreeNode rootNode, IReadOnlyCollection<string> allGroupsWithParameters)
      {
         if (!allGroupsWithParameters.Any())
            return rootNode;

         var allLeafNodes = rootNode.AllLeafNodes.ToList();
         if (allLeafNodes.Count == 0)
            return rootNode;

         //one element that has no parameters
         if (allLeafNodes.ElementAt(0) == rootNode)
            return rootNode;

         bool nodeWasRemoved = false;
         foreach (var groupNode in allLeafNodes)
         {
            //node contains parameters=>continue
            if (groupNodeHasParameters(groupNode, allGroupsWithParameters))
               continue;

            //This is the root node, nothing to do
            if (groupNode.ParentNode == null)
               continue;

            groupNode.ParentNode.RemoveChild(groupNode);
            nodeWasRemoved = true;
         }

         //did we remove any leaf node? if not, return the root node.
         //if yes, trim the node again
         if (nodeWasRemoved)
            return removeParameterLessNodes(rootNode, allGroupsWithParameters);

         return rootNode;
      }

      private bool groupNodeHasParameters(ITreeNode groupNode, IEnumerable<string> allGroupsWithParameters)
      {
         var group = groupNode.TagAsObject.DowncastTo<IGroup>();
         return allGroupsWithParameters.Contains(group.Name);
      }
   }
}