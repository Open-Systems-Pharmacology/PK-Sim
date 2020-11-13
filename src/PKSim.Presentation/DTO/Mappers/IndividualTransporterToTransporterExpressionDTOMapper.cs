using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;

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
         simulationSubject.AllMoleculeContainersFor(transporter)
            .SelectMany(x => x.AllParameters())
            .Union(transporter.AllGlobalExpressionParameters)
            .MapAllUsing(_expressionContainerMapper)
            .Each(dto.AddExpressionParameter);
         return dto;
      }
   }
}