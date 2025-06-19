using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers;

public static class SnapshotContextExtensions
{
   public static PKSimProject PKSimProject(this SnapshotContext context)
   {
      return context.Project as PKSimProject;
   }
}