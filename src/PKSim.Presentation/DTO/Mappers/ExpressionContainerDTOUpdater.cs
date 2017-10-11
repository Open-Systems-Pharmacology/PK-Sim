using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Extensions;

using PKSim.Presentation.DTO.Individuals;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IExpressionContainerDTOUpdater
   {
      void UpdateProperties(ExpressionContainerDTO expressionDTO, MoleculeExpressionContainer expressionContainer);
   }

   public class ExpressionContainerDTOUpdater : IExpressionContainerDTOUpdater
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IGroupRepository _groupRepository;
      private readonly IParameterToParameterDTOInContainerMapper<ExpressionContainerDTO> _parameterMapper;

      public ExpressionContainerDTOUpdater(IParameterToParameterDTOInContainerMapper<ExpressionContainerDTO> parameterMapper, IRepresentationInfoRepository representationInfoRepository, IGroupRepository groupRepository)
      {
         _parameterMapper = parameterMapper;
         _representationInfoRepository = representationInfoRepository;
         _groupRepository = groupRepository;
      }

      public void UpdateProperties(ExpressionContainerDTO expressionDTO, MoleculeExpressionContainer expressionContainer)
      {
         var group = _groupRepository.GroupByName(expressionContainer.GroupName);
         var groupInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.GROUP, expressionContainer.GroupName);
         var containerInfo = _representationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, expressionContainer.ContainerName);

         expressionDTO.GroupingPathDTO = groupInfo.ToPathElement();
         expressionDTO.ContainerPathDTO = containerInfo.ToPathElement();
         expressionDTO.Sequence = group.Sequence;
         expressionDTO.RelativeExpressionParameter =
            _parameterMapper.MapFrom(expressionContainer.RelativeExpressionParameter, expressionDTO,
                                     x => x.RelativeExpression, x => x.RelativeExpressionParameter);

         expressionDTO.RelativeExpressionNormParameter =
            _parameterMapper.MapFrom(expressionContainer.RelativeExpressionNormParameter, expressionDTO,
                                     x => x.RelativeExpressionNorm, x => x.RelativeExpressionNormParameter);
      }
   }
}