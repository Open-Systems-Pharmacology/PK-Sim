using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
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

         return new SimulationExpressionsDTO(allGlobalParameters, allParameters.Except(allGlobalParameters).MapAllUsing(_expressionContainerMapper)
         );
      }
   }
}