using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IExpressionContainerMapper
   {
      //TODO probably not needed anymore
      void UpdateProperties(ExpressionContainerDTO expressionDTO, MoleculeExpressionContainer expressionContainer);

      ExpressionContainerParameterDTO MapFrom(MoleculeExpressionContainer expressionContainer, IndividualMolecule individualMolecule,
         IParameter parameter);

      ExpressionContainerParameterDTO MapFrom(IndividualProtein individualMolecule, IParameter parameter);
   }

   public class ExpressionContainerMapper : IExpressionContainerMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IGroupRepository _groupRepository;
      private readonly IParameterToParameterDTOInContainerMapper<ExpressionContainerParameterDTO> _parameterMapper;

      public ExpressionContainerMapper(IParameterToParameterDTOInContainerMapper<ExpressionContainerParameterDTO> parameterMapper,
         IRepresentationInfoRepository representationInfoRepository, IGroupRepository groupRepository)
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
         // expressionDTO.RelativeExpressionParameter =
         //    _parameterMapper.MapFrom(expressionContainer.RelativeExpressionParameter, expressionDTO,
         //                             x => x.RelativeExpression, x => x.RelativeExpressionParameter);
      }

      public ExpressionContainerParameterDTO MapFrom(MoleculeExpressionContainer expressionContainer, IndividualMolecule individualMolecule,
         IParameter parameter)
      {
         var compartment = parameter.ParentContainer.ParentContainer;
         var organ = compartment.ParentContainer;
         //Relative expression parameters not in lumen should be displayed directly under the organism
         var compartmentName = parameter.IsExpression() && !parameter.IsInLumen() ? string.Empty : compartment.Name;
         return createExpressionContainerParameterDTOFrom(organ.Name, compartmentName, expressionContainer.GroupName, parameter);
      }

      public ExpressionContainerParameterDTO MapFrom(IndividualProtein individualMolecule, IParameter parameter)
      {
         //TODO DEFINE CONSTANT
         var containerName = parameter.Name.Contains("blood cells") ? CoreConstants.Compartment.BloodCells :
            parameter.Name.EndsWith("plasma") ? CoreConstants.Compartment.Plasma : CoreConstants.Compartment.VascularEndothelium;

         return createExpressionContainerParameterDTOFrom( string.Empty, containerName, parameter.GroupName, parameter);
      }

      private ExpressionContainerParameterDTO createExpressionContainerParameterDTOFrom(string containerName, string compartmentName,
         string groupName, IParameter moleculeParameter)
      {
         var moleculeName = moleculeParameter.ParentContainer.Name;
         var group = _groupRepository.GroupByName(groupName);

         var dto = new ExpressionContainerParameterDTO
         {
            MoleculeName = moleculeName,
            ContainerName = containerName,
            CompartmentName = compartmentName,
            GroupingPathDTO = _representationInfoRepository.PathElementFor(RepresentationObjectType.GROUP, groupName),
            ContainerPathDTO = _representationInfoRepository.PathElementFor(RepresentationObjectType.CONTAINER, containerName),
            CompartmentPathDTO = _representationInfoRepository.PathElementFor(RepresentationObjectType.CONTAINER, compartmentName),
            Sequence = group.Sequence
         };



         dto.Parameter = _parameterMapper.MapFrom(moleculeParameter, dto, x => x.Value, x => x.Parameter);
         return dto;
      }
   }
}