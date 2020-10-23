using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using PKSim.Presentation.DTO.Individuals;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationExpressionsDTO
   {
      public IParameterDTO ReferenceConcentration { get; }
      public IParameterDTO HalfLifeLiver { get; }
      public IParameterDTO HalfLifeIntestine { get; }
      public IReadOnlyList<ExpressionParameterDTO> ExpressionParameters { get; }
      public IReadOnlyList<IParameter> MoleculeParameters { get; }
      public IReadOnlyList<IParameterDTO> AllParameters { get; }

      public SimulationExpressionsDTO(
         IParameterDTO referenceConcentration,
         IParameterDTO halfLifeLiver,
         IParameterDTO halfLifeIntestine,
         IReadOnlyList<ExpressionParameterDTO> expressionParameters)
      {
         ReferenceConcentration = referenceConcentration;
         HalfLifeLiver = halfLifeLiver;
         HalfLifeIntestine = halfLifeIntestine;
         ExpressionParameters = expressionParameters;
         MoleculeParameters = new[] {ReferenceConcentration.Parameter, HalfLifeLiver.Parameter, HalfLifeIntestine.Parameter};

         AllParameters = new List<IParameterDTO>(expressionParameters.Select(x => x.Parameter))
         {
            referenceConcentration, halfLifeLiver, halfLifeIntestine
         };
      }
   }
}