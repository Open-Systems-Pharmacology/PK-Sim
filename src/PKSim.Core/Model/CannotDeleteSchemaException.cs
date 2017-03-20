using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class CannotDeleteSchemaException : PKSimException
   {
      public CannotDeleteSchemaException() : base(PKSimConstants.Error.CannotDeleteSchema)
      {
      }
   }
}