using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Regions;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Main;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Presenters.Classifications;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Presenters.ObservedData;
using OSPSuite.Presentation.Regions;
using OSPSuite.Presentation.Services;
using OSPSuite.Presentation.Views;
using BuildingBlockRemovedEvent = PKSim.Core.Events.BuildingBlockRemovedEvent;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_BuildingBlockExplorerPresenter : ContextSpecification<IBuildingBlockExplorerPresenter>
   {
      protected IBuildingBlockExplorerView _view;
      protected ITreeNodeFactory _treeNodeFactory;
      protected IRegion _region;
      protected ITreeNodeContextMenuFactory _contextMenuFactory;
      protected PKSimProject _project;
      protected ITreeNode<RootNodeType> _compoundFolderNode;
      protected ITreeNode<RootNodeType> _individualFolderNode;
      protected ITreeNode<RootNodeType> _formulationFolderNode;
      protected ITreeNode<Individual> _individualNode;
      protected ITreeNode<Compound> _compoundNode;
      protected SimulationNode _simulationNode;
      protected IBuildingBlockIconRetriever _buildingBlockIconRetriever;
      protected ITreeNode<RootNodeType> _protocolFolderNode;
      protected Individual _individual;
      private IRegionResolver _regionResolver;
      protected ITreeNode<RootNodeType> _populationFolderNode;
      protected RootNode _observationRootNode;
      private IBuildingBlockTask _buildingBlockTask;
      private ITreeNode<RootNodeType> _eventRootNode;
      private IToolTipPartCreator _toolTipCreator;
      private IProjectRetriever _projectRetriever;
      protected IClassificationPresenter _classificationPresenter;
      private IMultipleTreeNodeContextMenuFactory _multipleTreeNodeContextMenuFactory;
      private IObservedDataInExplorerPresenter _observedDataInExplorerPresenter;

      protected override void Context()
      {
         _view = A.Fake<IBuildingBlockExplorerView>();
         A.CallTo(() => _view.TreeView).Returns(A.Fake<IUxTreeView>());
         _regionResolver = A.Fake<IRegionResolver>();
         _region = A.Fake<IRegion>();
         A.CallTo(() => _regionResolver.RegionWithName(RegionNames.BuildingBlockExplorer)).Returns(_region);
         _project = new PKSimProject();
         var compound = A.Fake<Compound>().WithName("compound");
         _individual = A.Fake<Individual>().WithName("individual");
         A.CallTo(() => _individual.BuildingBlockType).Returns(PKSimBuildingBlockType.Individual);
         A.CallTo(() => _individual.Species).Returns(new Species());
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(compound);
         _observedDataInExplorerPresenter = A.Fake<IObservedDataInExplorerPresenter>();
         _buildingBlockIconRetriever = A.Fake<IBuildingBlockIconRetriever>();
         _treeNodeFactory = A.Fake<ITreeNodeFactory>();
         _contextMenuFactory = A.Fake<ITreeNodeContextMenuFactory>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _toolTipCreator = A.Fake<IToolTipPartCreator>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _classificationPresenter = A.Fake<IClassificationPresenter>();
         _multipleTreeNodeContextMenuFactory = A.Fake<IMultipleTreeNodeContextMenuFactory>();
         sut = new BuildingBlockExplorerPresenter(_view, _treeNodeFactory, _contextMenuFactory, _multipleTreeNodeContextMenuFactory, _buildingBlockIconRetriever, _regionResolver,
            _buildingBlockTask, _toolTipCreator, _projectRetriever, _classificationPresenter, _observedDataInExplorerPresenter);

         _compoundFolderNode = new RootNode(PKSimRootNodeTypes.CompoundFolder);
         _individualFolderNode = new RootNode(PKSimRootNodeTypes.IndividualFolder);
         _formulationFolderNode = new RootNode(PKSimRootNodeTypes.FormulationFolder);
         _protocolFolderNode = new RootNode(PKSimRootNodeTypes.ProtocolFolder);
         _populationFolderNode = new RootNode(PKSimRootNodeTypes.PopulationFolder);
         _eventRootNode = new RootNode(PKSimRootNodeTypes.EventFolder);
         _simulationNode = new SimulationNode(new ClassifiableSimulation {Subject = new IndividualSimulation {Id = "1"}});
         _compoundNode = new ObjectWithIdAndNameNode<Compound>(compound);
         _individualNode = new ObjectWithIdAndNameNode<Individual>(_individual);
         _observationRootNode = new RootNode(RootNodeTypes.ObservedDataFolder);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.CompoundFolder)).Returns(_compoundFolderNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualFolder)).Returns(_individualFolderNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.ProtocolFolder)).Returns(_protocolFolderNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.FormulationFolder)).Returns(_formulationFolderNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.PopulationFolder)).Returns(_populationFolderNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(RootNodeTypes.ObservedDataFolder)).Returns(_observationRootNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.EventFolder)).Returns(_eventRootNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(_individual)).Returns(_individualNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(compound)).Returns(_compoundNode);

         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.CompoundFolder.Id)).Returns(_compoundFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.IndividualFolder.Id)).Returns(_individualFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.ProtocolFolder.Id)).Returns(_protocolFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.FormulationFolder.Id)).Returns(_formulationFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.PopulationFolder.Id)).Returns(_populationFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(RootNodeTypes.ObservedDataFolder.Id)).Returns(_observationRootNode);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.EventFolder.Id)).Returns(_eventRootNode);

         A.CallTo(() => _view.AddNode(A<ITreeNode>._)).ReturnsLazily(s => s.Arguments[0].DowncastTo<ITreeNode>());
      }
   }

  

   public class When_starting_the_tree_view_presenter : concern_for_BuildingBlockExplorerPresenter
   {
      protected override void Because()
      {
         sut.Initialize();
      }

      [Observation]
      public void should_retrieve_the_project_explorer_region_and_add_its_view_into_it()
      {
         A.CallTo(() => _region.Add(_view)).MustHaveHappened();
      }

      [Observation]
      public void should_initialize_the_classification_presenter()
      {
         A.CallTo(() => _classificationPresenter.InitializeWith(sut)).MustHaveHappened();
      }
   }

   public class When_the_project_presenter_is_being_notified_that_a_building_block_was_added : concern_for_BuildingBlockExplorerPresenter
   {
      protected override void Because()
      {
         sut.Handle(new BuildingBlockAddedEvent(_individual, _project));
      }

      [Observation]
      public void should_refresh_the_project_tree()
      {
         A.CallTo(() => _view.AddNode(A<ITreeNode>.Ignored)).MustHaveHappened();
      }
   }

   public class When_the_project_presenter_is_being_notified_that_the_project_was_closed : concern_for_BuildingBlockExplorerPresenter
   {
      protected override void Because()
      {
         sut.Handle(new ProjectClosedEvent());
      }

      [Observation]
      public void should_clear_the_project_tree()
      {
         A.CallTo(() => _view.TreeView.DestroyAllNodes()).MustHaveHappened();
      }
   }

   public class When_the_project_presenter_is_being_notified_that_the_project_was_created : concern_for_BuildingBlockExplorerPresenter
   {
      protected override void Because()
      {
         sut.Handle(new ProjectCreatedEvent(_project));
      }

      [Observation]
      public void should_clear_the_tree_view()
      {
         A.CallTo(() => _view.TreeView.DestroyAllNodes()).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_root_node_for_the_observed_data_folder()
      {
         A.CallTo(() => _view.AddNode(_observationRootNode)).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_root_node_for_the_compound_data_folder()
      {
         A.CallTo(() => _view.AddNode(_compoundFolderNode)).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_root_node_for_the_individual_data_folder()
      {
         A.CallTo(() => _view.AddNode(_individualFolderNode)).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_root_node_for_the_protocol_folder()
      {
         A.CallTo(() => _view.AddNode(_protocolFolderNode)).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_root_node_for_the_formulation_folder()
      {
         A.CallTo(() => _view.AddNode(_formulationFolderNode)).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_root_node_for_the_population_folder()
      {
         A.CallTo(() => _view.AddNode(_populationFolderNode)).MustHaveHappened();
      }
   }

   public class When_the_building_block_explorer_presenter_is_asked_to_remove_classification_node : concern_for_BuildingBlockExplorerPresenter
   {
      private ITreeNode<IClassification> _parentNode;

      protected override void Context()
      {
         base.Context();
         _parentNode = A.Fake<ITreeNode<IClassification>>();
      }

      protected override void Because()
      {
         sut.RemoveClassification(_parentNode);
      }

      [Observation]
      public void should_delegate_to_the_classification_presenter()
      {
         A.CallTo(() => _classificationPresenter.RemoveClassification(_parentNode)).MustHaveHappened();
      }
   }

   public class When_the_project_presenter_is_being_notified_that_a_building_block_was_deleted : concern_for_BuildingBlockExplorerPresenter
   {
      protected override void Context()
      {
         base.Context();
         _individual.Id = "tralal";
         A.CallTo(() => _view.TreeView.NodeById(_individual.Id)).Returns(_individualNode);
      }

      protected override void Because()
      {
         sut.Handle(new BuildingBlockRemovedEvent(_individual, _project));
      }

      [Observation]
      public void should_remove_the_node_corresponding_to_the_removed_building_block()
      {
         A.CallTo(() => _view.TreeView.DestroyNode(_individualNode)).MustHaveHappened();
      }
   }

   public class When_the_project_presenter_is_being_notified_that_a_simulation_is_starting_ : concern_for_BuildingBlockExplorerPresenter
   {
      protected override void Because()
      {
         sut.Handle(new SimulationRunStartedEvent());
      }

      [Observation]
      public void should_disable_the_view()
      {
         _view.Enabled.ShouldBeFalse();
      }
   }

   public class When_the_project_presenter_is_being_notified_that_a_simulation_has_terminated : concern_for_BuildingBlockExplorerPresenter
   {
      protected override void Because()
      {
         sut.Handle(new SimulationRunFinishedEvent(null, new TimeSpan()));
      }

      [Observation]
      public void should_enable_the_view()
      {
         _view.Enabled.ShouldBeTrue();
      }
   }


   public class When_asked_if_multiple_selection_is_allowed_for_a_given_set_of_nodes : concern_for_BuildingBlockExplorerPresenter
   {
      [Observation]
      public void should_return_true_if_all_nodes_have_the_same_type()
      {
         sut.AllowMultiSelectFor(new[] {A.Fake<ClassificationNode>(), A.Fake<ClassificationNode>()}).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         sut.AllowMultiSelectFor(new ITreeNode[] {A.Fake<ClassificationNode>(), A.Fake<ObservedDataNode>()}).ShouldBeFalse();
      }
   }
}