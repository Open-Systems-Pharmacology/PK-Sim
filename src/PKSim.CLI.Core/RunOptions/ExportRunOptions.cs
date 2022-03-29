using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Core.RunOptions
{
   public class ExportRunOptions : Notifier, IValidatable, IWithOutputFolder
   {
      private string _outputFolder;

      public string ProjectFile { get; set; }
      public SimulationExportMode ExportMode { get; set; }
      public IEnumerable<string> Simulations { get; set; }
      public bool RunSimulation { get; set; }

      /// <summary>
      /// Specifies if all simulations should be exported if the provided list of Simulations is empty.
      /// Default is true and will be overwritten for special use cases
      /// </summary>
      public bool ExportAllSimulationsIfListIsEmpty { get; set; } = true;

      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();

      public string OutputFolder
      {
         get => _outputFolder;
         set => SetProperty(ref _outputFolder, value);
      }

      public ExportRunOptions()
      {
         Rules.AddRange(new[]
         {
            RunOptionsRules.OutputFolderDefined,
            GenericRules.FileExists<ExportRunOptions>(x => x.ProjectFile)
         });
      }
   }
}