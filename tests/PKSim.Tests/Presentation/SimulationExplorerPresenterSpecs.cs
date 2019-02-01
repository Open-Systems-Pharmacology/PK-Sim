using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Classifications;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Regions;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Main;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationExplorerPresenter : ContextSpecification<ISimulationExplorerPresenter>
   {
      private IBuildingBlockIconRetriever _buildingBlockIconRetriever;
      protected ISimulationExplorerView _view;
      protected ITreeNodeFactory _treeNodeFactory;
      protected ITreeNodeContextMenuFactory _contextMenuFactory;
      protected Simulation _simulation;
      protected Simulation _populationSimulation;
      protected SimulationNode _individualSimulationNode;
      protected SimulationNode _populationSimulationNode;
      protected PKSimProject _project;
      protected UsedBuildingBlockInSimulationNode _usedBuildingBlockNode;
      private IRegionResolver _regionResolver;
      protected IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      protected UsedBuildingBlock _usedCompoundBuildingBlock;
      protected IBuildingBlockTask _buildingBlockTask;
      protected UsedObservedData _usedObservedData;
      private IToolTipPartCreator _toolTipNodeCreator;
      protected IProjectRetriever _projectRetriever;
      protected IClassificationPresenter _classificationPresenter;
      private Simulation _importedSimulaton;
      protected SimulationNode _importedSimulationNode;
      protected ComparisonNode _comparisonNode;
      protected RootNode _simulationFolderNode;
      protected RootNode _comparisonFolderNode;
      private IMultipleTreeNodeContextMenuFactory _multipleTreeNodeContextMenuFactory;
      protected IPKSimBuildingBlock _templageCompoundBuildingBlock;
      protected ITreeNode _usedObservedDataNode;
      private IParameterAnalysablesInExplorerPresenter _parameterAnalysablesInExplorerPresenter;
      protected IObservedDataInSimulationManager _observedDataInSimulationManager;

      protected override void Context()
      {
         _buildingBlockIconRetriever = A.Fake<IBuildingBlockIconRetriever>();
         _view = A.Fake<ISimulationExplorerView>();
         A.CallTo(() => _view.TreeView).Returns(A.Fake<IUxTreeView>());
         _treeNodeFactory = A.Fake<ITreeNodeFactory>();
         _contextMenuFactory = A.Fake<ITreeNodeContextMenuFactory>();
         _regionResolver = A.Fake<IRegionResolver>();
         _buildingBlockInSimulationManager = A.Fake<IBuildingBlockInSimulationManager>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _toolTipNodeCreator = A.Fake<IToolTipPartCreator>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _multipleTreeNodeContextMenuFactory = A.Fake<IMultipleTreeNodeContextMenuFactory>();
         _parameterAnalysablesInExplorerPresenter = A.Fake<IParameterAnalysablesInExplorerPresenter>();

         _simulationFolderNode = new RootNode(RootNodeTypes.SimulationFolder);
         _comparisonFolderNode = new RootNode(RootNodeTypes.ComparisonFolder);
         _project = new PKSimProject();
         _usedObservedData = new UsedObservedData {Id = "UsedData"};
         _simulation = new IndividualSimulation().WithName("individualSimulation").WithId("individualSimulation");
         _populationSimulation = new PopulationSimulation().WithName("populationSimulation").WithId("populationSimulation");
         _importedSimulaton = A.Fake<Simulation>().WithName("ImportedSimulation").WithId("ImportedSimulation");
         A.CallTo(() => _importedSimulaton.IsImported).Returns(true);
         _simulation.Properties = new SimulationProperties();
         _simulation.ModelProperties = new ModelProperties();
         _simulation.ModelConfiguration = A.Fake<ModelConfiguration>();
         _populationSimulation.Properties = new SimulationProperties();
         _populationSimulation.ModelProperties = new ModelProperties();
         _populationSimulation.ModelConfiguration = A.Fake<ModelConfiguration>();
         _classificationPresenter = A.Fake<IClassificationPresenter>();
         _project.AddBuildingBlock(_simulation);
         _project.AddBuildingBlock(_populationSimulation);
         var classifiableIndividualSimulation = new ClassifiableSimulation {Subject = _simulation};
         var classfiablePopulationSimulation = new ClassifiableSimulation {Subject = _populationSimulation};
         var classifiableImportSimulation = new ClassifiableSimulation {Subject = _importedSimulaton};
         _project.AddClassifiable(classifiableIndividualSimulation);
         _project.AddClassifiable(classfiablePopulationSimulation);
         _individualSimulationNode = new SimulationNode(classifiableIndividualSimulation);
         _populationSimulationNode = new SimulationNode(classfiablePopulationSimulation);
         _importedSimulationNode = new SimulationNode(classifiableImportSimulation);
         _usedObservedDataNode = A.Fake<ITreeNode>();
         A.CallTo(() => _treeNodeFactory.CreateFor(classifiableIndividualSimulation)).Returns(_individualSimulationNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(classfiablePopulationSimulation)).Returns(_populationSimulationNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(classifiableImportSimulation)).Returns(_importedSimulationNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(_usedObservedData)).Returns(_usedObservedDataNode);
         _project.AddBuildingBlock(_importedSimulaton);

         var simulationComparison = A.Fake<ISimulationComparison>().WithId("SimComp_Id");
         var classifiableComparison = new ClassifiableComparison {Subject = simulationComparison};
         _comparisonNode = new ComparisonNode(classifiableComparison);
         _project.AddSimulationComparison(simulationComparison);
         A.CallTo(() => _treeNodeFactory.CreateFor(classifiableComparison)).Returns(_comparisonNode);


         _usedCompoundBuildingBlock = new UsedBuildingBlock("toto", PKSimBuildingBlockType.Compound) {Id = "usedBB"};
         _simulation.AddUsedBuildingBlock(_usedCompoundBuildingBlock);
         _simulation.AddUsedObservedData(_usedObservedData);

         _project.AddClassifiable(classifiableComparison);

         _templageCompoundBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         _usedBuildingBlockNode = new UsedBuildingBlockInSimulationNode(_simulation, _usedCompoundBuildingBlock, _templageCompoundBuildingBlock);
         A.CallTo(() => _treeNodeFactory.CreateFor(_simulation, _usedCompoundBuildingBlock)).Returns(_usedBuildingBlockNode);
         A.CallTo(() => _buildingBlockIconRetriever.IconFor(_simulation)).Returns(ApplicationIcons.SimulationGreen);
         A.CallTo(() => _buildingBlockIconRetriever.IconFor(_populationSimulation)).Returns(ApplicationIcons.SimulationGreen);
         A.CallTo(() => _buildingBlockIconRetriever.IconFor(_usedCompoundBuildingBlock)).Returns(ApplicationIcons.CompoundRed);
         A.CallTo(() => _view.TreeView.NodeById(_simulation.Id)).Returns(_individualSimulationNode);
         A.CallTo(() => _view.TreeView.NodeById(_usedCompoundBuildingBlock.Id)).Returns(_usedBuildingBlockNode);
         A.CallTo(() => _view.TreeView.NodeById(RootNodeTypes.SimulationFolder.Id)).Returns(_simulationFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(RootNodeTypes.ComparisonFolder.Id)).Returns(_comparisonFolderNode);

         _observedDataInSimulationManager = A.Fake<IObservedDataInSimulationManager>();
         sut = new SimulationExplorerPresenter(_view, _treeNodeFactory, _contextMenuFactory, _multipleTreeNodeContextMenuFactory, _buildingBlockIconRetriever,
            _regionResolver, _buildingBlockTask, _buildingBlockInSimulationManager, _toolTipNodeCreator, _projectRetriever, _classificationPresenter, _parameterAnalysablesInExplorerPresenter, _observedDataInSimulationManager);

         A.CallTo(() => _projectRetriever.CurrentProject).Returns(_project);
      }
   }

   public class When_double_clicking_a_node_and_the_tree_node_is_a_used_building_block : concern_for_SimulationExplorerPresenter
   {
      private ITreeNode _treeNode;

      protected override void Context()
      {
         base.Context();
         _treeNode = new UsedBuildingBlockInSimulationNode(_simulation, _usedCompoundBuildingBlock, _templageCompoundBuildingBlock);
      }

      protected override void Because()
      {
         sut.NodeDoubleClicked(_treeNode);
      }

      [Observation]
      public void should_not_result_in_activation_of_the_context_menu()
      {
         A.CallTo(() => _contextMenuFactory.CreateFor(_treeNode, sut)).MustNotHaveHappened();
      }
   }

   public class When_the_simulation_explorer_has_a_simulation_added_which_is_already_located_in_the_project : concern_for_SimulationExplorerPresenter
   {
      private IClassifiable _classifiable;
      private Classification _classification;
      private ClassificationNode _classificationNode;
      private List<ITreeNode> _addedNodes;

      protected override void Context()
      {
         base.Context();
         _addedNodes = new List<ITreeNode>();
         _classifiable = _project.AllClassifiables.FindById(_simulation.Id);
         _classification = new Classification();
         _classifiable.Parent = _classification;
         _project.AddClassification(_classification);
         _classificationNode = new ClassificationNode(_classification);
         A.CallTo(() => _view.TreeView.NodeById(_classification.Id)).Returns(_classificationNode);

         A.CallTo(() => _view.AddNode(A<ITreeNode>._)).Invokes(x => _addedNodes.Add(x.GetArgument<ITreeNode>(0)));
         _project.AddObservedData(new DataRepository(_usedObservedData.Id));
      }

      protected override void Because()
      {
         sut.Handle(new BuildingBlockAddedEvent(_simulation, _project));
      }

      [Observation]
      public void should_use_the_existing_classification_instead_of_creating_a_new_one()
      {
         _addedNodes.ShouldContain(_classificationNode);
      }

      [Observation]
      public void should_add_the_observed_data_node()
      {
         _addedNodes.ShouldContain(_usedObservedDataNode);
      }
   }

   public class When_the_simulation_explorer_is_adding_a_simulation_whose_observed_data_do_not_exist_in_project_anymore : concern_for_SimulationExplorerPresenter
   {
      private List<ITreeNode> _addedNodes;

      protected override void Context()
      {
         base.Context();
         _addedNodes = new List<ITreeNode>();
         A.CallTo(() => _view.AddNode(A<ITreeNode>._)).Invokes(x => _addedNodes.Add(x.GetArgument<ITreeNode>(0)));
      }

      protected override void Because()
      {
         sut.Handle(new BuildingBlockAddedEvent(_simulation, _project));
      }

      [Observation]
      public void should_not_add_the_observed_data_node()
      {
         _addedNodes.ShouldNotContain(_usedObservedDataNode);
      }
   }

   public class When_the_simulation_explorer_presenter_is_being_notified_that_a_simulation_was_created : concern_for_SimulationExplorerPresenter
   {
      protected override void Because()
      {
         sut.Handle(new ProjectCreatedEvent(_project));
      }

      [Observation]
      public void should_add_the_root_folders_to_the_view()
      {
         A.CallTo(() => _view.AddNode(_simulationFolderNode)).MustHaveHappened();
         A.CallTo(() => _view.AddNode(_comparisonFolderNode)).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_node_for_each_simulation_under_the_simulation_root_node()
      {
         _simulationFolderNode.Children.Contains(_individualSimulationNode).ShouldBeTrue();
      }

      [Observation]
      public void should_add_one_node_for_each_comparison_under_the_comparison_root_node()
      {
         _comparisonFolderNode.Children.Contains(_comparisonNode).ShouldBeTrue();
      }

      [Observation]
      public void should_not_add_any_building_block_information_for_the_imported_simulation()
      {
         _importedSimulationNode.Children.Any().ShouldBeFalse();
      }

      [Observation]
      public void should_add_one_node_for_each_building_block_used_in_the_simulation()
      {
         _individualSimulationNode.Children.Contains(_usedBuildingBlockNode).ShouldBeTrue();
      }
   }

   public class When_notifiy_that_an_entity_was_renamed_that_is_not_a_building_block : concern_for_SimulationExplorerPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Handle(new ProjectCreatedEvent(_project));
         _usedBuildingBlockNode.Text = "oldText";
      }

      protected override void Because()
      {
         sut.Handle(new RenamedEvent(A.Fake<IEntity>()));
      }

      [Observation]
      public void should_not_update_the_name_of_the_building_block_used_in_the_simulation()
      {
         _usedBuildingBlockNode.Text.ShouldBeEqualTo("oldText");
      }
   }

   public class When_notifiy_that_a_simulation_was_renamed : concern_for_SimulationExplorerPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.Handle(new ProjectCreatedEvent(_project));
         _usedBuildingBlockNode.Text = "oldText";
         _usedCompoundBuildingBlock.Name = "newName";
      }

      protected override void Because()
      {
         sut.Handle(new RenamedEvent(_simulation));
      }

      [Observation]
      public void should_update_the_node_for_the_simulation()
      {
         _usedBuildingBlockNode.Text.ShouldBeEqualTo("newName");
      }
   }

   public class When_notifiy_that_a_building_block_used_in_a_simulation_was_renamed : concern_for_SimulationExplorerPresenter
   {
      private IPKSimBuildingBlock _buildingBlock;

      protected override void Context()
      {
         base.Context();
         sut.Handle(new ProjectCreatedEvent(_project));
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _buildingBlockInSimulationManager.SimulationsUsing(_buildingBlock)).Returns(new[] {_simulation});
         _usedBuildingBlockNode.Text = "oldText";
         _usedCompoundBuildingBlock.Name = "newName";
      }

      protected override void Because()
      {
         sut.Handle(new RenamedEvent(_buildingBlock));
      }

      [Observation]
      public void should_update_the_name_of_the_simulation_using_that_building_block()
      {
         _usedBuildingBlockNode.Text.ShouldBeEqualTo("newName");
      }
   }

   public class When_notified_that_an_observed_data_used_in_a_simulation_has_been_renamed : concern_for_SimulationExplorerPresenter
   {
      private DataRepository _dataRepository;
      private ITreeNode _node;

      protected override void Context()
      {
         base.Context();
         _dataRepository = new DataRepository("repositoryId");
         _simulation.AddUsedObservedData(_dataRepository);
         _project.AddObservedData(_dataRepository);
         _node = A.Fake<ITreeNode>();
         A.CallTo(() => _treeNodeFactory.CreateFor(A<UsedObservedData>.That.Matches(x => string.Equals(x.Id, _dataRepository.Id)))).Returns(_node);
         A.CallTo(() => _observedDataInSimulationManager.SimulationsUsing(_dataRepository)).Returns(new List<Simulation> {_simulation});
      }

      protected override void Because()
      {
         sut.Handle(new RenamedEvent(_dataRepository));
      }

      [Observation]
      public void the_node_created_by_the_tree_factory_is_added_to_the_explorer()
      {
         A.CallTo(() => _view.AddNode(_node)).MustHaveHappened();
      }

      [Observation]
      public void the_tree_factory_is_used_to_create_a_new_node()
      {
         A.CallTo(() => _treeNodeFactory.CreateFor(A<UsedObservedData>.That.Matches(x => string.Equals(x.Id, _dataRepository.Id)))).MustHaveHappened();
      }
   }

   public class When_notifiy_that_a_building_block_that_is_not_used_in_a_simulation_was_renamed : concern_for_SimulationExplorerPresenter
   {
      private IPKSimBuildingBlock _anotherBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _anotherBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _buildingBlockInSimulationManager.SimulationsUsing(_anotherBuildingBlock)).Returns(Enumerable.Empty<Simulation>());
         sut.Handle(new ProjectCreatedEvent(_project));
         _usedBuildingBlockNode.Text = "oldText";
         _anotherBuildingBlock.Name = "newName";
      }

      protected override void Because()
      {
         sut.Handle(new RenamedEvent(_anotherBuildingBlock));
      }

      [Observation]
      public void should_not_update_the_name_of_the_building_block_used_in_the_simulation()
      {
         _usedBuildingBlockNode.Text.ShouldBeEqualTo("oldText");
      }
   }

   public class When_simulation_explorer_is_told_that_a_node_was_activaed : concern_for_SimulationExplorerPresenter
   {
      protected override void Because()
      {
         sut.NodeDoubleClicked(_individualSimulationNode);
      }

      [Observation]
      public void should_leverage_the_building_block_task_to_edit_the_building_block()
      {
         A.CallTo(() => _buildingBlockTask.Edit(_simulation)).MustHaveHappened();
      }
   }

   public class When_the_simulation_presenter_is_asked_if_it_can_drop_a_node_on_another_one : concern_for_SimulationExplorerPresenter
   {
      [Observation]
      public void should_return_true_when_dropping_a_simulation_node_on_a_simulation_folder_node()
      {
         sut.CanDrop(_individualSimulationNode, _simulationFolderNode).ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_when_dropping_a_comparision_node_on_a_comparison_folder_node()
      {
         sut.CanDrop(_comparisonNode, _comparisonFolderNode).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_when_dropping_a_comparision_node_on_a_simulation_folder_node()
      {
         sut.CanDrop(_individualSimulationNode, _comparisonFolderNode).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_when_dropping_a_simulation_node_on_a_comparison_folder_node()
      {
         sut.CanDrop(_comparisonNode, _simulationFolderNode).ShouldBeFalse();
      }
   }

   public class When_grouping_the_root_folder_by_simulation_types : concern_for_SimulationExplorerPresenter
   {
      protected override void Because()
      {
         sut.AddToClassificationTree(_simulationFolderNode, PKSimConstants.Classifications.SimulationType);
      }

      [Observation]
      public void should_create_one_folder_for_individual_simulation_with_all_individual_simulations_under_it()
      {
         A.CallTo(() => _classificationPresenter.GroupClassificationsByCategory(_simulationFolderNode, PKSimConstants.Classifications.SimulationType,
            A<Func<ClassifiableSimulation, string>>._)).MustHaveHappened();
      }
   }

   public class When_the_simulation_explorer_presente_is_notified_of_the_swap_building_block_event : concern_for_SimulationExplorerPresenter
   {
      private IPKSimBuildingBlock _newBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _newBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         sut.Handle(new ProjectCreatedEvent(_project));
      }

      protected override void Because()
      {
         sut.Handle(new SwapBuildingBlockEvent(_templageCompoundBuildingBlock, _newBuildingBlock));
      }

      [Observation]
      public void should_udpate_the_reference_to_the_old_building_block_with_the_new_building_block()
      {
         _usedBuildingBlockNode.TemplateBuildingBlock.ShouldBeEqualTo(_newBuildingBlock);
      }
   }
}