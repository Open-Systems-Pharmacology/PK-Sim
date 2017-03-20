using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class FormulationMappingDTO : DxValidatableDTO
   {
      public ApplicationType ApplicationType { get; set; }
      public string FormulationKey { get; set; }
      public FormulationSelectionDTO Selection { get; set; }
      public Formulation Formulation => Selection?.BuildingBlock;

      public string Route => ApplicationType.Route;

      public FormulationMappingDTO()
      {
         Rules.AddRange(AllRules.All());
      }

      private static class AllRules
      {
         private static IBusinessRule formulationNotNull { get; } = CreateRule.For<FormulationMappingDTO>()
            .Property(x => x.Selection)
            .WithRule((dto, f) => !dto.ApplicationType.NeedsFormulation || f?.BuildingBlock != null)
            .WithError((dto, f) => PKSimConstants.Error.FormulationIsRequiredForType(dto.ApplicationType.ToString()));

         public static IEnumerable<IBusinessRule> All()
         {
            yield return formulationNotNull;
         }
      }
   }
}