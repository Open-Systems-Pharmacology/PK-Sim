using PKSim.CLI.Core.Services;

namespace PKSim.BatchTool.DTO
{
   public class SnapshotSingleFolderDTO : InputAndOutputFolderDTO
   {
      public string LogFileFullPath { get; set; }
      public SnapshotExportMode ExportMode { get; set; }
   }
}