using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Individuals
{
   public class ExpressionProfileSelectionDTO : ValidatableDTO
   {
      private ExpressionProfile _expressionProfile;

      public ExpressionProfile ExpressionProfile
      {
         get => _expressionProfile;
         set => SetProperty(ref _expressionProfile, value);
      }

      public IReadOnlyList<string> AllExistingMolecules { get; set; } = new List<string>();

      public ExpressionProfileSelectionDTO()
      {
         Rules.AddRange(ExpressionProfileSelectionDTORules.All());
      }

      private static class ExpressionProfileSelectionDTORules
      {
         private static IBusinessRule moleculeNotInUsed { get; } = CreateRule.For<ExpressionProfileSelectionDTO>()
            .Property(item => item.ExpressionProfile)
            .WithRule((dto, expressionProfile) => expressionProfile == null || !dto.AllExistingMolecules.Contains(expressionProfile.MoleculeName))
            .WithError((dto, expressionProfile) => PKSimConstants.Error.NameAlreadyExistsInContainerType(expressionProfile?.MoleculeName, PKSimConstants.ObjectTypes.Individual));

         private static IBusinessRule buildingBlockNotNull { get; } = CreateRule.For<ExpressionProfileSelectionDTO>()
            .Property(item => item.ExpressionProfile)
            .WithRule((dto, block) => block != null)
            .WithError((dto, block) => PKSimConstants.Error.BuildingBlockNotDefined(PKSimConstants.ObjectTypes.ExpressionProfile));


         internal static IEnumerable<IBusinessRule> All()
         {
            yield return buildingBlockNotNull;
            yield return moleculeNotInUsed;
         }
      }
   }
}