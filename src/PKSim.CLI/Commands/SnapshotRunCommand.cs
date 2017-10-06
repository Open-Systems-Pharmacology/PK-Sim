using System.Text;
using CommandLine;
using PKSim.CLI.Core;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Commands
{
   [Verb("snap", HelpText = "Start snapshot workflows by loading a set of project (or snapshot) files and creating the corresponding snapshot (or project) file automatically")]
   public class SnapshotRunCommand : IWithInputAndOutputFolders
   {
      public SnapshotExportMode ExportMode { get; set; } = SnapshotExportMode.Snapshot;

      [Option('i', "input", Required = true, HelpText = "Input folder containing all project or snapshot files to load")]
      public string InputFolder { get; set; }

      [Option('o', "output", Required = true, HelpText = "Output folder where project or snapshot files will be exported")]
      public string OutputFolder { get; set; }

      [Option('p', "project", HelpText = "Create project files from snapshot files")]
      public bool ExportProject
      {
         set => ExportMode = SnapshotExportMode.Project;
      }

      [Option('s', "snapshot", HelpText = "Create snaphot files from project files")]
      public bool ExportSnapshot
      {
         set => ExportMode = SnapshotExportMode.Snapshot;
      }

      [Option('l', "log", Required = false, HelpText = "Full path of log file where log output will be written. If not defined, it wil be exported in the output folder")]
      public string LogFileFullPath { get; set; }

      public override string ToString()
      {
         var sb = new StringBuilder();
         this.LogOption(sb);
         sb.AppendLine($"Log file: {LogFileFullPath}");
         sb.AppendLine($"Export mode: {ExportMode}");
         return sb.ToString();
      }

      public SnapshotRunOptions ToRunOptions()
      {
         return new SnapshotRunOptions
         {
            LogFileFullPath = LogFileFullPath,
            OutputFolder = OutputFolder,
            InputFolder = InputFolder,
            ExportMode = ExportMode
         };
      }
   }
}