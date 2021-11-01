using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class NoTemplateAvailableException : PKSimException
   {
      public NoTemplateAvailableException(string buildingBlockType) : base(PKSimConstants.Error.NoTemplateAvailableForType(buildingBlockType))
      {
      }
   }
}