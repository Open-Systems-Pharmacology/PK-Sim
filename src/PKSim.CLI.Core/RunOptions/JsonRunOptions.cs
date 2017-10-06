using OSPSuite.Core.Domain;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.CLI.Core.Services;
using PKSim.Core;

namespace PKSim.CLI.Core.RunOptions
{
   public class JsonRunOptions : Notifier, IValidatable, IWithInputAndOutputFolders
   {
      private string _outputFolder;
      private string _inputFolder;
      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();
      public BatchExportMode ExportMode { get; set; }
      public NotificationType NotificationType { get; set; } = NotificationType.Info | NotificationType.Error;
      public string LogFileFullPath { get; set; }

      public JsonRunOptions()
      {
         Rules.AddRange(new[]
         {
            RunOptionsRules.InputFolderDefined,
            RunOptionsRules.OutputFolderDefined,
            GenericRules.NonEmptyRule<JsonRunOptions>(x => x.LogFileFullPath)
         });
      }

      public string InputFolder
      {
         get => _inputFolder;
         set => SetProperty(ref _inputFolder, value);
      }

      public string OutputFolder
      {
         get => _outputFolder;
         set
         {
            SetProperty(ref _outputFolder, value);
            if (string.IsNullOrEmpty(LogFileFullPath))
            {
               LogFileFullPath = CoreConstants.DefaultBatchLogFullPath(OutputFolder);
            }
         }
      }
   }
}