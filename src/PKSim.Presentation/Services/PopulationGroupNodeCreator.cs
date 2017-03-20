using System.Collections.Generic;
using System.Linq;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Services
{
   public interface IPopulationGroupNodeCreator
   {
      ITreeNode CreateGroupNodeFor(IGroup rootGroup, IReadOnlyCollection<IParameter> allParameters);
      ITreeNode CreateCovariateNodeFor(string covariate);
   }

   public class PopulationGroupNodeCreator : IPopulationGroupNodeCreator
   {
      private readonly IParameterGroupTask _parameterGroupTask;
      private readonly IParameterGroupNodeCreator _parameterGroupNodeCreator;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly IToolTipPartCreator _toolTipPartCreator;
      private readonly IPathToPathElementsMapper _pathElementsMapper;
      private readonly ITreeNodeFactory _treeNodeFactory;

      public PopulationGroupNodeCreator(IParameterGroupNodeCreator parameterGroupNodeCreator, IParameterGroupTask parameterGroupTask,
         ITreeNodeFactory treeNodeFactory, IFullPathDisplayResolver fullPathDisplayResolver,
         IToolTipPartCreator toolTipPartCreator, IPathToPathElementsMapper pathElementsMapper)
      {
         _parameterGroupNodeCreator = parameterGroupNodeCreator;
         _parameterGroupTask = parameterGroupTask;
         _treeNodeFactory = treeNodeFactory;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _toolTipPartCreator = toolTipPartCreator;
         _pathElementsMapper = pathElementsMapper;
      }

      public ITreeNode CreateGroupNodeFor(IGroup rootGroup, IReadOnlyCollection<IParameter> allParameters)
      {
         var rootNode = _parameterGroupNodeCreator.MapForPopulationFrom(rootGroup, allParameters);

         foreach (var groupNode in rootNode.AllNodes.ToList())
         {
            var group = groupNode.TagAsObject.DowncastTo<IGroup>();
            addNodesForParametersUnder(groupNode, allParametersIn(group, allParameters));
         }

         return rootNode;
      }

      public ITreeNode CreateCovariateNodeFor(string covariate)
      {
         //uses the covariate name as id!
         return _treeNodeFactory.CreateCovariateNode(covariate);
      }

      private IEnumerable<IParameter> allParametersIn(IGroup group, IEnumerable<IParameter> allParameters)
      {
         if (group.Name.IsOneOf(CoreConstants.Groups.COMPOUND, CoreConstants.Groups.COMPOUND_PROCESSES))
            return _parameterGroupTask.ParametersInTopGroup(group, allParameters);

         return _parameterGroupTask.ParametersIn(group, allParameters);
      }

      private bool parameterHaveSameDisplayNamesAsOneParentGroup(IEnumerable<PathElements> allPathElements, ITreeNode treeNode)
      {
         //if the display name of the parameters are equal to the group display name, no need to add a container as well
         var allDisplayNames = allPathElements.Select(x => x[PathElement.Name].DisplayName).Distinct().ToList();

         //more than one display names in parameters? return true 
         if (allDisplayNames.Count != 1) return false;

         var displayName = allDisplayNames[0];
         while (treeNode != null)
         {
            if (string.Equals(treeNode.Text, displayName))
               return true;

            treeNode = treeNode.ParentNode;
         }
         return false;
      }

      private void addNodesForParametersUnder(ITreeNode node, IEnumerable<IParameter> allParameters)
      {
         var cache = new Cache<PathElements, IParameter>(_pathElementsMapper.MapFrom);
         cache.AddRange(allParameters);

         if (parameterHaveSameDisplayNamesAsOneParentGroup(cache.Keys, node))
            createNodeHiearchyWithoutParameterNameUnder(node, cache);
         else
            createNodeHiearchyWithParameterNameUnder(node, cache);
      }

      private void createNodeHiearchyWithoutParameterNameUnder(ITreeNode groupNode, Cache<PathElements, IParameter> allPathElementByParameters)
      {
         createNodeHiearchyUnder(groupNode, allPathElementByParameters, allPathElementByParameters.Keys, new[] {PathElement.TopContainer, PathElement.Container, PathElement.BottomCompartment});
      }

      private void createNodeHiearchyWithParameterNameUnder(ITreeNode groupNode, Cache<PathElements, IParameter> allPathElementByParameters)
      {
         createNodeHiearchyUnder(groupNode, allPathElementByParameters, allPathElementByParameters.Keys, new[] {PathElement.Name, PathElement.TopContainer, PathElement.Container, PathElement.BottomCompartment});
      }

      private void createNodeHiearchyUnder(ITreeNode node, Cache<PathElements, IParameter> allPathElementByParameters, IEnumerable<PathElements> pathElements, IReadOnlyList<PathElement> pathElementStructure)
      {
         var currentPathElementStructure = pathElementStructure.ToList();

         //No more node structure to create, we add the parameters under the given node
         if (!currentPathElementStructure.Any())
         {
            pathElements.Each(pathElement => addParameterNode(node, pathElement, allPathElementByParameters));
            return;
         }

         var currentPathElement = currentPathElementStructure[0];
         currentPathElementStructure.Remove(currentPathElement);

         foreach (var pathElementByPosition in pathElements.GroupBy(x => x[currentPathElement]))
         {
            var topNode = node;
            var pathElementDTO = pathElementByPosition.Key;
            if (shouldAddPathElementToNodeHiearchy(pathElementDTO, pathElementByPosition.ElementAt(0)))
            {
               topNode = createPathElementNodeFor(node, pathElementDTO);
            }

            createNodeHiearchyUnder(topNode, allPathElementByParameters, pathElementByPosition, currentPathElementStructure);
         }
      }

      private ITreeNode createPathElementNodeFor(ITreeNode node, PathElementDTO pathElementDTO)
      {
         var pathElementNode = _treeNodeFactory.CreateFor(pathElementDTO.DisplayName, string.Format("{0}-{1}", node.Id, pathElementDTO.DisplayName), pathElementDTO.IconName);

         var existingNode = node.Children.Find(n => Equals(n.Id, pathElementNode.Id));
         if (existingNode != null)
            return existingNode;

         node.AddChild(pathElementNode);
         return pathElementNode;
      }

      private bool shouldAddPathElementToNodeHiearchy(PathElementDTO pathElementDTO, PathElements pathElements)
      {
         return !string.IsNullOrEmpty(pathElementDTO.DisplayName)
                && !pathElementDTO.DisplayName.IsOneOf(Constants.ORGANISM, Constants.NEIGHBORHOODS, CoreConstants.Organ.Lumen, Constants.APPLICATIONS, Constants.EVENTS)
                && !Equals(parameterDisplayElementDTOFor(pathElements), pathElementDTO); //means that the parameter won't have the same display name as the parent node
      }

      private void addParmeterNode(ITreeNode node, IParameter parameter, PathElementDTO pathElementDTO)
      {
         var representation = new RepresentationInfo {DisplayName = pathElementDTO.DisplayName, IconName = pathElementDTO.IconName};
         var parameterNode = _treeNodeFactory.CreateFor(parameter, representation);
         parameterNode.ToolTip = _toolTipPartCreator.ToolTipFor(_fullPathDisplayResolver.FullPathFor(parameter));
         node.AddChild(parameterNode);
      }

      private void addParameterNode(ITreeNode node, PathElements pathElements, Cache<PathElements, IParameter> allPathElementByParameters)
      {
         var parameter = allPathElementByParameters[pathElements];
         addParmeterNode(node, parameter, parameterDisplayElementDTOFor(pathElements));
      }

      private PathElementDTO parameterDisplayElementDTOFor(PathElements pathElements)
      {
         return pathElementAt(pathElements, PathElement.Molecule) ??
                pathElementAt(pathElements, PathElement.BottomCompartment) ??
                pathElementAt(pathElements, PathElement.Container) ??
                pathElementAt(pathElements, PathElement.Name);
      }

      private PathElementDTO pathElementAt(PathElements pathElements, PathElement pathElement)
      {
         var dto = pathElements[pathElement];
         return string.IsNullOrEmpty(dto.DisplayName) ? null : dto;
      }
   }
}