using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class CannotDeleteParameterAlternativeException : PKSimException
   {
      public CannotDeleteParameterAlternativeException()
         : base(PKSimConstants.Error.CannotDeleteParameterAlternative)
      {
      }
   }
}