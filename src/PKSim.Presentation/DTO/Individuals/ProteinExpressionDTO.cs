using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Individuals
{
   public class ProteinExpressionDTO : ValidatableDTO<IndividualProtein>
   {
      private readonly IList<ExpressionContainerDTO> _allContainerExpressions;
      private IndividualProtein _protein;
      public ParameterDTO ReferenceConcentrationParameter { get; set; }
      public double ReferenceConcentration { get; set; }

      public ProteinExpressionDTO(IndividualProtein protein) : base(protein)
      {
         _allContainerExpressions = new List<ExpressionContainerDTO>();
         _protein = protein;
      }

      public IEnumerable<ExpressionContainerDTO> AllContainerExpressions
      {
         get { return _allContainerExpressions; }
      }

      public void AddProteinExpression(ExpressionContainerDTO expressionContainerDTO)
      {
         _allContainerExpressions.Add(expressionContainerDTO);
      }

      public TissueLocation TissueLocation
      {
         get { return _protein.TissueLocation; }
         set { _protein.TissueLocation = value; }
      }

      public IntracellularVascularEndoLocation IntracellularVascularEndoLocation
      {
         get { return _protein.IntracellularVascularEndoLocation; }
         set { _protein.IntracellularVascularEndoLocation = value; }
      }

      public MembraneLocation MembraneLocation
      {
         get { return _protein.MembraneLocation; }
         set { _protein.MembraneLocation = value; }
      }

      //this code should be necessary. But it seems that a issue in dev express hold a reference top the dto=>hence we need to clear references to domain by hand
      public void ClearReferences()
      {
         _protein = null;
         _allContainerExpressions.Each(x => x.ClearReferences());
         _allContainerExpressions.Clear();
      }
   }
}