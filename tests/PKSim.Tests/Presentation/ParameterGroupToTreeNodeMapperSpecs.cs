using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using FakeItEasy;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Nodes;
using OSPSuite.Core.Domain;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterGroupToTreeNodeMapper : ContextSpecification<IParameterGroupToTreeNodeMapper>
   {
      protected IGroup _group;
      protected IGroup _childGroup;
      protected ITreeNodeFactory _treeNodeFactory;
      protected IGroup _grandChildGroup;
      protected IGroup _hiddenChildGroup;

      protected override void Context()
      {
         _group = A.Fake<IGroup>();
         _group.Visible = true;

         _childGroup = A.Fake<IGroup>();
         _childGroup.Visible = true;
         _hiddenChildGroup = A.Fake<IGroup>();
         _hiddenChildGroup.Visible = false;
         _grandChildGroup = A.Fake<IGroup>();
         _grandChildGroup.Visible = true;

         A.CallTo(() => _childGroup.Name).Returns("Tata");
         _childGroup.DisplayName = "Tata";
         _childGroup.PopDisplayName = "TataPop";

         A.CallTo(() => _grandChildGroup.Name).Returns("Titi");
         _grandChildGroup.DisplayName = "Titi";
         _grandChildGroup.PopDisplayName = "TitiPop";

         A.CallTo(() => _hiddenChildGroup.Name).Returns("hidden");
         _hiddenChildGroup.DisplayName = "hidden";
         _hiddenChildGroup.PopDisplayName = "hiddenPop";

         A.CallTo(() => _group.Children).Returns(new[] {_childGroup, _hiddenChildGroup});
         A.CallTo(() => _hiddenChildGroup.Name).Returns("Toto");
         _hiddenChildGroup.DisplayName = "Toto";
         _hiddenChildGroup.PopDisplayName = "TotoPop";

         A.CallTo(() => _childGroup.Children).Returns(new[] {_grandChildGroup});
         A.CallTo(() => _grandChildGroup.Children).Returns(new List<IGroup>());
         _treeNodeFactory = new TreeNodeFactoryForSpecs();

         sut = new ParameterGroupToTreeNodeMapper(_treeNodeFactory);
      }
   }

   public class When_mapping_a_parameter_group : concern_for_ParameterGroupToTreeNodeMapper
   {
      private ITreeNode _result;

      protected override void Because()
      {
         _result = sut.MapFrom(_group);
      }

      [Observation]
      public void should_return_a_node_with_name_and_id_set_to_those_of_the_parameter_group()
      {
         _result.Text.ShouldBeEqualTo(_group.DisplayName);
         _result.Id.ShouldBeEqualTo(_group.Name);
      }

      [Observation]
      public void should_only_add_visible_groups_to_the_hiearchy()
      {
         IList<ITreeNode> children = _result.Children.ToList();
         children.Count(x => x.Id == _hiddenChildGroup.DisplayName).ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_create_one_sub_node_for_each_sub_parameter_group()
      {
         IList<ITreeNode> children = _result.Children.ToList();
         children.Count.ShouldBeEqualTo(1);
         ITreeNode child = children[0];
         child.Text.ShouldBeEqualTo(_childGroup.DisplayName);
         IList<ITreeNode> grandChildren = child.Children.ToList();
         grandChildren.Count.ShouldBeEqualTo(1);
         grandChildren[0].Text.ShouldBeEqualTo(_grandChildGroup.DisplayName);
      }
   }

   public class When_mapping_a_parameter_group_for_population : concern_for_ParameterGroupToTreeNodeMapper
   {
      private ITreeNode _result;

      protected override void Because()
      {
         _result = sut.MapForPopulationFrom(_group);
      }

      [Observation]
      public void should_return_a_node_with_name_and_id_set_to_those_of_the_parameter_group()
      {
         _result.Text.ShouldBeEqualTo(_group.PopDisplayName);
         _result.Id.ShouldBeEqualTo(_group.Name);
      }

      [Observation]
      public void should_only_add_visible_groups_to_the_hiearchy()
      {
         IList<ITreeNode> children = _result.Children.ToList();
         children.Count(x => x.Id == _hiddenChildGroup.Name).ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_create_one_sub_node_for_each_sub_parameter_group()
      {
         IList<ITreeNode> children = _result.Children.ToList();
         children.Count.ShouldBeEqualTo(1);
         ITreeNode child = children[0];
         child.Text.ShouldBeEqualTo(_childGroup.PopDisplayName);
         IList<ITreeNode> grandChildren = child.Children.ToList();
         grandChildren.Count.ShouldBeEqualTo(1);
         grandChildren[0].Text.ShouldBeEqualTo(_grandChildGroup.PopDisplayName);
      }
   }
}