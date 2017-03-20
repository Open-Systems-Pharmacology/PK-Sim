using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.Nodes;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Repositories;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Services;
using OSPSuite.Assets;

namespace PKSim.Presentation.Nodes
{
   public interface ITreeNodeFactory : OSPSuite.Presentation.Nodes.ITreeNodeFactory
   {
      ITreeNode<TObjectBase> CreateFor<TObjectBase>(TObjectBase entity, RepresentationInfo representationInfo) where TObjectBase : class, IObjectBase;
      ITreeNode CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock);
      ITreeNode CreateFor(UsedObservedData usedObservedData);
      ITreeNode CreateFor(ClassifiableSimulation simulation);
      ITreeNode CreateFor(PartialProcess partialProcess);
      ITreeNode CreateFor(SystemicProcess systemicProcess);
      ITreeNode CreateFor(ModelProperties modelProperties);
      ITreeNode CreateFor(ClassifiableComparison classifiableComparison);
      ITreeNode CreateFor(PartialProcess partialProcess, string proteinName);
      ITreeNode<RootNodeType> CreateFor(RootNodeType rootNode);
      ITreeNode CreateFor(SystemicProcessNodeType rootNode);
      CovariateNode CreateCovariateNode(string covariateName);
      ITreeNode<IGroup> CreateGroupAll();
      ITreeNode<IGroup> CreateDynamicGroup(string key, string containerName, IEnumerable<IParameter> allParametersInDynamicGroup);
      ITreeNode<IGroup> CreateGroupFavorites();
      ITreeNode CreateFor(CompoundParameterNodeType compoundParameterNodeType);

   }

   public class TreeNodeFactory : OSPSuite.Presentation.Nodes.TreeNodeFactory, ITreeNodeFactory
   {
      private readonly IBuildingBlockRetriever _buildingBlockRetriever;

      public TreeNodeFactory(IBuildingBlockRetriever buildingBlockRetriever, IObservedDataRepository observedDataRepository, IToolTipPartCreator toolTipPartCreator) : base(observedDataRepository, toolTipPartCreator)
      {
         _buildingBlockRetriever = buildingBlockRetriever;
      }

      public ITreeNode<TObjectBase> CreateFor<TObjectBase>(TObjectBase entity, RepresentationInfo representationInfo) where TObjectBase : class, IObjectBase
      {
         var node = CreateObjectBaseNode(entity);
         node.Text = representationInfo.DisplayName;
         node.ToolTip = _toolTipPartCreator.ToolTipFor(representationInfo.Description);
         node.Icon = ApplicationIcons.IconByName(representationInfo.IconName);
         return node;
      }

      public ITreeNode CreateFor(ClassifiableComparison classifiableComparison)
      {
         return new ComparisonNode(classifiableComparison);
      }

      public ITreeNode CreateFor(ClassifiableSimulation simulation)
      {
         //for a simulation node, we only need the info defined in the sim properties
         return new SimulationNode(simulation);
      }

      public ITreeNode CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock)
      {
         var templateBuildingBlock = _buildingBlockRetriever.BuildingBlockWithId(usedBuildingBlock.TemplateId);
         return new UsedBuildingBlockInSimulationNode(simulation, usedBuildingBlock, templateBuildingBlock) {ToolTip = _toolTipPartCreator.ToolTipFor(usedBuildingBlock)};
      }    

      public ITreeNode CreateFor(PartialProcess partialProcess)
      {
         return new CompoundProcessNode(partialProcess);
      }

      public ITreeNode CreateFor(SystemicProcess systemicProcess)
      {
         return new CompoundProcessNode(systemicProcess);
      }

      public ITreeNode CreateFor(CompoundParameterNodeType compoundParameterNodeType)
      {
         return new CompoundParameterNode(compoundParameterNodeType);
      }

      public ITreeNode CreateFor(ModelProperties modelProperties)
      {
         if (modelProperties == null || modelProperties.ModelConfiguration == null)
            return null;

         var node = CreateFor(nodeText: modelProperties.ModelConfiguration.ModelName);

         node.ToolTip = _toolTipPartCreator.ToolTipFor(modelProperties);
         return node;
      }

      public ITreeNode CreateFor(PartialProcess partialProcess, string proteinName)
      {
         return new PartialProcessMoleculeNode(proteinName, partialProcess);
      }

    
      public ITreeNode<RootNodeType> CreateFor(RootNodeType rootNode)
      {
         return new RootNode(rootNode);
      }

      public ITreeNode CreateFor(SystemicProcessNodeType systemicProcessNodeType)
      {
         return new SystemProcessRootNode(systemicProcessNodeType);
      }

      public CovariateNode CreateCovariateNode(string covariateName)
      {
         return new CovariateNode(covariateName);
      }

      public ITreeNode<IGroup> CreateGroupAll()
      {
         return new GroupNode(new Group {Name = CoreConstants.Groups.ALL, DisplayName = PKSimConstants.UI.All});
      }

      public ITreeNode<IGroup> CreateDynamicGroup(string key, string containerName, IEnumerable<IParameter> allParametersInDynamicGroup)
      {
         return new GroupNode(new DynamicGroup(allParametersInDynamicGroup) {Name = key, DisplayName = containerName});
      }

      public ITreeNode<IGroup> CreateGroupFavorites()
      {
         var favoritesGroup = new Group {Name = CoreConstants.Groups.FAVORITES, DisplayName = PKSimConstants.UI.Favorites};
         return new GroupNode(favoritesGroup) {Icon = ApplicationIcons.Favorites};
      }
   }
}