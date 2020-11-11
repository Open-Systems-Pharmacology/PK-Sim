using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
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

         var allGlobalParameters = allParameters.Where(x => x.NameIsOneOf(
            CoreConstants.Parameters.REFERENCE_CONCENTRATION,
            CoreConstants.Parameters.HALF_LIFE_LIVER,
            CoreConstants.Parameters.HALF_LIFE_INTESTINE)
         ).ToArray();


         return new SimulationExpressionsDTO(allGlobalParameters, allParameters.Except(allGlobalParameters).MapAllUsing(_expressionContainerMapper)
         );
      }
   }
}