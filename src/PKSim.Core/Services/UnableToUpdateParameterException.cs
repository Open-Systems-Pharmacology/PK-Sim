using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class UnableToUpdateParameterException : PKSimException
   {
      public UnableToUpdateParameterException(string parameterPath, string simulationName)
         : base(PKSimConstants.Error.UnableToUpdateParameterException(parameterPath, simulationName))
      {
      }
   }
}