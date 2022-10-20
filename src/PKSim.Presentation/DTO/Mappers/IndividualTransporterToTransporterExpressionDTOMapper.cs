using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Individuals;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualTransporterToTransporterExpressionDTOMapper
   {
      IndividualTransporterDTO MapFrom(IndividualTransporter transporter, ISimulationSubject simulationSubject);
      void UpdateExpressionParameters(IndividualTransporterDTO individualTransporterDTO, ISimulationSubject simulationSubject);
   }

   public class IndividualTransporterToTransporterExpressionDTOMapper : IIndividualTransporterToTransporterExpressionDTOMapper
   {
      private readonly IExpressionParameterMapper<TransporterExpressionParameterDTO> _expressionParameterMapper;
      private readonly ITransportDirectionRepository _transportDirectionRepository;
      private readonly ITransporterTemplateRepository _transporterTemplateRepository;

      public IndividualTransporterToTransporterExpressionDTOMapper(
         IExpressionParameterMapper<TransporterExpressionParameterDTO> expressionParameterMapper,
         ITransportDirectionRepository transportDirectionRepository,
         ITransporterTemplateRepository transporterTemplateRepository
         )
      {
         _expressionParameterMapper = expressionParameterMapper;
         _transportDirectionRepository = transportDirectionRepository;
         _transporterTemplateRepository = transporterTemplateRepository;
      }

      public IndividualTransporterDTO MapFrom(IndividualTransporter transporter, ISimulationSubject simulationSubject)
      {
         var dto = new IndividualTransporterDTO(transporter);
         UpdateExpressionParameters(dto, simulationSubject);
         return dto;
      }

      public void UpdateExpressionParameters(IndividualTransporterDTO individualTransporterDTO, ISimulationSubject simulationSubject)
      {
         var transporter = individualTransporterDTO.Transporter;
         individualTransporterDTO.ClearExpressionParameters();

         //Local parameters
         foreach (var transporterExpressionContainer in simulationSubject.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter))
         {
            var organ = transporterExpressionContainer.LogicalContainer;
            var isInOrganWithLumenOrBrain = organ.IsOrganWithLumen() || organ.IsBrain();

            foreach (var parameter in transporterExpressionContainer.AllParameters())
            {
               var expressionParameterDTO = _expressionParameterMapper.MapFrom(parameter);
               expressionParameterDTO.TransporterExpressionContainer = transporterExpressionContainer;
               expressionParameterDTO.IsInOrganWithLumenOrBrain = isInOrganWithLumenOrBrain;
               var direction = retrieveTransporterDirectionFor(transporterExpressionContainer, parameter, isInOrganWithLumenOrBrain);
               expressionParameterDTO.TransportDirection = _transportDirectionRepository.ById(direction);
               individualTransporterDTO.AddExpressionParameter(expressionParameterDTO);
            }
         }

         //Global parameters;
         foreach (var parameter in transporter.AllGlobalExpressionParameters)
         {
            var expressionParameter = _expressionParameterMapper.MapFrom(parameter);
            expressionParameter.TransporterExpressionContainer = parameter.IsNamed(REL_EXP_BLOOD_CELLS)
               ? transporter.BloodCellsContainer
               : transporter.VascularEndotheliumContainer;

            var direction = expressionParameter.TransporterExpressionContainer.TransportDirection;
            expressionParameter.TransportDirection = _transportDirectionRepository.ById(direction);
            individualTransporterDTO.AddExpressionParameter(expressionParameter);
         }

         var hasTemplateForSpecies = _transporterTemplateRepository.HasTransporterTemplateFor(simulationSubject.Species.Name, transporter.Name);
         individualTransporterDTO.DefaultAvailableInDatabase = hasTemplateForSpecies;
      }

      private TransportDirectionId retrieveTransporterDirectionFor(TransporterExpressionContainer transporterExpressionContainer, IParameter parameter,
         bool isInOrganWithLumenOrBrain = false)
      {
         if (parameter.IsNamed(INITIAL_CONCENTRATION))
            return TransportDirectionId.None;
         //Organ without lumen only show transporter direction at the rel exp parameter level
         if (!isInOrganWithLumenOrBrain)
            return parameter.IsNamed(REL_EXP) ? transporterExpressionContainer.TransportDirection : TransportDirectionId.None;

         return !parameter.IsNamed(REL_EXP) ? transporterExpressionContainer.TransportDirection : TransportDirectionId.None;
      }

   }
}