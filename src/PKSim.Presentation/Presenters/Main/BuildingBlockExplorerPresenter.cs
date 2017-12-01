using System;
using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Regions;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Main;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters.Classifications;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Presenters.ObservedData;
using OSPSuite.Presentation.Regions;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Presenters.Main
{
   public interface IBuildingBlockExplorerPresenter : IExplorerPresenter,
      IListener<SwapBuildingBlockEvent>
   {
   }

   public class BuildingBlockExplorerPresenter : ExplorerPresenter<IBuildingBlockExplorerView, IBuildingBlockExplorerPresenter>, IBuildingBlockExplorerPresenter
   {
      private readonly IObservedDataInExplorerPresenter _observedDataInExplorerPresenter;

      public BuildingBlockExplorerPresenter(IBuildingBlockExplorerView view, ITreeNodeFactory treeNodeFactory, ITreeNodeContextMenuFactory treeNodeContextMenuFactory, IMultipleTreeNodeContextMenuFactory multipleTreeNodeContextMenuFactory, IBuildingBlockIconRetriever buildingBlockIconRetriever, IRegionResolver regionResolver, IBuildingBlockTask buildingBlockTask, IToolTipPartCreator toolTipPartCreator, IProjectRetriever projectRetriever, IClassificationPresenter classificationPresenter, IObservedDataInExplorerPresenter observedDataInExplorerPresenter)
         : base(view, treeNodeFactory, treeNodeContextMenuFactory, multipleTreeNodeContextMenuFactory, buildingBlockIconRetriever, regionResolver, buildingBlockTask, RegionNames.BuildingBlockExplorer, projectRetriever, classificationPresenter, toolTipPartCreator)
      {
         _observedDataInExplorerPresenter = observedDataInExplorerPresenter;
         _observedDataInExplorerPresenter.InitializeWith(this, classificationPresenter,RootNodeTypes.ObservedDataFolder);
      }

      protected override ITreeNode AddBuildingBlockToTree(IPKSimBuildingBlock buildingBlock)
      {
         switch (buildingBlock.BuildingBlockType)
         {
            case PKSimBuildingBlockType.Compound:
               return addCompoundToTree(buildingBlock.DowncastTo<Compound>());
            case PKSimBuildingBlockType.Formulation:
               return addFormulationToTree(buildingBlock.DowncastTo<Formulation>());
            case PKSimBuildingBlockType.Protocol:
               return addProtocolToTree(buildingBlock.DowncastTo<Protocol>());
            case PKSimBuildingBlockType.Individual:
               return addIndividualToTree(buildingBlock.DowncastTo<Individual>());
            case PKSimBuildingBlockType.Population:
               return addPopulationToTree(buildingBlock.DowncastTo<Population>());
            case PKSimBuildingBlockType.Event:
               return addEventToTree(buildingBlock.DowncastTo<PKSimEvent>());
            case PKSimBuildingBlockType.Simulation:
               return null;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      protected override void AddProjectToTree(IProject project)
      {
         using (new BatchUpdate(_view))
         {
            _view.DestroyNodes();

            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.PopulationFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.CompoundFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.FormulationFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.ProtocolFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.EventFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(RootNodeTypes.ObservedDataFolder));

            var pksimProject = project.DowncastTo<PKSimProject>();
            pksimProject.All<Individual>().Each(bb => addIndividualToTree(bb));
            pksimProject.All<Compound>().Each(bb => addCompoundToTree(bb));
            pksimProject.All<Protocol>().Each(bb => addProtocolToTree(bb));
            pksimProject.All<Formulation>().Each(bb => addFormulationToTree(bb));
            pksimProject.All<Population>().Each(bb => addPopulationToTree(bb));
            pksimProject.All<PKSimEvent>().Each(bb => addEventToTree(bb));

            _observedDataInExplorerPresenter.AddObservedDataToTree(project);
         }
      }

      private ITreeNode addEventToTree(PKSimEvent pkSimEvent)
      {
         return addBuildingBlockToTree(pkSimEvent, PKSimRootNodeTypes.EventFolder, ApplicationIcons.Event);
      }

      private ITreeNode addPopulationToTree(Population population)
      {
         return addBuildingBlockToTree(population, PKSimRootNodeTypes.PopulationFolder, ApplicationIcons.Population);
      }

      private ITreeNode addIndividualToTree(Individual individual)
      {
         return addBuildingBlockToTree(individual, PKSimRootNodeTypes.IndividualFolder, ApplicationIcons.IconByName(individual.Species.Icon));
      }

      private ITreeNode addCompoundToTree(Compound compound)
      {
         return addBuildingBlockToTree(compound, PKSimRootNodeTypes.CompoundFolder, ApplicationIcons.Compound);
      }

      private ITreeNode addFormulationToTree(Formulation formulation)
      {
         return addBuildingBlockToTree(formulation, PKSimRootNodeTypes.FormulationFolder, ApplicationIcons.Formulation);
      }

      private ITreeNode addProtocolToTree(Protocol protocol)
      {
         return addBuildingBlockToTree(protocol, PKSimRootNodeTypes.ProtocolFolder, ApplicationIcons.Protocol);
      }

      private ITreeNode addBuildingBlockToTree<TBuildingBlock>(TBuildingBlock buildingBlock, RootNodeType buildingBlockFolderType, ApplicationIcon icon) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         var buildingBockFolderNode = _view.NodeByType(buildingBlockFolderType);

         return _view.AddNode(_treeNodeFactory.CreateFor(buildingBlock)
            .WithIcon(icon)
            .Under(buildingBockFolderNode));
      }

      public override void AddToClassificationTree(ITreeNode<IClassification> parentNode, string category)
      {
         _observedDataInExplorerPresenter.GroupObservedDataByCategory(parentNode, category);
      }

      public override bool RemoveDataUnderClassification(ITreeNode<IClassification> classificationNode)
      {
         return _observedDataInExplorerPresenter.RemoveObservedDataUnder(classificationNode);
      }

      public override IEnumerable<ClassificationTemplate> AvailableClassificationCategories(ITreeNode<IClassification> parentClassificationNode)
      {
         return _observedDataInExplorerPresenter.AvailableObservedDataCategoriesIn(parentClassificationNode);
      }

      public override bool CanDrag(ITreeNode node)
      {
         if (node == null)
            return false;

         if (node.IsAnImplementationOf<ClassificationNode>())
            return true;

         return _observedDataInExplorerPresenter.CanDrag(node);
      }

      public void Handle(SwapBuildingBlockEvent eventToHandle)
      {
         //need to remove the node of the old protocol when a protocal swap is happening as the nomral remove building block event is not triggered
         RemoveNodeFor(eventToHandle.OldBuildingBlock);
      }
   }
}