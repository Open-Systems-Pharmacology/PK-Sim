using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.CLI.Core.Services;
using PKSim.Core.Services;

namespace PKSim.CLI.Core.RunOptions
{
   public class JsonRunOptions : Notifier, IValidatable, IWithInputAndOutputFolders
   {
      private string _outputFolder;
      private string _inputFolder;
      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();
      public SimulationExportMode ExportMode { get; set; }

      /// <summary>
      ///    Indicates whether all outputs of the simulation should be exported or only the one actually selected in the
      ///    simulation.
      ///    Default value is <c>TRUE</c>
      /// </summary>
      public bool RunForAllOutputs { get; set; } = true;

      /// <summary>
      ///    How do we deal with Jacobian. As is to stay compatible with version prior to v11
      /// </summary>
      public JacobianUse JacobianUse { get; set; } = JacobianUse.AsIs;

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

      public void Deconstruct(out string inputFolder, out string outputFolder, out SimulationExportMode exportMode)
      {
         inputFolder = InputFolder;
         outputFolder = OutputFolder;
         exportMode = ExportMode;
      }
   }
}