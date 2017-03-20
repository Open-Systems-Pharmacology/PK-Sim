using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Nodes;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationHierarchyNodeCreator : ContextSpecification<IPopulationHierarchyNodeCreator>
   {
      private IRepresentationInfoRepository _representationInfoRepository;
      protected ITreeNodeFactory _treeNodeFactory;

      protected override void Context()
      {
         _representationInfoRepository= A.Fake<IRepresentationInfoRepository>();
         _treeNodeFactory= new TreeNodeFactoryForSpecs();
         sut = new PopulationHierarchyNodeCreator(_treeNodeFactory,_representationInfoRepository);
      }
   }

   public class When_creating_the_tree_hiearchy_for_a_parameter : concern_for_PopulationHierarchyNodeCreator
   {
      IParameter _parameter;
      private IContainer _parentContainer;
      private IContainer _grandParentContainer;
      private IContainer _rootContainer;
      private ITreeNode _node;

      protected override void Context()
      {
         base.Context();
         _parameter=new PKSimParameter();
         _parentContainer = new Container().WithName("Parent");
         _grandParentContainer = new Container().WithName("GrandParent");
         _rootContainer = new Container().WithName("Root");

         _parentContainer.Add(_parameter);
         _grandParentContainer.Add(_parentContainer);
         _rootContainer.Add(_grandParentContainer);
      }

      protected override void Because()
      {
         _node = sut.CreateHierarchyNodeFor(_parameter);
      }

      [Observation]
      public void should_have_created_a_node_for_the_parameter_and_all_its_ancestors_except_the_root_container()
      {
         _node.TagAsObject.ShouldBeEqualTo(_grandParentContainer);
         _node.AllLeafNodes.Count().ShouldBeEqualTo(1);
         _node.AllLeafNodes.ElementAt(0).TagAsObject.ShouldBeEqualTo(_parameter);
      }
   }
}	