using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers;

public class SnapshotContext : SnapshotContext<PKSimProject>
{
   //This constructor should only be called when initiation the project load and the project is not available
   public SnapshotContext() : base(new PKSimProject(), ProjectVersions.Current)
   {
   }

   public SnapshotContext(PKSimProject project, int version) : base(project, version)
   {
   }

   public SnapshotContext(SnapshotContext baseContext) : base(baseContext.Project, baseContext.Version)
   {
   }

   /// <summary>
   /// Returns true if the format is V9 or earlier
   /// </summary>
   public bool IsV9FormatOrEarlier => Version <= ProjectVersions.V9;

   /// <summary>
   /// Returns true if the format is V10 or earlier
   /// </summary>
   public bool IsV10FormatOrEarlier => Version <= ProjectVersions.V10;


   /// <summary>
   /// Returns true if the format is V11 or earlier
   /// </summary>
   public bool IsV11FormatOrEarlier => Version <= ProjectVersions.V11;
}