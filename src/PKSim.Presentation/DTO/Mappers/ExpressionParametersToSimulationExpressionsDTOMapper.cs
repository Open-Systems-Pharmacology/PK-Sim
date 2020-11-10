using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IExpressionParametersToSimulationExpressionsDTOMapper : IMapper<IEnumerable<IParameter>, SimulationExpressionsDTO>
   {
   }

   public class ExpressionParametersToSimulationExpressionsDTOMapper : IExpressionParametersToSimulationExpressionsDTOMapper
   {
      private readonly IParameterToParameterDTOMapper _parameterMapper;
      private readonly IExpressionParameterMapper _expressionContainerMapper;

      public ExpressionParametersToSimulationExpressionsDTOMapper(
         IParameterToParameterDTOMapper parameterMapper,
         IExpressionParameterMapper expressionContainerMapper)
      {
         _parameterMapper = parameterMapper;
         _expressionContainerMapper = expressionContainerMapper;
      }

      public SimulationExpressionsDTO MapFrom(IEnumerable<IParameter> expressionParameters)
      {
         var allParameters = expressionParameters.ToList();

         return new SimulationExpressionsDTO(
            updateGlobalParameter(allParameters, CoreConstants.Parameters.REFERENCE_CONCENTRATION),
            updateGlobalParameter(allParameters, CoreConstants.Parameters.HALF_LIFE_LIVER),
            updateGlobalParameter(allParameters, CoreConstants.Parameters.HALF_LIFE_INTESTINE),
            relativeExpressionsFrom(allParameters).ToList()
         );
      }

      private IParameterDTO updateGlobalParameter(List<IParameter> allParameters, string globalParameterName)
      {
         var globalParameter = allParameters.FindByName(globalParameterName);
         allParameters.Remove(globalParameter);
         return _parameterMapper.MapFrom(globalParameter);
      }

      private IEnumerable<ExpressionParameterDTO> relativeExpressionsFrom(IReadOnlyList<IParameter> allParameters) =>
         allParameters.Select(expressionContainerFor);

      private ExpressionParameterDTO expressionContainerFor(IParameter relativeExpression) => _expressionContainerMapper.MapFrom(relativeExpression);
   }
}