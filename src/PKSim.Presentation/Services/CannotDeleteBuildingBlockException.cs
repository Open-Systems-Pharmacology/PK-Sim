using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using OSPSuite.Assets;
using OSPSuite.Utility.Exceptions;

namespace PKSim.Presentation.Services
{
   public class CannotDeleteBuildingBlockException : OSPSuiteException
   {
      public CannotDeleteBuildingBlockException(string buildingBlockType, string buildingBlockName, IReadOnlyList<Simulation> simulationsUsingBuildingBlock)
      {
         Message = Error.CannotDeleteBuildingBlockUsedBy(buildingBlockName, buildingBlockType,
            simulationsUsingBuildingBlock.Select(simulationNamed).ToList());
      }

      private static string simulationNamed(Simulation x)
      {
         return $"{ObjectTypes.Simulation.ToLowerInvariant()} '{x.Name}'";
      }

      public override string Message { get; }
   }
}