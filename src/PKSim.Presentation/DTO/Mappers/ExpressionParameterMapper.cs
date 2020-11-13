using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IExpressionParameterMapper<TExpressionParameterDTO> : IMapper<IParameter, TExpressionParameterDTO> where TExpressionParameterDTO: ExpressionParameterDTO, new()
   {
   }

   public class ExpressionParameterMapper<TExpressionParameterDTO> : IExpressionParameterMapper<TExpressionParameterDTO> where TExpressionParameterDTO: ExpressionParameterDTO, new()
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IGroupRepository _groupRepository;
      private readonly IExpressionParameterTask _expressionParameterTask;
      private readonly IParameterToParameterDTOInContainerMapper<TExpressionParameterDTO> _parameterMapper;

      public ExpressionParameterMapper(
         IParameterToParameterDTOInContainerMapper<TExpressionParameterDTO> parameterMapper,
         IRepresentationInfoRepository representationInfoRepository,
         IGroupRepository groupRepository,
         IExpressionParameterTask expressionParameterTask)
      {
         _parameterMapper = parameterMapper;
         _representationInfoRepository = representationInfoRepository;
         _groupRepository = groupRepository;
         _expressionParameterTask = expressionParameterTask;
      }

      public TExpressionParameterDTO MapFrom(IParameter parameter)
      {
         var containerName = CoreConstants.ContainerName.GlobalExpressionContainerNameFor(parameter.Name);
         var groupName = _expressionParameterTask.ExpressionGroupFor(parameter);
         if (groupName == CoreConstants.Groups.VASCULAR_SYSTEM)
            return createExpressionContainerParameterDTOFrom(string.Empty, containerName, groupName, parameter);

         //Parameters are located in a molecule which is in a compartment => Parent.Parent
         var compartment = parameter.ParentContainer.ParentContainer;
         var organ = compartment.ParentContainer;
         //Relative expression parameters not in lumen should be displayed directly under the organism
         var compartmentName = parameter.IsExpression() && !parameter.IsInLumen() ? string.Empty : compartment.Name;
         return createExpressionContainerParameterDTOFrom(organ.Name, compartmentName, groupName, parameter);
      }

      private TExpressionParameterDTO createExpressionContainerParameterDTOFrom(string containerName, string compartmentName,
         string groupName, IParameter moleculeParameter)
      {
         var moleculeName = moleculeParameter.ParentContainer.Name;
         var group = _groupRepository.GroupByName(groupName);

         var dto = new TExpressionParameterDTO
         {
            MoleculeName = moleculeName,
            ContainerName = containerName,
            CompartmentName = compartmentName,
            GroupName = groupName,
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