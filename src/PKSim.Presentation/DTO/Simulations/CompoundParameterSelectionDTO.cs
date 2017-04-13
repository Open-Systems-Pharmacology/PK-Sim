using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Simulations
{
   public class CompoundParameterSelectionDTO : DxValidatableDTO
   {
      private readonly ParameterAlternativeGroup _compoundParameterGroup;
      public ParameterAlternative SelectedAlternative { get; set; }
      public string ParameterName { get; set; }

      public CompoundParameterSelectionDTO(ParameterAlternativeGroup compoundParameterGroup)
      {
         _compoundParameterGroup = compoundParameterGroup;
         Rules.AddRange(AllRules.All());
      }

      public ParameterAlternativeGroup CompoundParameterGroup
      {
         get { return _compoundParameterGroup; }
      }

      private static class AllRules
      {
         public static IEnumerable<IBusinessRule> All()
         {
            yield return selectionNotEmpty;
         }
      }

      private static IBusinessRule selectionNotEmpty
      {
         get
         {
            return CreateRule.For<CompoundParameterSelectionDTO>()
               .Property(item => item.SelectedAlternative)
               .WithRule((param, value) => value != null)
               .WithError((param, value) => PKSimConstants.Error.CompoundParameterSelectionNeededFor(param.ParameterName));
         }
      }
   }
}