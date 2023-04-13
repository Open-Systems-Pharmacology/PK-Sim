using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Assets;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Presentation.Mappers
{
   public interface IDynamicGroupExpander
   {
      void AddDynamicGroupNodesTo(ITreeNode node, IReadOnlyCollection<IParameter> allParameters);
   }

   public class DynamicGroupExpander : IDynamicGroupExpander
   {
      private readonly IParameterGroupTask _parameterGroupTask;
      private readonly ITreeNodeFactory _treeNodeFactory;

      public DynamicGroupExpander(IParameterGroupTask parameterGroupTask, ITreeNodeFactory treeNodeFactory)
      {
         _parameterGroupTask = parameterGroupTask;
         _treeNodeFactory = treeNodeFactory;
      }

      public void AddDynamicGroupNodesTo(ITreeNode node, IReadOnlyCollection<IParameter> allParameters)
      {
         var group = node.TagAsObject as IGroup;
         if (group == null) return;

         if (group.IsNamed(CoreConstants.Groups.COMPOUND))
            addCompoundNodeTo(node, allParameters);

         if (group.IsNamed(CoreConstants.Groups.COMPOUND_PROCESSES))
            addCompoundProcessesNodeTo(node, allParameters);

         if (group.IsNamed(CoreConstants.Groups.RELATIVE_EXPRESSION))
            addRelativeExpressionsNodeTo(node, allParameters);

         if (group.IsNamed(CoreConstants.Groups.PROTOCOL))
            addCompoundApplicationTo(node, allParameters);
      }

      private string closestAppliedMolecule(IParameter parameter)
      {
         var protocolSchemaItemContainer = parameter.ParentContainer;
         var applicationContainer = protocolSchemaItemContainer.ParentContainer;
         //for now, the applied molecule amount is assumed to be under the application container (
         var firstMoleculeAmount = applicationContainer.GetAllChildren<MoleculeAmount>().First();
         return firstMoleculeAmount.Name;
      }

      private void addCompoundProcessesNodeTo(ITreeNode node, IEnumerable<IParameter> allParameters)
      {
         addDynamicParameterGroupNodeTo(node, CoreConstants.Groups.COMPOUND_PROCESS_ITEM, allParameters, compoundProcessName);
      }

      private void addCompoundApplicationTo(ITreeNode node, IEnumerable<IParameter> allParameters)
      {
         addDynamicParameterGroupNodeTo(node, CoreConstants.Groups.PROTOCOL_ITEM, allParameters, closestAppliedMolecule);
      }

      private void addCompoundNodeTo(ITreeNode node, IEnumerable<IParameter> allParameters)
      {
         addDynamicParameterGroupNodeTo(node, CoreConstants.Groups.COMPOUND_ITEM, allParameters, parentContainerName);
      }

      private void addRelativeExpressionsNodeTo(ITreeNode node, IEnumerable<IParameter> allParameters)
      {
         addDynamicParameterGroupNodeTo(node, CoreConstants.Groups.RELATIVE_EXPRESSION_ITEM, allParameters, parentContainerName, isWellDefinedMolecule);
      }

      private bool isWellDefinedMolecule(string moleculeName)
      {
         return !moleculeName.IsUndefinedMolecule();
      }

      private void addDynamicParameterGroupNodeTo(ITreeNode node, string subGroupName, IEnumerable<IParameter> allParameters, Func<IParameter, string> keySelector)
      {
         addDynamicParameterGroupNodeTo(node, subGroupName, allParameters, keySelector, x => true);
      }

      private void addDynamicParameterGroupNodeTo(ITreeNode node, string subGroupName, IEnumerable<IParameter> allParameters, Func<IParameter, string> keySelector, Func<string, bool> keyAvailableFunc)
      {
         var group = node.TagAsObject.DowncastTo<IGroup>();
         var allGroupParameters = _parameterGroupTask.ParametersInTopGroup(group.Name, allParameters);

         foreach (var parametersForNewGroup in allGroupParameters.GroupBy(keySelector).OrderBy(x => x.Key))
         {
            if (!keyAvailableFunc(parametersForNewGroup.Key))
               continue;

            var compositeKey = CompositeNameFor(subGroupName, parametersForNewGroup.Key);
            var parameterContainer = parametersForNewGroup.First().ParentContainer;
            var dynamicNode = _treeNodeFactory.CreateDynamicGroup(compositeKey, parametersForNewGroup.Key, parametersForNewGroup);
            dynamicNode.Icon = ApplicationIcons.IconByNameOrDefault(parameterContainer.Icon, node.Icon);
            node.AddChild(dynamicNode);
         }
      }

      private string parentContainerName(IParameter parameter)
      {
         return parameter.ParentContainer.Name;
      }

      private string compoundProcessName(IParameter parameter)
      {
         var process = parameter.ParentContainer;
         var processName = process.Name;

         if (process.ContainerType == ContainerType.Reaction || process.ParentContainer == null)
            return processName;

         return CompositeNameFor(process.ParentContainer.Name, processName);
      }
   }
}