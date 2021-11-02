using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class NoTemplateAvailableException : PKSimException
   {
      public NoTemplateAvailableException(string templateType) : base(PKSimConstants.Error.NoTemplateAvailableForType(templateType))
      {
      }
   }
}