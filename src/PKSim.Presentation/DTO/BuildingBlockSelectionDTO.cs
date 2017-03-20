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
         get { return _buildingBlock; }
         set
         {
            _buildingBlock = value;
            OnPropertyChanged(() => BuildingBlock);
         }
      }

      public bool ValidateBuildingBlock
      {
         get { return _validateBuildingBlock; }
         set
         {
            _validateBuildingBlock = value;
            OnPropertyChanged(() => ValidateBuildingBlock);
            OnPropertyChanged(() => BuildingBlock);
         }
      }

      private static class AllRules
      {
         private static IBusinessRule buildingBlockNotNull
         {
            get
            {
               return CreateRule.For<BuildingBlockSelectionDTO>()
                  .Property(item => item.BuildingBlock)
                  .WithRule((dto, block) => !dto.ValidateBuildingBlock || block != null)
                  .WithError((dto, block) => PKSimConstants.Error.BuildingBlockNotDefined(dto.BuildingBockType));
            }
         }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return buildingBlockNotNull;
         }
      }
   }
}