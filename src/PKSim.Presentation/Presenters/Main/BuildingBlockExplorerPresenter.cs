using System;
using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Classifications;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Presenters.ObservedData;
using OSPSuite.Presentation.Regions;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Regions;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Main;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Presenters.Main
{
   public interface IBuildingBlockExplorerPresenter : IExplorerPresenter,
      IListener<SwapBuildingBlockEvent>,
      IListener<RenamedEvent>
   {
   }

   public class BuildingBlockExplorerPresenter : ExplorerPresenter<IBuildingBlockExplorerView, IBuildingBlockExplorerPresenter>, IBuildingBlockExplorerPresenter
   {
      private readonly IObservedDataInExplorerPresenter _observedDataInExplorerPresenter;

      public BuildingBlockExplorerPresenter(IBuildingBlockExplorerView view, ITreeNodeFactory treeNodeFactory, ITreeNodeContextMenuFactory treeNodeContextMenuFactory, IMultipleTreeNodeContextMenuFactory multipleTreeNodeContextMenuFactory, IBuildingBlockIconRetriever buildingBlockIconRetriever,
         IRegionResolver regionResolver, IBuildingBlockTask buildingBlockTask, IToolTipPartCreator toolTipPartCreator, IProjectRetriever projectRetriever, IClassificationPresenter classificationPresenter, IObservedDataInExplorerPresenter observedDataInExplorerPresenter)
         : base(view, treeNodeFactory, treeNodeContextMenuFactory, multipleTreeNodeContextMenuFactory, buildingBlockIconRetriever, regionResolver, buildingBlockTask, RegionNames.BuildingBlockExplorer, projectRetriever, classificationPresenter, toolTipPartCreator)
      {
         _observedDataInExplorerPresenter = observedDataInExplorerPresenter;
         _observedDataInExplorerPresenter.InitializeWith(this, classificationPresenter, RootNodeTypes.ObservedDataFolder);
      }

      protected override ITreeNode AddBuildingBlockToTree(IPKSimBuildingBlock buildingBlock)
      {
         switch (buildingBlock)
         {
            case Compound compound:
               return addCompoundToTree(compound);
            case Formulation formulation:
               return addFormulationToTree(formulation);
            case Protocol protocol:
               return addProtocolToTree(protocol);
            case Individual individual:
               return addIndividualToTree(individual);
            case Population population:
               return addPopulationToTree(population);
            case PKSimEvent pksimEvent:
               return addEventToTree(pksimEvent);
            case ObserverSet observerSet:
               return addObserverSetToTree(observerSet);
            case ExpressionProfile expressionProfile:
               return addExpressionProfileToTree(expressionProfile);
            case Simulation _:
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

            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.ExpressionProfileFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.PopulationFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.CompoundFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.FormulationFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.ProtocolFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.EventFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.ObserverSetFolder));
            _view.AddNode(_treeNodeFactory.CreateFor(RootNodeTypes.ObservedDataFolder));

            var pksimProject = project.DowncastTo<PKSimProject>();
            pksimProject.All<IPKSimBuildingBlock>().Each(x => AddBuildingBlockToTree(x));

            _observedDataInExplorerPresenter.AddObservedDataToTree(project);
         }
      }

      private ITreeNode addEventToTree(PKSimEvent pkSimEvent) => addBuildingBlockToTree(pkSimEvent, PKSimRootNodeTypes.EventFolder, ApplicationIcons.Event);

      private ITreeNode addPopulationToTree(Population population) => addBuildingBlockToTree(population, PKSimRootNodeTypes.PopulationFolder, ApplicationIcons.Population);

      private ITreeNode addIndividualToTree(Individual individual) => addBuildingBlockToTree(individual, PKSimRootNodeTypes.IndividualFolder, ApplicationIcons.IconByName(individual.Icon));

      private ITreeNode addCompoundToTree(Compound compound) => addBuildingBlockToTree(compound, PKSimRootNodeTypes.CompoundFolder, ApplicationIcons.Compound);

      private ITreeNode addFormulationToTree(Formulation formulation) => addBuildingBlockToTree(formulation, PKSimRootNodeTypes.FormulationFolder, ApplicationIcons.Formulation);

      private ITreeNode addProtocolToTree(Protocol protocol) => addBuildingBlockToTree(protocol, PKSimRootNodeTypes.ProtocolFolder, ApplicationIcons.Protocol);

      private ITreeNode addObserverSetToTree(ObserverSet observerSet) => addBuildingBlockToTree(observerSet, PKSimRootNodeTypes.ObserverSetFolder, ApplicationIcons.Observer);

      private ITreeNode addExpressionProfileToTree(ExpressionProfile expressionProfile) => addBuildingBlockToTree(expressionProfile, PKSimRootNodeTypes.ExpressionProfileFolder, ApplicationIcons.IconByName(expressionProfile.Icon));

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
         //need to remove the node of the old protocol when a protocol swap is happening as the normal remove building block event is not triggered
         RemoveNodeFor(eventToHandle.OldBuildingBlock);
      }

      public void Handle(RenamedEvent renamedEvent)
      {
         handleRenamedBuildingBlock(renamedEvent);
         handleRenamedObservedData(renamedEvent);
      }

      private void handleRenamedObservedData(RenamedEvent renamedEvent)
      {
         var observedData = renamedEvent.RenamedObject as DataRepository;
         if (observedData == null)
            return;

         RefreshTreeAfterRename();
      }

      private void handleRenamedBuildingBlock(RenamedEvent renamedEvent)
      {
         var buildingBlock = renamedEvent.RenamedObject as IPKSimBuildingBlock;
         if (buildingBlock == null)
            return;

         if (buildingBlock.IsAnImplementationOf<Simulation>())
            return;

         RefreshTreeAfterRename();
      }
   }
}