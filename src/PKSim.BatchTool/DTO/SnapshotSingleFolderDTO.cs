using PKSim.CLI.Core.Services;

namespace PKSim.BatchTool.DTO
{
   public class SnapshotSingleFolderDTO : InputAndOutputFolderDTO
   {
      public SnapshotExportMode ExportMode { get; set; }
   }
}