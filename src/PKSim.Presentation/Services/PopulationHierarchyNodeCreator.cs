using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Nodes;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Services
{
   public interface IPopulationHierarchyNodeCreator
   {
      IReadOnlyCollection<ITreeNode> CreateHierarchyNodeFor(IReadOnlyList<IParameter> allParameters);
      ITreeNode CreateHierarchyNodeFor(IParameter parameter);
   }

   public class PopulationHierarchyNodeCreator : IPopulationHierarchyNodeCreator
   {
      private readonly ITreeNodeFactory _treeNodeFactory;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public PopulationHierarchyNodeCreator(ITreeNodeFactory treeNodeFactory, IRepresentationInfoRepository representationInfoRepository)
      {
         _treeNodeFactory = treeNodeFactory;
         _representationInfoRepository = representationInfoRepository;
      }

      public IReadOnlyCollection<ITreeNode> CreateHierarchyNodeFor(IReadOnlyList<IParameter> allParameters)
      {
         return allParameters.Select(CreateHierarchyNodeFor).ToList();
      }

      public ITreeNode CreateHierarchyNodeFor(IParameter parameter)
      {
         var rootContainer = parameter.RootContainer;
         var currentContainer = parameter.ParentContainer;
         var currentNode = nodeFor(parameter);
         do
         {
            var parentNode = nodeFor(currentContainer);
            parentNode.AddChild(currentNode);
            currentNode = parentNode;
            currentContainer = currentContainer.ParentContainer;
         } while (currentContainer != rootContainer);

         return currentNode;
      }

      private ITreeNode nodeFor<T>(T entity) where T : class, IEntity
      {
         var representationInfo = _representationInfoRepository.InfoFor(entity);
         return _treeNodeFactory.CreateFor(entity, representationInfo);
      }
   }
}