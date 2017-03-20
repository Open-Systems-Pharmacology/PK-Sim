namespace PKSim.Core.Services
{
   public class InvalidSimulationConfigurationException : PKSimException
   {
      public InvalidSimulationConfigurationException(string message) : base(message)
      {
      }
   }
}