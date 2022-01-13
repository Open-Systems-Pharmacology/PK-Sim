using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Parameters.Mappers
{
   public interface INodeToCustomizableParametersPresenterMapper : IMapper<ITreeNode, ICustomParametersPresenter>
   {
   }

   public class NodeToCustomizableParametersPresenterMapper : INodeToCustomizableParametersPresenterMapper
   {
      private readonly IContainerToCustomableParametersPresenterMapper _containerPresenterMapper;
      private readonly IParameterGroupToCustomizableParametersPresenter _parameterGroupPresenterMapper;
      private readonly IMultiParameterEditPresenterFactory _multiParameterEditPresenterFactory;

      public NodeToCustomizableParametersPresenterMapper(
         IContainerToCustomableParametersPresenterMapper containerPresenterMapper,
         IParameterGroupToCustomizableParametersPresenter parameterGroupPresenterMapper,
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