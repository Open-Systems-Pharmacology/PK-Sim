using System.Collections.Generic;
using System.Text;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Core.RunOptions
{
   public class SnapshotRunOptions : Notifier, IValidatable, IWithInputAndOutputFolders
   {
      private string _outputFolder;
      private string _inputFolder;
      public SnapshotExportMode ExportMode { get; set; } = SnapshotExportMode.Snapshot;
      public NotificationType NotificationType { get; set; } = NotificationType.Info | NotificationType.Error;

      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();

      public string InputFolder
      {
         get => _inputFolder;
         set => SetProperty(ref _inputFolder, value);
      }

      public string OutputFolder
      {
         get => _outputFolder;
         set => SetProperty(ref _outputFolder, value);
      }

      public IReadOnlyList<string> Folders { get; set; }

      public SnapshotRunOptions()
      {
         Folders = new List<string>();
         Rules.AddRange(new[]
         {
            RunOptionsRules.InputFolderDefined,
            RunOptionsRules.OutputFolderDefined,
         });
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         this.LogOption(sb);
         sb.AppendLine($"Export mode: {ExportMode}");
         return sb.ToString();
      }
   }
}