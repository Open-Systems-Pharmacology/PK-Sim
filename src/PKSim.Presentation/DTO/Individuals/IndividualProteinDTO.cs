using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Individuals
{
   public class IndividualProteinDTO : ValidatableDTO<IndividualProtein>
   {
      private readonly IndividualProtein _individualProtein;
      private readonly List<ExpressionParameterDTO> _allExpressionParameters = new List<ExpressionParameterDTO>();

      public IndividualProteinDTO(IndividualProtein individualProtein) : base(individualProtein)
      {
         _individualProtein = individualProtein;
      }

      public IReadOnlyList<ExpressionParameterDTO> AllExpressionParameters => _allExpressionParameters;


      public void AddExpressionParameter(ExpressionParameterDTO expressionParameterDTO)
      {
         _allExpressionParameters.Add(expressionParameterDTO);
      }
   }
}