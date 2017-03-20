using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Parameters.Mappers
{
   public interface INodeToCustomableParametersPresenterMapper : IMapper<ITreeNode, ICustomParametersPresenter>
   {
   }

   public class NodeToCustomableParametersPresenterMapper : INodeToCustomableParametersPresenterMapper
   {
      private readonly IContainerToCustomableParametersPresenterMapper _containerPresenterMapper;
      private readonly IParameterGroupToCustomableParametersPresenter _parameterGroupPresenterMapper;
      private readonly IMultiParameterEditPresenterFactory _multiParameterEditPresenterFactory;

      public NodeToCustomableParametersPresenterMapper(IContainerToCustomableParametersPresenterMapper containerPresenterMapper,
                                                       IParameterGroupToCustomableParametersPresenter parameterGroupPresenterMapper,
                                                       IMultiParameterEditPresenterFactory multiParameterEditPresenterFactory)
      {
         _containerPresenterMapper = containerPresenterMapper;
         _parameterGroupPresenterMapper = parameterGroupPresenterMapper;
         _multiParameterEditPresenterFactory = multiParameterEditPresenterFactory;
      }

      public ICustomParametersPresenter MapFrom(ITreeNode node)
      {
         var parameterGroupNode = node as ITreeNode<IGroup>;
         if (parameterGroupNode != null)
            return _parameterGroupPresenterMapper.MapFrom(parameterGroupNode.Tag);

         var containerNode = node as ITreeNode<IContainer>;
         if (containerNode != null)
            return _containerPresenterMapper.MapFrom(containerNode.Tag);

         return _multiParameterEditPresenterFactory.Create();
      }
   }
}