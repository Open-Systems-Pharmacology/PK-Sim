using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using PKSim.Presentation.Nodes;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Mappers
{
   public interface IParameterContainerToTreeNodeMapper : IMapper<IContainer, ITreeNode>
   {
   }

   public class ParameterContainerToTreeNodeMapper : IParameterContainerToTreeNodeMapper
   {
      private readonly ITreeNodeFactory _treeNodeFactory;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public ParameterContainerToTreeNodeMapper(ITreeNodeFactory treeNodeFactory, IRepresentationInfoRepository representationInfoRepository)
      {
         _treeNodeFactory = treeNodeFactory;
         _representationInfoRepository = representationInfoRepository;
      }

      public ITreeNode MapFrom(IContainer container)
      {
         var node = createNodeForContainer(container);

         container.GetChildren<IContainer>(nodeShouldBeCreatedForContainer)
            .Each(childContainer => node.AddChild(MapFrom(childContainer)));

         return node;
      }

      private ITreeNode createNodeForContainer(IContainer container)
      {
         var representationInfo = _representationInfoRepository.InfoFor(container);
         var node = _treeNodeFactory.CreateFor(container, representationInfo);

         var moleculeAmount = container as MoleculeAmount;
         if (moleculeAmount != null)
            node.Icon = ApplicationIcons.IconByName(moleculeAmount.QuantityType.ToString());

         var reaction = container as Reaction;
         if (reaction != null)
            node.Icon = ApplicationIcons.Reaction;

         return node;
      }

      private bool nodeShouldBeCreatedForContainer(IContainer container)
      {
         if (container.IsAnImplementationOf<IDistributedParameter>())
            return false;

         return true;
         //no need to iterate through the colleciton. If there is at least one parameter, the node should be created
         //return container.GetAllChildren<IParameter>().FirstOrDefault() != null;
      }
   }
}