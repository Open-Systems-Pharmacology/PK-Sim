using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IIndividualTransporterToTransporterExpressionDTOMapper
   {
      IndividualTransporterDTO MapFrom(IndividualTransporter transporter, ISimulationSubject simulationSubject);
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

         //Local parameters
         foreach (var transporterExpressionContainer in simulationSubject.AllMoleculeContainersFor<TransporterExpressionContainer>(transporter))
         {
            //Two parents to move up the hierarchy => Organ/Comp/Transporter
            var isInOrganWithLumen = transporterExpressionContainer.LogicalContainer.IsOrganWithLumen();
            foreach (var parameter in transporterExpressionContainer.AllParameters())
            {
               var expressionParameter = _expressionContainerMapper.MapFrom(parameter);
               expressionParameter.TransporterExpressionContainer = transporterExpressionContainer;
               expressionParameter.TransportDirection =
                  retrieveTransporterDirectionFor(transporterExpressionContainer, parameter, isInOrganWithLumen);
               dto.AddExpressionParameter(expressionParameter);
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
            dto.AddExpressionParameter(expressionParameter);
         }

         return dto;
      }

      private TransportDirection retrieveTransporterDirectionFor(TransporterExpressionContainer transporterExpressionContainer, IParameter parameter,
         bool isInOrganWithLumen)
      {
         if (parameter.IsNamed(INITIAL_CONCENTRATION))
            return TransportDirections.None;

         if (!isInOrganWithLumen)
            return parameter.IsNamed(REL_EXP) ? transporterExpressionContainer.TransportDirection : TransportDirections.None;

         return !parameter.IsNamed(REL_EXP) ? transporterExpressionContainer.TransportDirection : TransportDirections.None;
      }
   }
}