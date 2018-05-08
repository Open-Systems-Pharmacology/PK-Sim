using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Core.RunOptions
{
   public class JsonRunOptions : Notifier, IValidatable, IWithInputAndOutputFolders
   {
      private string _outputFolder;
      private string _inputFolder;
      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();
      public SimulationExportMode ExportMode { get; set; }

      public JsonRunOptions()
      {
         Rules.AddRange(new[]
         {
            RunOptionsRules.InputFolderDefined,
            RunOptionsRules.OutputFolderDefined,
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
         set => SetProperty(ref _outputFolder, value);
      }
   }
}