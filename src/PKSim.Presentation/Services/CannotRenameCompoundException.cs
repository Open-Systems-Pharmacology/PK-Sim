using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Services
{
   public class CannotRenameCompoundException : PKSimException
   {
       private readonly string _errorMessage;

       public CannotRenameCompoundException(string compoundName, IList< PKSim.Core.Model.Simulation> simulationsUsingBuildingBlock)
        {
            if (simulationsUsingBuildingBlock.Count > 1)
               _errorMessage = PKSimConstants.Error.CannotRenameCompoundUsedInSimulations(compoundName,simulationsUsingBuildingBlock.Select(x => x.Name).ToString("\n", "'"));
            else
               _errorMessage = PKSimConstants.Error.CannotRenameCompoundUsedInSimulation(compoundName,simulationsUsingBuildingBlock.Select(x => x.Name).ToString("\n", "'"));
        }

        public override string Message
        {
            get { return _errorMessage; }
        }
    }
}