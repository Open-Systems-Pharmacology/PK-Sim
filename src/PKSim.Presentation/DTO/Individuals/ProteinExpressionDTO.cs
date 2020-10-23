using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Individuals
{
   public class ProteinExpressionDTO : ValidatableDTO<IndividualProtein>
   {
      private IndividualProtein _protein;

      public ProteinExpressionDTO(IndividualProtein protein) : base(protein)
      {
         _protein = protein;
      }

      //this code should be necessary. But it seems that a issue in dev express hold a reference top the dto=>hence we need to clear references to domain by hand
      public void ClearReferences()
      {
         //TODO
         _protein = null;
      //    _allContainerExpressions.Each(x => x.ClearReferences());
      //    _allContainerExpressions.Clear();
       }
   }
}