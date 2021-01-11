using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationExpressionsDTO
   {
      public IReadOnlyList<ExpressionParameterDTO> ExpressionParameters { get; }
      public IReadOnlyList<IParameter> MoleculeParameters { get; }

      public SimulationExpressionsDTO(
         IReadOnlyList<IParameter> moleculeParameters,
         IReadOnlyList<ExpressionParameterDTO> expressionParameters)
      {
         ExpressionParameters = expressionParameters;
         MoleculeParameters = moleculeParameters;
      }
   }
}