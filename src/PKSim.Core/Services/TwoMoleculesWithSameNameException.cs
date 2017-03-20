using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class TwoMoleculesWithSameNameException : PKSimException
   {
      public TwoMoleculesWithSameNameException(string duplicateName)
         : base(PKSimConstants.Error.UnableToCreateSimulationWithMoleculesHavingSameName(duplicateName))
      {
      }
   }
}