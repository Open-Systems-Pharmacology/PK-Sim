using OSPSuite.Utility;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public static class PKSimBuildingBlockTypeExtensions
   {
      public static TemplateType AsTemplateType(this PKSimBuildingBlockType buildingBlockType)
      {
         return EnumHelper.ParseValue<TemplateType>(buildingBlockType.ToString());
      }
   }
}