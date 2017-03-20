using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class CannotDeleteDefaultParameterAlternativeException : PKSimException
   {
      public CannotDeleteDefaultParameterAlternativeException()
         : base(PKSimConstants.Error.CannotDeleteDefaultParameterAlternative)
      {
      }
   }
}