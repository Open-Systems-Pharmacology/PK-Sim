using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Mappers
{
   public interface IProteinExpressionContainerToExpressionContainerInfoMapper : IMapper<MoleculeExpressionContainer, ExpressionContainerInfo>
   {
   }

   public class ProteinExpressionContainerToExpressionContainerInfoMapper : IProteinExpressionContainerToExpressionContainerInfoMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public ProteinExpressionContainerToExpressionContainerInfoMapper(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public ExpressionContainerInfo MapFrom(MoleculeExpressionContainer moleculeExpressionContainer)
      {
         return new ExpressionContainerInfo(moleculeExpressionContainer.Name,
                                            _representationInfoRepository.DisplayNameFor(moleculeExpressionContainer),
                                            moleculeExpressionContainer.RelativeExpression);
      }
   }
}