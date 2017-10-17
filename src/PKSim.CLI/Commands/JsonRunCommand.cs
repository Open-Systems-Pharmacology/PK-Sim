using System.Text;
using CommandLine;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Commands
{
    [Verb("run", HelpText = "Start batch run by loading a set of batch json files, running all simulations and exporting the results")]
    public class JsonRunCommand : RunCommand<JsonRunOptions>, IWithInputAndOutputFolders
    {
        public BatchExportMode ExportMode { get; set; }

        [Option('i', "input", Required = true, HelpText = "Input folder containing all batch json files to run")]
        public string InputFolder { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output folder where calculated values will be exported")]
        public string OutputFolder { get; set; }

        [Option('c', "csv", HelpText = "Export simulation outputs to csv")]
        public bool ExportCsv
        {
            set
            {
                if (value)
                    ExportMode = ExportMode | BatchExportMode.Csv;
            }
        }

        [Option('x', "xml", HelpText = "Export the simulation model xml")]
        public bool ExportXml
        {
            set
            {
                if (value)
                    ExportMode = ExportMode | BatchExportMode.Xml;
            }
        }

        [Option('j', "json", HelpText = "Export simulation outputs to json")]
        public bool ExportJson
        {
            set
            {
                if (value)
                    ExportMode = ExportMode | BatchExportMode.Json;
            }
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

        public override JsonRunOptions ToRunOptions()
        {
            return new JsonRunOptions
            {
                InputFolder = InputFolder,
                OutputFolder = OutputFolder,
                ExportMode = ExportMode,
            };
        }
    }
}