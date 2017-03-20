using System.Collections.Generic;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationExpressionsDTO
   {
      public IParameterDTO ReferenceConcentration { get; set; }
      public IParameterDTO HalfLifeLiver { get; set; }
      public IParameterDTO HalfLifeIntestine { get; set; }
      public IEnumerable<ExpressionContainerDTO> RelativeExpressions { get; set; }

      public IEnumerable<IParameterDTO> AllParameters()
      {
         yield return ReferenceConcentration;
         yield return HalfLifeLiver;
         yield return HalfLifeIntestine;

         foreach (var expressionContainer in RelativeExpressions)
         {
            yield return expressionContainer.RelativeExpressionParameter;
         }
      }
   }
}