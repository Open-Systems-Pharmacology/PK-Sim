using System.Text;
using CommandLine;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Commands
{
    [Verb("snap", HelpText = "Start snapshot workflows by loading a set of project (or snapshot) files and creating the corresponding snapshot (or project) file automatically")]
    public class SnapshotRunCommand : RunCommand<SnapshotRunOptions>, IWithInputAndOutputFolders
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            this.LogOption(sb);
            sb.AppendLine($"Log file: {LogFileFullPath}");
            sb.AppendLine($"Log level: {LogLevel}");
            sb.AppendLine($"Export mode: {ExportMode}");
            return sb.ToString();
        }

        public override SnapshotRunOptions ToRunOptions()
        {
            return new SnapshotRunOptions
            {
                OutputFolder = OutputFolder,
                InputFolder = InputFolder,
                ExportMode = ExportMode
            };
        }
    }
}