using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class NoBuildingBlockTemplateAvailableException : PKSimException
   {
      public NoBuildingBlockTemplateAvailableException(string buildingBlockType) : base(PKSimConstants.Error.NoTemplateBuildingBlockAvailableForType(buildingBlockType))
      {
      }
   }
}