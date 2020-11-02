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
      ExpressionParameterDTO MapFrom(MoleculeExpressionContainer expressionContainer, IndividualMolecule individualMolecule,
         IParameter parameter);

      ExpressionParameterDTO MapFrom(IndividualMolecule individualMolecule, IParameter parameter);
   }

   public class ExpressionContainerMapper : IExpressionContainerMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IGroupRepository _groupRepository;
      private readonly IParameterToParameterDTOInContainerMapper<ExpressionParameterDTO> _parameterMapper;

      public ExpressionContainerMapper(IParameterToParameterDTOInContainerMapper<ExpressionParameterDTO> parameterMapper,
         IRepresentationInfoRepository representationInfoRepository, IGroupRepository groupRepository)
      {
         _parameterMapper = parameterMapper;
         _representationInfoRepository = representationInfoRepository;
         _groupRepository = groupRepository;
      }

      public ExpressionParameterDTO MapFrom(MoleculeExpressionContainer expressionContainer, IndividualMolecule individualMolecule,
         IParameter parameter)
      {
         var compartment = parameter.ParentContainer.ParentContainer;
         var organ = compartment.ParentContainer;
         //Relative expression parameters not in lumen should be displayed directly under the organism
         var compartmentName = parameter.IsExpression() && !parameter.IsInLumen() ? string.Empty : compartment.Name;
         return createExpressionContainerParameterDTOFrom(organ.Name, compartmentName, expressionContainer.GroupName, parameter);
      }

      public ExpressionParameterDTO MapFrom(IndividualMolecule individualMolecule, IParameter parameter)
      {
         var containerName = CoreConstants.ContainerName.GlobalExpressionContainerNameFor(parameter.Name);
         return createExpressionContainerParameterDTOFrom(string.Empty, containerName, parameter.GroupName, parameter);
      }

      private ExpressionParameterDTO createExpressionContainerParameterDTOFrom(string containerName, string compartmentName,
         string groupName, IParameter moleculeParameter)
      {
         var moleculeName = moleculeParameter.ParentContainer.Name;
         var group = _groupRepository.GroupByName(groupName);

         var dto = new ExpressionParameterDTO
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