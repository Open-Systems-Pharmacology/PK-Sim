using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationParameterGroupsPresenter : ContextSpecification<IPopulationParameterGroupsPresenter>
   {
      protected IParameterGroupTask _parameterGroupTask;
      protected IPopulationGroupNodeCreator _groupNodeCreator;
      protected IPopulationParameterGroupsView _view;
      protected List<IParameter> _allParameters;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IPopulationHierarchyNodeCreator _hierarchyNodeCreator;

      protected override void Context()
      {
         _view = A.Fake<IPopulationParameterGroupsView>();
         _parameterGroupTask = A.Fake<IParameterGroupTask>();
         _groupNodeCreator = A.Fake<IPopulationGroupNodeCreator>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _hierarchyNodeCreator = A.Fake<IPopulationHierarchyNodeCreator>();
         _allParameters = new List<IParameter>();
         sut = new PopulationParameterGroupsPresenter(_view, _parameterGroupTask, _groupNodeCreator, _hierarchyNodeCreator);
      }
   }

   public class When_intitializing_the_parameter_group_presenter_with_a_set_of_parameters_that_should_be_displayed_using_the_group_structure : concern_for_PopulationParameterGroupsPresenter
   {
      private IEnumerable<ITreeNode> _nodes;
      private ITreeNode _treeNode1;
      private ITreeNode _treeNode2;

      protected override void Context()
      {
         base.Context();
         var group1 = new Group {Name = "group1"};
         var group2 = new Group {Name = "group2"};
         _treeNode1 = new GroupNode(group1);
         _treeNode2 = new GroupNode(group2);


         A.CallTo(() => _view.AddNodes(A<IEnumerable<ITreeNode>>._))
            .Invokes(x => _nodes = x.GetArgument<IEnumerable<ITreeNode>>(0));

         A.CallTo(() => _parameterGroupTask.TopGroupsUsedBy(A<IEnumerable<IParameter>>._)).Returns(new[] {group1, group2});
         A.CallTo(() => _groupNodeCreator.CreateGroupNodeFor(group1, A<IReadOnlyCollection<IParameter>>._)).Returns(_treeNode1);
         A.CallTo(() => _groupNodeCreator.CreateGroupNodeFor(group2, A<IReadOnlyCollection<IParameter>>._)).Returns(_treeNode2);
      }

      protected override void Because()
      {
         sut.AddParameters(_allParameters, displayParameterUsingGroupStructure: true);
      }

      [Observation]
      public void should_retrieve_all_the_group_to_display_and_initialize_the_view()
      {
         _nodes.ShouldOnlyContain(_treeNode1, _treeNode2);
      }
   }

   public class When_intitializing_the_parameter_group_presenter_with_a_set_of_parameters_that_should_be_displayed_using_the_hierarchical_structure : concern_for_PopulationParameterGroupsPresenter
   {
      private IEnumerable<ITreeNode> _nodes;
      private IParameter p1;
      private ITreeNode _treeNode2;
      private ITreeNode _treeNode1;

      protected override void Context()
      {
         base.Context();
         _treeNode1 = A.Fake<ITreeNode>();
         _treeNode2 = A.Fake<ITreeNode>();
         p1 = A.Fake<IParameter>().WithGroup("Group_that_does_not_exist");
         _allParameters.Add(p1);

         A.CallTo(() => _view.AddNodes(A<IEnumerable<ITreeNode>>._))
            .Invokes(x => _nodes = x.GetArgument<IEnumerable<ITreeNode>>(0));

         A.CallTo(() => _hierarchyNodeCreator.CreateHierarchyNodeFor(A<IReadOnlyList<IParameter>>._)).Returns(new[] {_treeNode1, _treeNode2});
      }

      protected override void Because()
      {
         sut.AddParameters(_allParameters, displayParameterUsingGroupStructure: false);
      }

      [Observation]
      public void should_retrieve_all_the_group_to_display_and_initialize_the_view()
      {
         _nodes.ShouldOnlyContain(_treeNode1, _treeNode2);
      }
   }

   public class When_a_node_that_does_not_represent_a_parameter_is_being_selected : concern_for_PopulationParameterGroupsPresenter
   {
      private bool _nodeSelectedRaised;
      private bool _parameteNodeSelectedRaised;

      protected override void Context()
      {
         base.Context();
         sut.GroupNodeSelected += (o, e) => { _nodeSelectedRaised = true; };
         sut.ParameterNodeSelected += (o, e) => { _parameteNodeSelectedRaised = true; };
      }

      protected override void Because()
      {
         sut.NodeSelected(A.Fake<ITreeNode>());
      }

      [Observation]
      public void should_notify_that_a_node_was_selected()
      {
         _nodeSelectedRaised.ShouldBeTrue();
      }

      [Observation]
      public void should_not_notify_that_a_parameter_node_was_selected()
      {
         _parameteNodeSelectedRaised.ShouldBeFalse();
      }

      [Observation]
      public void should_return_no_parameter()
      {
         sut.SelectedParameter.ShouldBeNull();
      }
   }

   public class When_a_node_that_represents_a_parameter_is_being_selected : concern_for_PopulationParameterGroupsPresenter
   {
      private bool _nodeSelectedRaised;
      private bool _parameteNodeSelectedRaised;
      private ITreeNode<IParameter> _parameterNode;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameterNode = A.Fake<ITreeNode<IParameter>>();
         sut.GroupNodeSelected += (o, e) => { _nodeSelectedRaised = true; };
         sut.ParameterNodeSelected += (o, e) => { _parameteNodeSelectedRaised = true; };
         _parameter = new PKSimParameter();
         A.CallTo(() => _parameterNode.Tag).Returns(_parameter);
         A.CallTo(() => _view.SelectedNode).Returns(_parameterNode);
      }

      protected override void Because()
      {
         sut.NodeSelected(_parameterNode);
      }

      [Observation]
      public void should_not_notify_that_a_node_was_selected()
      {
         _nodeSelectedRaised.ShouldBeFalse();
      }

      [Observation]
      public void should_notify_that_a_parameter_node_was_selected()
      {
         _parameteNodeSelectedRaised.ShouldBeTrue();
      }

      [Observation]
      public void should_return_the_selected_parameter()
      {
         sut.SelectedParameter.ShouldBeEqualTo(_parameter);
      }
   }

   public class When_a_parameter_node_is_being_double_clicked : concern_for_PopulationParameterGroupsPresenter
   {
      private bool _parameterNodeDoubleClicked;
      private PKSimParameter _parameter;
      private ITreeNode<IParameter> _parameterNode;

      protected override void Context()
      {
         base.Context();
         _parameterNode = A.Fake<ITreeNode<IParameter>>();
         sut.ParameterNodeDoubleClicked += (o, e) => { _parameterNodeDoubleClicked = true; };
         _parameter = new PKSimParameter();
         A.CallTo(() => _parameterNode.Tag).Returns(_parameter);
      }

      protected override void Because()
      {
         sut.NodeDoubleClicked(_parameterNode);
      }

      [Observation]
      public void should_notify_that_a_parameter_node_was_double_clicked()
      {
         _parameterNodeDoubleClicked.ShouldBeTrue();
      }
   }

   public class When_a_covariate_node_is_being_double_clicked : concern_for_PopulationParameterGroupsPresenter
   {
      private bool _covariateNodeDoubleClicked;
      private CovariateNode _covariateNode;

      protected override void Context()
      {
         base.Context();
         _covariateNode = new CovariateNode("Gender");
         sut.CovariateNodeDoubleClicked += (o, e) => { _covariateNodeDoubleClicked = true; };
      }

      protected override void Because()
      {
         sut.NodeDoubleClicked(_covariateNode);
      }

      [Observation]
      public void should_notify_that_a_covariate_node_was_double_clicked()
      {
         _covariateNodeDoubleClicked.ShouldBeTrue();
      }
   }

   public class When_a_node_that_is_not_a_parameter_node_is_being_double_clicked : concern_for_PopulationParameterGroupsPresenter
   {
      private bool _parameterNodeDoubleClicked;
      private bool _covariateNodeDoubleClicked;

      protected override void Context()
      {
         base.Context();
         sut.ParameterNodeDoubleClicked += (o, e) => { _parameterNodeDoubleClicked = true; };
         sut.CovariateNodeDoubleClicked += (o, e) => { _covariateNodeDoubleClicked = true; };
      }

      protected override void Because()
      {
         sut.NodeDoubleClicked(A.Fake<ITreeNode>());
      }

      [Observation]
      public void should_not_notify_that_a_parameter_node_was_double_clicked()
      {
         _parameterNodeDoubleClicked.ShouldBeFalse();
      }

      [Observation]
      public void should_not_notify_that_a_covariate_node_was_double_clicked()
      {
         _covariateNodeDoubleClicked.ShouldBeFalse();
      }
   }

   public class When_the_presenter_is_told_to_remove_a_parameter : concern_for_PopulationParameterGroupsPresenter
   {
      private IParameter _parameter;
      private IParameter _anotherParameterThatDoesNotExist;
      private ITreeNode _node;
      private ITreeNode _rootNode;
      private IEnumerable<ITreeNode> _deletedNodes;

      protected override void Context()
      {
         base.Context();
         _node = A.Fake<ITreeNode>();
         _rootNode = new GroupNode(new Group {Name = "RootNode"});
         _parameter = A.Fake<IParameter>();
         _anotherParameterThatDoesNotExist = A.Fake<IParameter>();
         A.CallTo(() => _view.NodeFor(_parameter)).Returns(_node);
         A.CallTo(() => _node.RootNode).Returns(_rootNode);
         A.CallTo(() => _view.NodeFor(_anotherParameterThatDoesNotExist)).Returns(null);
         A.CallTo(() => _view.RemoveNodes(A<IEnumerable<ITreeNode>>._))
            .Invokes(x => _deletedNodes = x.GetArgument<IEnumerable<ITreeNode>>(0));
      }

      protected override void Because()
      {
         sut.RemoveParameter(_parameter);
         sut.RemoveParameter(_anotherParameterThatDoesNotExist);
      }

      [Observation]
      public void should_tell_the_view_to_delete_the_node_corresponding_to_the_parameter_if_the_node_exists()
      {
         _deletedNodes.ShouldOnlyContain(_rootNode, _node);
      }
   }

   public class When_the_presenter_is_told_to_select_a_parameer : concern_for_PopulationParameterGroupsPresenter
   {
      private IParameter _parameter;
      private ITreeNode _node;

      protected override void Context()
      {
         base.Context();
         _node = A.Fake<ITreeNode>();
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _view.NodeFor(_parameter)).Returns(_node);
      }

      protected override void Because()
      {
         sut.SelectParameter(_parameter);
      }

      [Observation]
      public void should_retrieve_the_node_for_the_paramete_and_select_the_node()
      {
         A.CallTo(() => _view.SelectNode(_node)).MustHaveHappened();
      }
   }

   public class When_the_presenter_is_asked_if_a_node_exists_for_a_given_parameter : concern_for_PopulationParameterGroupsPresenter
   {
      private IParameter _parameter;
      private IParameter _anotherParameterThatDoesNotExist;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameter>();
         _anotherParameterThatDoesNotExist = A.Fake<IParameter>();
         A.CallTo(() => _view.NodeFor(_parameter)).Returns(A.Fake<ITreeNode>());
         A.CallTo(() => _view.NodeFor(_anotherParameterThatDoesNotExist)).Returns(null);
      }

      [Observation]
      public void should_return_true_if_a_node_as_defined_otherwise_false()
      {
         sut.HasNodeFor(_parameter).ShouldBeTrue();
         sut.HasNodeFor(_anotherParameterThatDoesNotExist).ShouldBeFalse();
      }
   }

   public class When_the_presenter_is_asked_to_prune_a_root_node : concern_for_PopulationParameterGroupsPresenter
   {
      private ITreeNode _rootNode;
      private ITreeNode _group1;
      private ITreeNode _group2;
      private ITreeNode _group3;
      private ITreeNode _parameterNode;
      private IEnumerable<ITreeNode> _allDeletedNode;

      protected override void Context()
      {
         base.Context();
         _rootNode = A.Fake<ITreeNode>();
         _group1 = new GroupNode(new Group {Name = "_group1"});
         _group2 = new GroupNode(new Group {Name = "_group2"});
         _group3 = new GroupNode(new Group {Name = "_group3"});
         _rootNode = new GroupNode(new Group {Name = "_rootNode"});
         _parameterNode = new ObjectWithIdAndNameNode<IParameter>(new PKSimParameter());
         _rootNode.AddChild(_group1);
         _rootNode.AddChild(_group2);
         _rootNode.AddChild(_group3);
         _group3.AddChild(_parameterNode);
         _parameterNode = A.Fake<ITreeNode>();
      }

      protected override void Because()
      {
         _allDeletedNode = sut.PruneNode(_rootNode);
      }

      [Observation]
      public void should_return_the_nodes_that_should_be_deleted()
      {
         _allDeletedNode.ShouldContain(_group1, _group2);
      }
   }

   public class When_pruning_a_root_node_that_does_not_contain_any_node : concern_for_PopulationParameterGroupsPresenter
   {
      private ITreeNode _group1;
      private IEnumerable<ITreeNode> _allDeletedNode;

      protected override void Context()
      {
         base.Context();
         _group1 = new GroupNode(new Group {Name = "_group1"});
      }

      protected override void Because()
      {
         _allDeletedNode = sut.PruneNode(_group1);
      }

      [Observation]
      public void should_return_that_the_node_should_be_deleted()
      {
         _allDeletedNode.ShouldContain(_group1);
      }
   }
}