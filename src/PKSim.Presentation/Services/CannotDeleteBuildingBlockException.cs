using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using OSPSuite.Assets;
using OSPSuite.Utility.Exceptions;

namespace PKSim.Presentation.Services
{
   public class CannotDeleteBuildingBlockException : OSPSuiteException
   {
      public CannotDeleteBuildingBlockException(string buildingBlockType, string buildingBlockName, IReadOnlyList<IPKSimBuildingBlock> buildingBlocksUsingBuildingBlockToDelete)
      {
         Message = Error.CannotDeleteBuildingBlockUsedBy(buildingBlockName, buildingBlockType,
            buildingBlocksUsingBuildingBlockToDelete.Select(CannotDeleteBuildingBlockException.buildingBlockName).ToList());
      }

      private static string buildingBlockName(IPKSimBuildingBlock x)
      {
         return $"{x.BuildingBlockType.ToString().ToLowerInvariant()} '{x.Name}'";
      }

      public override string Message { get; }
   }
}