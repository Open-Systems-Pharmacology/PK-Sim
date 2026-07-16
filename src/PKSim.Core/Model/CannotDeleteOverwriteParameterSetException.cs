using System.Collections.Generic;
using PKSim.Assets;

namespace PKSim.Core.Model;

public class CannotDeleteOverwriteParameterSetException : PKSimException
{
   public CannotDeleteOverwriteParameterSetException(string overwriteParameterSetName, string compoundName, IReadOnlyList<string> simulationNames)
      : base(PKSimConstants.Error.CannotDeleteOverwriteParameterSetUsedInSimulations(overwriteParameterSetName, compoundName, simulationNames))
   {
   }
}