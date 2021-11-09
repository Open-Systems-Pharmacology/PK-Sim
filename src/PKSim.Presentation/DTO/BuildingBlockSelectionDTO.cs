using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO
{
   public class BuildingBlockSelectionDTO : ValidatableDTO
   {
      public string BuildingBockType { get; set; }
      public bool AllowEmptySelection { get; set; }
      private bool _validateBuildingBlock;
      private IPKSimBuildingBlock _buildingBlock;

      public BuildingBlockSelectionDTO()
      {
         ValidateBuildingBlock = true;
         Rules.AddRange(AllRules.All());
      }

      public IPKSimBuildingBlock BuildingBlock
      {
         get => _buildingBlock;
         set => SetProperty(ref _buildingBlock, value);
      }

      public bool ValidateBuildingBlock
      {
         get => _validateBuildingBlock;
         set
         {
            _validateBuildingBlock = value;
            OnPropertyChanged(() => ValidateBuildingBlock);
            OnPropertyChanged(() => BuildingBlock);
         }
      }

      private static class AllRules
      {
         private static IBusinessRule buildingBlockNotNull { get; } = CreateRule.For<BuildingBlockSelectionDTO>()
            .Property(item => item.BuildingBlock)
            .WithRule((dto, block) => !dto.ValidateBuildingBlock || block != null)
            .WithError((dto, block) => PKSimConstants.Error.BuildingBlockNotDefined(dto.BuildingBockType));

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return buildingBlockNotNull;
         }
      }
   }
}