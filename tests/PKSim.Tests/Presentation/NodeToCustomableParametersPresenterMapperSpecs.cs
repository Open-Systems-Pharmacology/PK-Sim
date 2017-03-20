using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Nodes;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Parameters.Mappers;
using OSPSuite.Core.Domain;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_NodeToCustomableParametersPresenterMapper : ContextSpecification<INodeToCustomableParametersPresenterMapper>
   {
      protected IParameterGroupToCustomableParametersPresenter _parameterGroupPresenterMapper;
      protected IContainerToCustomableParametersPresenterMapper _containerPresentrMapper;
      protected IMultiParameterEditPresenter _multiEditPresenter;
      protected IMultiParameterEditPresenterFactory _multiEditPresenterFactory;

      protected override void Context()
      {
         _multiEditPresenter = A.Fake<IMultiParameterEditPresenter>();
         _parameterGroupPresenterMapper = A.Fake<IParameterGroupToCustomableParametersPresenter>();
         _containerPresentrMapper = A.Fake<IContainerToCustomableParametersPresenterMapper>();
         _multiEditPresenterFactory = A.Fake<IMultiParameterEditPresenterFactory>();
         A.CallTo(() => _multiEditPresenterFactory.Create()).Returns(_multiEditPresenter);
         sut = new NodeToCustomableParametersPresenterMapper(_containerPresentrMapper, _parameterGroupPresenterMapper, _multiEditPresenterFactory);
      }
   }

   
   public class When_mapping_a_parameter_group_node_to_a_parameter_presenter : concern_for_NodeToCustomableParametersPresenterMapper
   {
      private ITreeNode<IGroup> _node;
      private ICustomParametersPresenter _presenter;

      protected override void Context()
      {
         base.Context();
         _node = A.Fake<ITreeNode<IGroup>>();
         _presenter = A.Fake<ICustomParametersPresenter>();
         A.CallTo(() => _node.Tag).Returns(A.Fake<IGroup>());
         A.CallTo(() => _parameterGroupPresenterMapper.MapFrom(_node.Tag)).Returns(_presenter);
      }

      [Observation]
      public void should_return_a_presenter_for_a_parameter_group()
      {
         sut.MapFrom(_node).ShouldBeEqualTo(_presenter);
      }
   }

   
   public class When_mapping_a_container_node_to_a_parameter_presenter : concern_for_NodeToCustomableParametersPresenterMapper
   {
      private ITreeNode<IContainer> _node;
      private ICustomParametersPresenter _presenter;

      protected override void Context()
      {
         base.Context();
         _node = A.Fake<ITreeNode<IContainer>>();
         _presenter = A.Fake<ICustomParametersPresenter>();
         A.CallTo(() => _node.Tag).Returns(A.Fake<IContainer>());
         A.CallTo(() => _containerPresentrMapper.MapFrom(_node.Tag)).Returns(_presenter);
      }

      [Observation]
      public void should_return_a_presenter_for_a_parameter_group()
      {
         sut.MapFrom(_node).ShouldBeEqualTo(_presenter);
      }
   }

   
   public class When_mapping_an_unknow_node_to_a_parameter_presenter : concern_for_NodeToCustomableParametersPresenterMapper
   {
      private ITreeNode _node;

      protected override void Context()
      {
         base.Context();
         _node = A.Fake<ITreeNode>();
      }

      [Observation]
      public void should_return_a_multi_edit_parameter_presenter()
      {
         sut.MapFrom(_node).ShouldBeEqualTo(_multiEditPresenter);
      }
   }
}