using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using FakeItEasy;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterGroupNodeCreator : ContextSpecification<IParameterGroupNodeCreator>
   {
      protected IParameterGroupToTreeNodeMapper _parameterGroupToTreeNodeMapper;
      protected IGroup _topGroup;
      protected List<IParameter> _allParameters;
      protected IDynamicGroupExpander _dynamicGroupExpander;

      protected override void Context()
      {
         _topGroup = new Group {Name = "topGroup"};
         _allParameters = new List<IParameter>();
         _parameterGroupToTreeNodeMapper = A.Fake<IParameterGroupToTreeNodeMapper>();
         _dynamicGroupExpander = A.Fake<IDynamicGroupExpander>();
         sut = new ParameterGroupNodeCreator(_parameterGroupToTreeNodeMapper, _dynamicGroupExpander);
      }
   }

   public class When_resolving_the_parameter_group_node_for_a_group_containing_sub_groups_without_parmaeters : concern_for_ParameterGroupNodeCreator
   {
      private ITreeNode _rootNode;
      private ITreeNode _topNode;
      private ITreeNode _node1;
      private ITreeNode _node2;
      private ITreeNode _node3;
      private ITreeNode _node11;
      private ITreeNode _node21;
      private ITreeNode _node22;
      private ITreeNode _node31;
      private IGroup _group1;
      private IGroup _group2;
      private IGroup _group3;
      private IGroup _group11;
      private IGroup _group21;
      private IGroup _group22;
      private IGroup _group31;

      protected override void Context()
      {
         base.Context();
         _group1 = new Group {Name = "_group1"};
         _group2 = new Group {Name = "_group2"};
         _group3 = new Group {Name = "_group3"};
         _group11 = new Group {Name = "_group11"};
         _group21 = new Group {Name = "_group21"};
         _group22 = new Group {Name = "_group22"};
         _group31 = new Group {Name = "_group31"};

         _topNode = new GroupNode(_topGroup);
         _node1 = new GroupNode(_group1);
         _node2 = new GroupNode(_group2);
         _node3 = new GroupNode(_group3);
         _node11 = new GroupNode(_group11);
         _node21 = new GroupNode(_group21);
         _node22 = new GroupNode(_group22);
         _node31 = new GroupNode(_group31);

         _topNode.AddChild(_node1);
         _topNode.AddChild(_node2);
         _topNode.AddChild(_node3);
         _node1.AddChild(_node11);
         _node2.AddChild(_node21);
         _node2.AddChild(_node22);
         _node3.AddChild(_node31);


         var p11 = A.Fake<IParameter>().WithGroup(_group11.Name);
         var p21 = A.Fake<IParameter>().WithGroup(_group21.Name);
         _allParameters.Add(p11);
         _allParameters.Add(p21);
         A.CallTo(() => _parameterGroupToTreeNodeMapper.MapFrom(_topGroup)).Returns(_topNode);
      }

      //Node 3 has only one child _node31 with no paraeters=>_node3 should have been deleted completly
      protected override void Because()
      {
         _rootNode = sut.MapFrom(_topGroup, _allParameters);
      }

      [Observation]
      public void should_return_a_node_containg_only_the_sub_groups_with_parameters()
      {
         _rootNode.Children.ShouldOnlyContain(_node1, _node2);
      }

      [Observation]
      public void should_have_removed_the_node_in_the_hierarchy_witout_parameters()
      {
         _node1.Children.ShouldOnlyContain(_node11);
         _node2.Children.ShouldOnlyContain(_node21);
      }

      [Observation]
      public void should_add_the_dynamic_node_the_leaf_groups()
      {
         A.CallTo(() => _dynamicGroupExpander.AddDynamicGroupNodesTo(_node11, _allParameters)).MustHaveHappened();
         A.CallTo(() => _dynamicGroupExpander.AddDynamicGroupNodesTo(_node21, _allParameters)).MustHaveHappened();
      }
   }

   public class When_resolving_the_parameter_group_node_for_a_single_node_without_parameters : concern_for_ParameterGroupNodeCreator
   {
      private ITreeNode _rootNode;
      private ITreeNode _topNode;

      protected override void Context()
      {
         base.Context();
         _topNode = new GroupNode(_topGroup);
         A.CallTo(() => _parameterGroupToTreeNodeMapper.MapFrom(_topGroup)).Returns(_topNode);
      }

      protected override void Because()
      {
         _rootNode = sut.MapFrom(_topGroup, _allParameters);
      }

      [Observation]
      public void should_return_the_node()
      {
         _rootNode.ShouldBeEqualTo(_topNode);
      }

      [Observation]
      public void should_add_the_dynamic_node_the_top_group()
      {
         A.CallTo(() => _dynamicGroupExpander.AddDynamicGroupNodesTo(_topNode, _allParameters)).MustHaveHappened();
      }
   }
}