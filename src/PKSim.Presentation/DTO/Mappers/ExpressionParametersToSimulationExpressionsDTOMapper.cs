using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IExpressionParametersToSimulationExpressionsDTOMapper : IMapper<IEnumerable<IParameter>, SimulationExpressionsDTO>
   {
   }

   public class ExpressionParametersToSimulationExpressionsDTOMapper : IExpressionParametersToSimulationExpressionsDTOMapper
   {
      private readonly IExpressionParameterMapper<ExpressionParameterDTO> _expressionContainerMapper;

      public ExpressionParametersToSimulationExpressionsDTOMapper(
         IExpressionParameterMapper<ExpressionParameterDTO> expressionContainerMapper)
      {
         _expressionContainerMapper = expressionContainerMapper;
      }

      public SimulationExpressionsDTO MapFrom(IEnumerable<IParameter> expressionParameters)
      {
         var allParameters = expressionParameters.ToList();
         var allGlobalParameters = allParameters.AllGlobalMoleculeParameters();

         var allExpressionParametersDTOs = allParameters.Except(allGlobalParameters).MapAllUsing(_expressionContainerMapper)
            .OrderBy(x => x.Sequence)
            //we do not want to disrupt the sequence that is based on physiological order except for global surrogate where the container is not defined
            .ThenBy(x => string.IsNullOrEmpty(x.ContainerName) ? x.CompartmentName : x.Sequence.ToString())
            .ToArray();

         return new SimulationExpressionsDTO(allGlobalParameters, allExpressionParametersDTOs);
      }
   }
}