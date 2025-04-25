using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Main;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Presenters.Classifications;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation
{
   public abstract class concern_for_ClassificationPresenter : ContextSpecification<ClassificationPresenter>
   {
      protected IClassificationTypeToRootNodeTypeMapper _rootNodeTypeMapper;
      private IProjectRetriever _projectRetriever;
      protected IExplorerPresenter _explorerPresenter;
      protected IApplicationController _applicationController;
      protected PKSimProject _project;
      protected RootNode _simulationFolderNode;
      protected RootNode _observedDataFolderNode;

      protected Cache<IClassification, ITreeNode<IClassification>> _classificationNodesCache;

      protected override void Context()
      {
         _rootNodeTypeMapper = A.Fake<IClassificationTypeToRootNodeTypeMapper>();
         _projectRetriever = A.Fake<IProjectRetriever>();
         _applicationController = A.Fake<IApplicationController>();
         _project = new PKSimProject();
         _classificationNodesCache = new Cache<IClassification, ITreeNode<IClassification>>(x => x.Tag, x => null);

         A.CallTo(() => _projectRetriever.CurrentProject).Returns(_project);
         sut = new ClassificationPresenter(_rootNodeTypeMapper, _applicationController, _projectRetriever);

         _explorerPresenter = A.Fake<IExplorerPresenter>();
         sut.InitializeWith(_explorerPresenter);

         _simulationFolderNode = createRootNode(RootNodeTypes.SimulationFolder, ClassificationType.Simulation);
         _observedDataFolderNode = createRootNode(RootNodeTypes.ObservedDataFolder, ClassificationType.ObservedData);

         A.CallTo(() => _explorerPresenter.AddNode(A<ITreeNode>._))
            .Invokes(x =>
            {
               var classificationNode = x.GetArgument<ITreeNode>(0) as ITreeNode<IClassification>;
               if (classificationNode != null)
                  addToCache(classificationNode);
            });

         A.CallTo(() => _explorerPresenter.RemoveNode(A<ITreeNode>._))
            .Invokes(x =>
            {
               var classificationNode = x.GetArgument<ITreeNode>(0) as ITreeNode<IClassification>;
               if (classificationNode != null)
                  removeFromCache(classificationNode);
            });
      }

      private void removeFromCache(ITreeNode<IClassification> classificationNode)
      {
         if (_classificationNodesCache.Contains(classificationNode.Tag))
            _classificationNodesCache.Remove(classificationNode.Tag);
      }

      private void addToCache(ITreeNode<IClassification> classificationNode)
      {
         if (!_classificationNodesCache.Contains(classificationNode.Tag))
            _classificationNodesCache.Add(classificationNode);
      }

      private RootNode createRootNode(RootNodeType rootNodeType, ClassificationType classificationType)
      {
         var node = new RootNode(rootNodeType);
         A.CallTo(() => _rootNodeTypeMapper.MapFrom(classificationType)).Returns(rootNodeType);
         A.CallTo(() => _explorerPresenter.NodeByType(rootNodeType)).Returns(node);
         return node;
      }
   }

   public class When_the_presenter_is_asked_to_remove_classification_node : concern_for_ClassificationPresenter
   {
      protected ITreeNode<IClassification> _parentNode;

      protected override void Context()
      {
         base.Context();
         var root = new Classification();
         var rootNode = new ClassificationNode(root);
         var parent = new Classification();
         _parentNode = A.Fake<ITreeNode<IClassification>>();
         A.CallTo(() => _parentNode.Tag).Returns(parent);

         _parentNode.Under(rootNode);
      }

      protected override void Because()
      {
         sut.RemoveClassification(_parentNode);
      }

      [Observation]
      public void the_node_should_be_cleared_from_the_view()
      {
         A.CallTo(() => _explorerPresenter.RemoveNode(A<ITreeNode>.Ignored)).MustHaveHappened();
      }

      [Observation]
      public void the_node_should_be_removed()
      {
         A.CallTo(() => _parentNode.Delete()).MustHaveHappened();
      }
   }

   public class When_the_classification_presenter_is_creating_a_classification_node_for_a_given_classification : concern_for_ClassificationPresenter
   {
      private IClassification _classification;
      private IClassification _parent;

      protected override void Context()
      {
         base.Context();
         _parent = new Classification {ClassificationType = ClassificationType.ObservedData}
            .WithId("parent").WithName("parent");

         _classification = new Classification {ClassificationType = ClassificationType.ObservedData}
            .WithId("child").WithName("child");

         _classification.Parent = _parent;
      }

      protected override void Because()
      {
         sut.AddClassificationsToTree(new[] {_parent, _classification});
      }

      [Observation]
      public void should_create_a_node_for_each_classification_in_the_hiearchy()
      {
         var node = _classificationNodesCache[_classification];
         var parentNode = node.ParentNode.DowncastTo<ITreeNode<IClassification>>();
         node.ShouldNotBeNull();
         node.Tag.ShouldBeEqualTo(_classification);
         node.RootNode.ShouldBeEqualTo(_observedDataFolderNode);
         parentNode.Tag.ShouldBeEqualTo(_parent);
      }
   }

   public class When_creating_a_classification_folder : concern_for_ClassificationPresenter
   {
      private INameClassificationPresenter _presenter;
      private ITreeNode<IClassification> _parentClassificationNode;
      private IClassification _parentClassification;

      protected override void Context()
      {
         base.Context();
         _presenter = A.Fake<INameClassificationPresenter>();
         A.CallTo(() => _applicationController.Start<INameClassificationPresenter>()).Returns(_presenter);
         //required to use this unspecific CallTo because of an issue with enum type
         A.CallTo(_presenter).WithReturnType<bool>().Returns(true);
         A.CallTo(() => _presenter.Name).Returns("CLASS");
         _parentClassification = new Classification {ClassificationType = ClassificationType.Simulation};
         _project.AddClassification(_parentClassification);
         _parentClassificationNode = new ClassificationNode(_parentClassification);
         A.CallTo(() => _explorerPresenter.NodeFor(A<IClassification>._)).Returns(null);
      }

      protected override void Because()
      {
         sut.CreateClassificationFolderUnder(_parentClassificationNode);
      }

      [Observation]
      public void should_ask_the_user_to_enter_a_new_name_for_the_classification_and_the_returned_node_should_represent_the_classificaiton_node()
      {
         var newClassification = _project.AllClassificationsByType(ClassificationType.Simulation).FindByName("CLASS");
         newClassification.ShouldNotBeNull();
         newClassification.Parent.ShouldBeEqualTo(_parentClassification);
      }
   }

   public class When_renaning_a_classifiaction : concern_for_ClassificationPresenter
   {
      private ITreeNode<IClassification> _classificationNode;
      private IClassification _classification;
      private IRenameClassificationPresenter _presenter;

      protected override void Context()
      {
         base.Context();
         _classificationNode = A.Fake<ITreeNode<IClassification>>();
         _classification = A.Fake<IClassification>().WithName("OLD_NAME");
         _presenter = A.Fake<IRenameClassificationPresenter>();
         A.CallTo(() => _classificationNode.Tag).Returns(_classification);
         A.CallTo(() => _applicationController.Start<IRenameClassificationPresenter>()).Returns(_presenter);
         A.CallTo(() => _presenter.Rename(_classification)).Returns(true);
         A.CallTo(() => _presenter.Name).Returns("NEW_NAME");
      }

      protected override void Because()
      {
         sut.RenameClassification(_classificationNode);
      }

      [Observation]
      public void should_ask_the_user_to_enter_a_new_name_for_the_classification()
      {
         A.CallTo(() => _presenter.Rename(_classification)).MustHaveHappened();
      }

      [Observation]
      public void should_have_renamed_the_classification()
      {
         _classification.Name.ShouldBeEqualTo("NEW_NAME");
      }
   }

   public class When_grouping_all_classifications_defined_in_a_project_by_a_given_category : concern_for_ClassificationPresenter
   {
      private ClassifiableSimulation _simulation1;
      private ClassifiableSimulation _simulation2;
      private ClassifiableSimulation _simulation3WithParent;
      private ClassifiableObservedData _observedData;

      protected override void Context()
      {
         base.Context();
         _simulation1 = new ClassifiableSimulation {Subject = new IndividualSimulation{Id="1"}};
         _simulation2 = new ClassifiableSimulation { Subject = new IndividualSimulation { Id = "2" } };
         var parentClassification = new Classification {ClassificationType = ClassificationType.Simulation};
         _simulation3WithParent = new ClassifiableSimulation { Subject = new IndividualSimulation { Id = "3" }, Parent = parentClassification };
         _observedData = new ClassifiableObservedData{Subject = new DataRepository()};
         _project.AddClassifiable(_simulation1);
         _project.AddClassifiable(_simulation2);
         _project.AddClassifiable(_simulation3WithParent);
         _project.AddClassifiable(_observedData);
         var simulationNode1 = new SimulationNode(_simulation1);
         A.CallTo(() => _explorerPresenter.NodeFor(_simulation1)).Returns(simulationNode1);
         var simulationNode2 = new SimulationNode(_simulation2);
         A.CallTo(() => _explorerPresenter.NodeFor(_simulation2)).Returns(simulationNode2);
         A.CallTo(() => _explorerPresenter.NodeFor(_simulation3WithParent)).Returns(new SimulationNode(_simulation3WithParent));
         A.CallTo(() => _explorerPresenter.NodeFor(_observedData)).Returns(new ObservedDataNode(_observedData));
         var parentClassificationNode = new ClassificationNode(parentClassification);
         A.CallTo(() => _explorerPresenter.NodeFor(parentClassification)).Returns(parentClassificationNode);
         _simulationFolderNode.AddChild(simulationNode1);
         _simulationFolderNode.AddChild(simulationNode2);
         _simulationFolderNode.AddChild(parentClassificationNode);
      
      }

      protected override void Because()
      {
         sut.GroupClassificationsByCategory<ClassifiableSimulation>(_simulationFolderNode, "Individual", x => "Human");
      }

      [Observation]
      public void should_have_created_a_classifiaction_named_after_the_distinct_category_values()
      {
         humanClassification.ShouldNotBeNull();
      }

      private IClassification humanClassification
      {
         get { return _project.AllClassificationsByType(ClassificationType.Simulation).FindByName("Human"); }
      }

      [Observation]
      public void should_have_moved_the_node_for_the_simulations_that_were_not_classified_under_the_newly_created_classification()
      {
         _simulation1.Parent.ShouldBeEqualTo(humanClassification);
         _simulation2.Parent.ShouldBeEqualTo(humanClassification);
      }

      [Observation]
      public void should_not_have_moved_the_node_for_the_simulations_that_were_already_classified()
      {
         _simulation3WithParent.Parent.ShouldNotBeEqualTo(humanClassification);
      }

      [Observation]
      public void should_not_have_moved_the_node_for_the_classifiable_that_do_not_have_the_right_type()
      {
         _observedData.Parent.ShouldNotBeEqualTo(humanClassification);
      }
   }

   public class When_removing_a_classification_folder_containing_classifiable_items : concern_for_ClassificationPresenter
   {
      private Classification _parentClassification;
      private Classification _childClassification;
      private ClassifiableSimulation _simulation;
      private ITreeNode<IClassification> _childClassificationNode;
      private ITreeNode<IClassification> _parentClassificationNode;
      private SimulationNode _simulationNode;

      protected override void Context()
      {
         base.Context();

         _parentClassification = new Classification {ClassificationType = ClassificationType.Simulation, Name = "parent"};
         _childClassification = new Classification {ClassificationType = ClassificationType.Simulation, Name = "child", Parent = _parentClassification};

         sut.AddClassificationsToTree(new[] {_childClassification, _parentClassification});

         //do that after so that real node and node fakes are created when adding the classificaiton to the tree
         _project.AddClassification(_childClassification);
         _project.AddClassification(_parentClassification);

         _simulation = new ClassifiableSimulation {Parent = _childClassification, Subject = new IndividualSimulation {Name = "Sim"}};

         _childClassificationNode = _classificationNodesCache[_childClassification];
         _simulationNode = new SimulationNode(_simulation);
         _childClassificationNode.AddChild(_simulationNode);

         _parentClassificationNode = _childClassificationNode.ParentNode.DowncastTo<ITreeNode<IClassification>>();
      }

      protected override void Because()
      {
         sut.RemoveClassification(_childClassificationNode);
      }

      [Observation]
      public void should_move_the_classifiable_node_to_the_parent_classification()
      {
         _parentClassificationNode.Children.ShouldContain(_simulationNode);
      }

      [Observation]
      public void should_attach_the_simulation_to_the_parent_classificaiton()
      {
         _simulation.Parent.ShouldBeEqualTo(_parentClassification);
      }

      [Observation]
      public void should_remove_the_classification_node_from_the_underlying_cache()
      {
         _classificationNodesCache[_childClassification].ShouldBeNull();
      }

      [Observation]
      public void should_remove_the_classification_from_the_project()
      {
         _project.AllClassifications.Contains(_childClassification).ShouldBeFalse();
      }

      [Observation]
      public void should_remove_the_classification_node_from_the_view()
      {
         _childClassificationNode.ParentNode.ShouldBeNull();
         _parentClassificationNode.Children.Contains(_childClassificationNode).ShouldBeFalse();
      }
   }

   public class When_removing_a_classification_would_result_in_a_duplicate_node : concern_for_ClassificationPresenter
   {
      private IClassification _humanClassification;
      private IClassification _humanOralClassification;
      private IClassification _oralClassification;
      private ClassifiableSimulation _simulation1;
      private ClassifiableSimulation _simulation2;
      private ITreeNode<IClassification> _humanOralClassificationNode;
      private ITreeNode<IClassification> _humanClassificationNode;
      private SimulationNode _simulation1Node;
      private ITreeNode<IClassification> _oralClassificationNode;
      private SimulationNode _simulation2Node;

      protected override void Context()
      {
         base.Context();
         //create the following tree
         //human => oral=>S1
         //oral=>S2
         _humanClassification = new Classification {ClassificationType = ClassificationType.Simulation, Name = "human"};
         _humanOralClassification = new Classification {ClassificationType = ClassificationType.Simulation, Name = "oral", Parent = _humanClassification};
         _oralClassification = new Classification {ClassificationType = ClassificationType.Simulation, Name = "oral"};

         sut.AddClassificationsToTree(new[] {_humanOralClassification, _humanClassification, _oralClassification});

         _project.AddClassification(_humanOralClassification);
         _project.AddClassification(_humanClassification);
         _project.AddClassification(_oralClassification);

         _simulation1 = new ClassifiableSimulation {Parent = _humanOralClassification, Subject = new IndividualSimulation {Name = "S1"}};
         _simulation2 = new ClassifiableSimulation {Parent = _oralClassification, Subject = new IndividualSimulation {Name = "S2"}};

         _humanOralClassificationNode = _classificationNodesCache[_humanOralClassification];
         _oralClassificationNode = _classificationNodesCache[_oralClassification];
         _simulation1Node = new SimulationNode(_simulation1).Under(_humanOralClassificationNode);
         _simulation2Node = new SimulationNode(_simulation2).Under(_oralClassificationNode);

         _humanClassificationNode = _humanOralClassificationNode.ParentNode.DowncastTo<ITreeNode<IClassification>>();

         A.CallTo(() => _explorerPresenter.NodeFor(_humanOralClassification)).Returns(_humanOralClassificationNode);
         A.CallTo(() => _explorerPresenter.NodeFor(_oralClassification)).Returns(_oralClassificationNode);
         A.CallTo(() => _explorerPresenter.NodeFor(_humanClassification)).Returns(_humanClassificationNode);
      }

      protected override void Because()
      {
         sut.RemoveClassification(_humanClassificationNode);
      }

      [Observation]
      public void should_move_the_classifiable_from_the_duplicate_into_the_equivalent_classification()
      {
         _simulation1.Parent.ShouldBeEqualTo(_oralClassification);
         _simulation2.Parent.ShouldBeEqualTo(_oralClassification);
      }

      [Observation]
      public void should_delete_the_duplicate_classification_from_the_project()
      {
         _project.AllClassifications.Contains(_humanClassification).ShouldBeFalse();
         _project.AllClassifications.Contains(_humanOralClassification).ShouldBeFalse();
      }

      [Observation]
      public void should_delete_the_duplicate_classification_node_from_the_view()
      {
         _classificationNodesCache[_humanClassification].ShouldBeNull();
         _classificationNodesCache[_humanOralClassification].ShouldBeNull();
      }
   }

   public class When_moving_a_classifiable_node_defined_under_a_given_classification_node_under_another_classification_node : concern_for_ClassificationPresenter
   {
      private ClassifiableSimulation _simulation;
      private IClassification _originalClassification;
      private IClassification _targetClassification;
      private SimulationNode _simulationNode;
      private ITreeNode<IClassification> _originalClassificationNode;
      private ITreeNode<IClassification> _targetClassificationNode;

      protected override void Context()
      {
         base.Context();
         _originalClassification = new Classification {ClassificationType = ClassificationType.Simulation, Name = "Original"};
         _targetClassification = new Classification {ClassificationType = ClassificationType.Simulation, Name = "Target"};
         _simulation = new ClassifiableSimulation {Parent = _originalClassification, Subject = new IndividualSimulation {Name = "S1"}};
         sut.AddClassificationsToTree(new[] {_originalClassification, _targetClassification});
         _project.AddClassification(_originalClassification);
         _project.AddClassification(_targetClassification);
         _originalClassificationNode = _classificationNodesCache[_originalClassification];
         _targetClassificationNode = _classificationNodesCache[_targetClassification];
         _simulationNode = new SimulationNode(_simulation).Under(_originalClassificationNode);
      }

      protected override void Because()
      {
         sut.MoveNode(_simulationNode, _targetClassificationNode);
      }

      [Observation]
      public void should_move_the_underlying_classifiable_under_the_new_classification()
      {
         _simulation.Parent.ShouldBeEqualTo(_targetClassification);
      }

      [Observation]
      public void should_move_the_simulation_node_under_the_new_classification_node()
      {
         _simulationNode.ParentNode.ShouldBeEqualTo(_targetClassificationNode);
         _originalClassificationNode.Children.Contains(_simulationNode).ShouldBeFalse();
      }
   }

   public class When_moving_an_observed_data_node_under_root_node : concern_for_ClassificationPresenter
   {
      private ITreeNode<IClassifiable> _observedDataNode;
      private ClassifiableObservedData _classifiableObservedData;
      private Classification _originalClassification;

      protected override void Context()
      {
         base.Context();
         _originalClassification = new Classification { ClassificationType = ClassificationType.ObservedData, Name = "Original" };
         _classifiableObservedData = new ClassifiableObservedData { Subject = new DataRepository(), Parent = _originalClassification };
         _observedDataNode = new ObservedDataNode(_classifiableObservedData);
         _project.AddClassification(_originalClassification);
         sut.AddClassificationsToTree(new[] { _originalClassification });
      }

      protected override void Because()
      {
         sut.MoveNode(_observedDataNode, _observedDataFolderNode);
      }

      [Observation]
      public void should_have_set_the_parent_classification_of_the_observed_data_classifiable_to_null()
      {
         _classifiableObservedData.Parent.ShouldBeNull();
      }
   }

   public class When_testing_if_a_classification_node_can_be_moved_under_another_classification_node : concern_for_ClassificationPresenter
   {
      private ClassificationNode _observedDataClassificationNode;
      private ClassificationNode _simulationClassificationNode;
      private ClassificationNode _groupClassificationNode;
      private ClassificationNode _subClassificationNode;

      protected override void Context()
      {
         base.Context();
         _observedDataClassificationNode = new ClassificationNode(new Classification {ClassificationType = ClassificationType.ObservedData, Name = "OBS"});
         _simulationClassificationNode = new ClassificationNode(new Classification {ClassificationType = ClassificationType.Simulation, Name = "SIM"});
         _groupClassificationNode = new ClassificationNode(new Classification {ClassificationType = ClassificationType.Simulation, Name = "PARENT"});
         _subClassificationNode = new ClassificationNode(new Classification {ClassificationType = ClassificationType.Simulation, Name = "SIM"});
         _groupClassificationNode.AddChild(_subClassificationNode);
      }

      [Observation]
      public void should_return_false_if_the_two_nodes_are_the_same()
      {
         sut.CanMove(_observedDataClassificationNode, _observedDataClassificationNode).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_two_underlying_classification_do_not_have_the_same_type()
      {
         sut.CanMove(_observedDataClassificationNode, _simulationClassificationNode).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_target_classification_already_have_a_sub_classifiation_with_the_same_name()
      {
         sut.CanMove(_simulationClassificationNode, _groupClassificationNode).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_classification_to_move_is_an_ancestor_of_the_target_classification()
      {
         sut.CanMove(_groupClassificationNode, _subClassificationNode).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_otherwise()
      {
         sut.CanMove(_subClassificationNode, _simulationClassificationNode).ShouldBeTrue();
      }
   }

   public class When_removing_all_empty_classifications_from_the_classification_presenter : concern_for_ClassificationPresenter
   {
      private IClassification _parentClassification;
      private IClassification _childEmptyClassification;
      private IClassification _childClassification;
      private ITreeNode<IClassification> _childEmptyClassificationNode;
      private ITreeNode<IClassification> _childClassificationNode;
      private ITreeNode<IClassification> _parentClassificationNode;
      private SimulationNode _simulationNode;
      private ClassifiableSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _parentClassification = new Classification { ClassificationType = ClassificationType.Simulation, Name = "parent" };
         _childEmptyClassification = new Classification { ClassificationType = ClassificationType.Simulation, Name = "childEmpty", Parent = _parentClassification };
         _childClassification = new Classification { ClassificationType = ClassificationType.Simulation, Name = "child", Parent = _parentClassification };
         _simulation=new ClassifiableSimulation{Subject = new IndividualSimulation()};
         sut.AddClassificationsToTree(new[] { _childEmptyClassification, _parentClassification, _childClassification });

         //do that after so that real node and node fakes are created when adding the classificaiton to the tree
         _project.AddClassification(_childEmptyClassification);
         _project.AddClassification(_parentClassification);
         _project.AddClassification(_childClassification);
         _childEmptyClassificationNode = _classificationNodesCache[_childEmptyClassification];
         _childClassificationNode = _classificationNodesCache[_childClassification];
         _parentClassificationNode = _childClassificationNode.ParentNode.DowncastTo<ITreeNode<IClassification>>();
         _simulationNode = new SimulationNode(_simulation);
         _childClassificationNode.AddChild(_simulationNode);
         _parentClassificationNode = _childClassificationNode.ParentNode.DowncastTo<ITreeNode<IClassification>>();
         A.CallTo(() => _explorerPresenter.NodeFor(_childEmptyClassification)).Returns(_childEmptyClassificationNode);
         A.CallTo(() => _explorerPresenter.NodeFor(_childClassification)).Returns(_childClassificationNode);
         A.CallTo(() => _explorerPresenter.NodeFor(_parentClassification)).Returns(_parentClassificationNode);
      }

      protected override void Because()
      {
         sut.RemoveEmptyClassifcations();
      }

      [Observation]
      public void should_remove_all_classifications_that_do_not_contain_any_classifiable()
      {
         _project.AllClassifications.Contains(_childEmptyClassification).ShouldBeFalse();
         _project.AllClassifications.Contains(_childClassification).ShouldBeTrue();
         _project.AllClassifications.Contains(_parentClassification).ShouldBeTrue();
      }
   }
}