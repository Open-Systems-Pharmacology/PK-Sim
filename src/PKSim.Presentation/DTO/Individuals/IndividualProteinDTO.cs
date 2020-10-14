using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Individuals
{
   public class IndividualProteinDTO : ValidatableDTO<IndividualProtein>
   {
      private readonly IndividualProtein _individualProtein;
      private readonly List<ExpressionContainerParameterDTO> _allExpressionContainerParameters = new List<ExpressionContainerParameterDTO>();

      public IndividualProteinDTO(IndividualProtein individualProtein) : base(individualProtein)
      {
         _individualProtein = individualProtein;
      }

      public IReadOnlyList<ExpressionContainerParameterDTO> AllExpressionContainerParameters => _allExpressionContainerParameters;


      public IEnumerable<ExpressionContainerParameterDTO> AllVisibleExpressionContainerParameters => _allExpressionContainerParameters.Where(x => x.Visible);

      public void AddExpressionContainerParameter(ExpressionContainerParameterDTO expressionContainerParameterDTO)
      {
         _allExpressionContainerParameters.Add(expressionContainerParameterDTO);
      }
   }
}