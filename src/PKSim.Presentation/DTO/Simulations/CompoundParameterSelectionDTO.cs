using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class CompoundParameterSelectionDTO : DxValidatableDTO
   {
      public ParameterAlternative SelectedAlternative { get; set; }
      public string ParameterName { get; set; }

      public CompoundParameterSelectionDTO(ParameterAlternativeGroup compoundParameterGroup)
      {
         CompoundParameterGroup = compoundParameterGroup;
         Rules.AddRange(AllRules.All());
      }

      public ParameterAlternativeGroup CompoundParameterGroup { get; }

      private static class AllRules
      {
         public static IEnumerable<IBusinessRule> All()
         {
            yield return selectionNotEmpty;
         }
      }

      private static IBusinessRule selectionNotEmpty { get; } = CreateRule.For<CompoundParameterSelectionDTO>()
         .Property(item => item.SelectedAlternative)
         .WithRule((param, value) => value != null)
         .WithError((param, value) => PKSimConstants.Error.CompoundParameterSelectionNeededFor(param.ParameterName));
   }
}