using System;
using System.Collections.Generic;
using System.Linq;
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
using PKSim.Assets;
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

            addClassifications(project, ClassificationType.ExpressionProfile);
            addClassifications(project, ClassificationType.Individual);
            addClassifications(project, ClassificationType.Population);
            addClassifications(project, ClassificationType.Compound);
            addClassifications(project, ClassificationType.Formulation);
            addClassifications(project, ClassificationType.Protocol);
            addClassifications(project, ClassificationType.Event);
            addClassifications(project, ClassificationType.ObserverSet);

            var pksimProject = project.DowncastTo<PKSimProject>();
            pksimProject.All<IPKSimBuildingBlock>().Each(x => AddBuildingBlockToTree(x));

            _observedDataInExplorerPresenter.AddAllClassificationsToTree(project);
         }
      }

      private ITreeNode addEventToTree(PKSimEvent pkSimEvent) => addBuildingBlockToTree<PKSimEvent, ClassifiableEvent>(pkSimEvent, PKSimRootNodeTypes.EventFolder, ApplicationIcons.Event);

      private ITreeNode addPopulationToTree(Population population) => addBuildingBlockToTree<Population, ClassifiablePopulation>(population, PKSimRootNodeTypes.PopulationFolder, ApplicationIcons.Population);

      private ITreeNode addIndividualToTree(Individual individual) => addBuildingBlockToTree<Individual, ClassifiableIndividual>(individual, PKSimRootNodeTypes.IndividualFolder, ApplicationIcons.IconByName(individual.Icon));

      private ITreeNode addCompoundToTree(Compound compound) => addBuildingBlockToTree<Compound, ClassifiableCompound>(compound, PKSimRootNodeTypes.CompoundFolder, ApplicationIcons.Compound);

      private ITreeNode addFormulationToTree(Formulation formulation) => addBuildingBlockToTree<Formulation, ClassifiableFormulation>(formulation, PKSimRootNodeTypes.FormulationFolder, ApplicationIcons.Formulation);

      private ITreeNode addProtocolToTree(Protocol protocol) => addBuildingBlockToTree<Protocol, ClassifiableProtocol>(protocol, PKSimRootNodeTypes.ProtocolFolder, ApplicationIcons.Protocol);

      private ITreeNode addObserverSetToTree(ObserverSet observerSet) => addBuildingBlockToTree<ObserverSet, ClassifiableObserverSet>(observerSet, PKSimRootNodeTypes.ObserverSetFolder, ApplicationIcons.Observer);

      private ITreeNode addExpressionProfileToTree(ExpressionProfile expressionProfile) => addBuildingBlockToTree<ExpressionProfile, ClassifiableExpressionProfile>(expressionProfile, PKSimRootNodeTypes.ExpressionProfileFolder, ApplicationIcons.IconByName(expressionProfile.Icon));

      private ITreeNode addBuildingBlockToTree<TBuildingBlock, TClassifiable>(TBuildingBlock buildingBlock, RootNodeType buildingBlockFolderType, ApplicationIcon icon) where TBuildingBlock : class, IPKSimBuildingBlock
         where TClassifiable : Classifiable<TBuildingBlock>, new()
      {
         return AddSubjectToClassifyToTree<TBuildingBlock, TClassifiable>(buildingBlock,
            classifiable => AddClassifiableToTree(classifiable, buildingBlockFolderType,
               (classificationNode, classifiableToAdd) =>
               {
                  var node = _treeNodeFactory.CreateForClassifiableBuildingBlock(classifiableToAdd).WithIcon(icon);
                  AddClassifiableNodeToView(node, classificationNode);
                  return node;
               }));
      }

      private void addClassifications(IProject project, ClassificationType classificationType) =>
         _classificationPresenter.AddClassificationsToTree(project.AllClassificationsByType(classificationType));


      public override void AddToClassificationTree(ITreeNode<IClassification> parentNode, string category)
      {
         switch (parentNode.Tag.ClassificationType)
         {
            case ClassificationType.ObservedData:
               _observedDataInExplorerPresenter.GroupObservedDataByCategory(parentNode, category);
               break;
            case ClassificationType.Individual:
               _classificationPresenter.GroupClassificationsByCategory<ClassifiableIndividual>(parentNode, category, x => categoryValueForIndividual(x, category));
               break;
            case ClassificationType.ExpressionProfile:
               _classificationPresenter.GroupClassificationsByCategory<ClassifiableExpressionProfile>(parentNode, category, x => categoryValueForExpressionProfile(x, category));
               break;
         }
      }

      private string categoryValueForIndividual(ClassifiableIndividual classifiable, string category)
      {
         if (string.Equals(category, PKSimConstants.Classifications.Species))
            return classifiable.Individual.Species?.DisplayName;

         return string.Empty;
      }

      private string categoryValueForExpressionProfile(ClassifiableExpressionProfile classifiable, string category)
      {
         if (string.Equals(category, PKSimConstants.Classifications.Molecule))
            return classifiable.ExpressionProfile.MoleculeName;

         return string.Empty;
      }

      public override bool RemoveDataUnderClassification(ITreeNode<IClassification> classificationNode)
      {
         if (classificationNode.Tag.ClassificationType == ClassificationType.ObservedData)
            return _observedDataInExplorerPresenter.RemoveObservedDataUnder(classificationNode);

         var buildingBlocks = classificationNode.AllLeafNodes
            .Select(x => x.TagAsObject)
            .OfType<IClassifiableWrapper>()
            .Select(x => x.WrappedObject)
            .OfType<IPKSimBuildingBlock>()
            .ToList();

         return _buildingBlockTask.Delete(buildingBlocks);
      }

      public override IEnumerable<ClassificationTemplate> AvailableClassificationCategories(ITreeNode<IClassification> parentClassificationNode)
      {
         switch (parentClassificationNode.Tag.ClassificationType)
         {
            case ClassificationType.ObservedData:
               return _observedDataInExplorerPresenter.AvailableObservedDataCategoriesIn(parentClassificationNode);
            case ClassificationType.Individual:
               return new[] {new ClassificationTemplate(PKSimConstants.Classifications.Species)};
            case ClassificationType.ExpressionProfile:
               return new[] {new ClassificationTemplate(PKSimConstants.Classifications.Molecule)};
            default:
               return Enumerable.Empty<ClassificationTemplate>();
         }
      }

      public override bool CanDrag(ITreeNode node)
      {
         if (node == null)
            return false;

         if (node.IsAnImplementationOf<ClassificationNode>())
            return true;

         if (node.TagAsObject is IClassifiableWrapper wrapper && wrapper.WrappedObject is IPKSimBuildingBlock)
            return true;

         return _observedDataInExplorerPresenter.CanDrag(node);
      }

      public override bool CopyAllowed() => false;

      public override void NodeDoubleClicked(ITreeNode node)
      {
         if (node.TagAsObject is IClassifiableWrapper wrapper && wrapper.WrappedObject is IPKSimBuildingBlock buildingBlock)
         {
            EditBuildingBlock(buildingBlock);
            return;
         }

         base.NodeDoubleClicked(node);
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
