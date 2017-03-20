using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class BuildingBlockAlreadyExistsInProjectException : PKSimException
   {
      public BuildingBlockAlreadyExistsInProjectException(IPKSimBuildingBlock buildingBlock) :
         base(PKSimConstants.Error.BuildingBlockAlreadyExists(buildingBlock.BuildingBlockType.ToString(), buildingBlock.Name))
      {
      }
   }
}