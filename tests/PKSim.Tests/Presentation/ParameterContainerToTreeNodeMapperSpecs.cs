using System.Linq;
using OSPSuite.Presentation.Nodes;

using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Nodes;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterContainerToTreeNodeMapper : ContextSpecification<IParameterContainerToTreeNodeMapper>
   {
      protected ITreeNodeFactory _treeNodeFactory;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IReportGenerator _reportGenerator;

      protected override void Context()
      {
         _representationInfoRepository  = A.Fake<IRepresentationInfoRepository>();
         _reportGenerator =A.Fake<IReportGenerator>();
         _treeNodeFactory = new TreeNodeFactoryForSpecs();
         sut = new ParameterContainerToTreeNodeMapper(_treeNodeFactory,_representationInfoRepository);
      }
   }

   
   public class When_mapping_a_container_to_a_tree_node_ : concern_for_ParameterContainerToTreeNodeMapper
   {
      private IContainer _container;
      private ITreeNode _node;
      private RepresentationInfo _availableInfo;

      protected override void Context()
      {
         base.Context();
         _container = new Container();
         var childContainer1 = new Container().WithId("childContainer1").WithName("childContainer1");
         var childChildContainer1 = new Container().WithId("childChildContainer1").WithName("childChildContainer1");
         childChildContainer1.Add(new Parameter().WithId("parameterInChildChildContainer1").WithName("parameterInChildChildContainer1"));
         var childContainer2 = new Container().WithId("childContainer2").WithName("childContainer2");
         childContainer2.Add(new Parameter().WithId("parameterInchildContainer2").WithName("parameterInchildContainer2"));
         var childContainer3 = new Container().WithId("_childContainer3").WithName("_childContainer3");
         var distrubutedParameter = new DistributedParameter().WithId("DistributedParameter").WithName("DistributedParameter");
         childContainer1.Add(childChildContainer1);
         _container.Add(childContainer1);
         _container.Add(childContainer2);
         _container.Add(childContainer3);
         childChildContainer1.Add(distrubutedParameter);
         _availableInfo =  new RepresentationInfo {DisplayName = "tralal", IconName = "Stop"};
         A.CallTo(() => _representationInfoRepository.InfoFor(_container)).Returns(_availableInfo);
         A.CallTo(() => _representationInfoRepository.InfoFor(childChildContainer1)).Returns(new RepresentationInfo());
         A.CallTo(() => _representationInfoRepository.InfoFor(childContainer2)).Returns(new RepresentationInfo());
         A.CallTo(() => _representationInfoRepository.InfoFor(childContainer1)).Returns(new RepresentationInfo());
         A.CallTo(() => _representationInfoRepository.InfoFor(childContainer3)).Returns(new RepresentationInfo());
         A.CallTo(() => _representationInfoRepository.InfoFor(distrubutedParameter)).Returns(new RepresentationInfo());
      }

      protected override void Because()
      {
         _node = sut.MapFrom(_container);
      }

      [Observation]
      public void should_return_a_node_with_one_child_node_for_each_child_container_of_the_mapped_container()
      {
         _node.Children.Count().ShouldBeEqualTo(3);
         //1 because child containr 1 has one sub container with one parameter
         _node.Children.First().Children.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_not_add_the_distributed_parameter_as_node_of_the_hierarchy()
      {
         var firstNode = _node.Children.First();
         var subNode = firstNode.Children.First();
         subNode.Children.Count().ShouldBeEqualTo(0);
      }


      [Observation]
      public void should_have_set_the_text_and_the_icon_for_each_node_according_to_the_representation_info()
      {
         _node.Text.ShouldBeEqualTo(_availableInfo.DisplayName);
         _node.Icon.ShouldBeEqualTo(ApplicationIcons.IconByName(_availableInfo.IconName));
      }
   }
}	