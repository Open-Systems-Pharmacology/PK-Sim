using OSPSuite.Core.Domain;
using PKSim.Core.Model;
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
      private readonly IExpressionParameterMapper<TransporterExpressionParameterDTO> _expressionContainerMapper;

      public IndividualTransporterToTransporterExpressionDTOMapper(
         IExpressionParameterMapper<TransporterExpressionParameterDTO> expressionContainerMapper)
      {
         _expressionContainerMapper = expressionContainerMapper;
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
            foreach (var parameter in transporterExpressionContainer.AllParameters())
            {
               var organ = transporterExpressionContainer.LogicalContainer;
               var expressionParameter = _expressionContainerMapper.MapFrom(parameter);
               var isInOrganWithLumenOrBrain = organ.IsOrganWithLumen() || organ.IsBrain();
               expressionParameter.TransporterExpressionContainer = transporterExpressionContainer;
               expressionParameter.IsInOrganWithLumenOrBrain = isInOrganWithLumenOrBrain;
               expressionParameter.TransportDirection =
                  retrieveTransporterDirectionFor(transporterExpressionContainer, parameter, isInOrganWithLumenOrBrain);
               individualTransporterDTO.AddExpressionParameter(expressionParameter);
            }
         }

         //Global parameters;
         foreach (var parameter in transporter.AllGlobalExpressionParameters)
         {
            var expressionParameter = _expressionContainerMapper.MapFrom(parameter);
            expressionParameter.TransporterExpressionContainer = parameter.IsNamed(REL_EXP_BLOOD_CELLS)
               ? transporter.BloodCellsContainer
               : transporter.VascularEndotheliumContainer;

            expressionParameter.TransportDirection = expressionParameter.TransporterExpressionContainer.TransportDirection;
            individualTransporterDTO.AddExpressionParameter(expressionParameter);
         }
      }

      private TransportDirection retrieveTransporterDirectionFor(TransporterExpressionContainer transporterExpressionContainer, IParameter parameter,
         bool isInOrganWithLumenOrBrain = false)
      {
         if (parameter.IsNamed(INITIAL_CONCENTRATION))
            return TransportDirections.None;
         //Organ without lumen only show transporter direction at the rel exp parameter level
         if (!isInOrganWithLumenOrBrain)
            return parameter.IsNamed(REL_EXP) ? transporterExpressionContainer.TransportDirection : TransportDirections.None;

         return !parameter.IsNamed(REL_EXP) ? transporterExpressionContainer.TransportDirection : TransportDirections.None;
      }
   }
}