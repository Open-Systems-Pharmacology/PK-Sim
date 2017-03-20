using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class CannotDeleteSchemaItemException : PKSimException
   {
      public CannotDeleteSchemaItemException() : base(PKSimConstants.Error.CannotDeleteSchemaItem)
      {
      }
   }
}