using OSPSuite.Core.Domain;
using OSPSuite.Utility.Validation;
using static PKSim.Assets.PKSimConstants.Error;

namespace OSPSuite.Core.Qualification
{
   public class BuildingBlockSwap : IWithName
   {
      public PKSimBuildingBlockType Type { get; set; }
      public string Name { get; set; }
      public string SnapshotFile { get; set; }

      public void Deconstruct(out PKSimBuildingBlockType type, out string name, out string snapshotFile)
      {
         type = Type;
         name = Name;
         snapshotFile = SnapshotFile;
      }
   }

   public class SimulationPlot
   {
      public string Simulation { get; set; }
      public int SectionId { get; set; }
   }

   public class QualifcationConfiguration : IValidatable
   {
      /// <summary>
      /// Typically name of the project as referenced in the configuration fie
      /// </summary>
      public string ProjectId { get; set; }


      /// <summary>
      ///    Path of project snapshot file used for this qualificaiton run
      /// </summary>
      public string SnapshotFile { get; set; }

      /// <summary>
      ///    Output folder where project artefacts will be exported. It will be created if it does not exist
      /// </summary>
      public string OutputFolder { get; set; }

      /// <summary>
      ///    Folder were observed data will be exported
      /// </summary>
      public string ObservedDataFolder { get; set; }

      /// <summary>
      ///    Path of mapping file that will be created for the project.
      /// </summary>
      public string MappingFile { get; set; }

      /// <summary>
      ///    Path of configuration file that will be created as part of the qualificaton run
      /// </summary>
      public string ReportConfigurationFile { get; set; }

      public SimulationPlot[] SimulationPlots { get; set; }

      public BuildingBlockSwap[] BuildingBlocks { get; set; }

      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();

      public QualifcationConfiguration()
      {
         Rules.AddRange(new[]
         {
            GenericRules.FileExists<QualifcationConfiguration>(x => x.SnapshotFile),
            GenericRules.NonEmptyRule<QualifcationConfiguration>(x => x.OutputFolder, QualificationOutputFolderNotDefined),
            GenericRules.NonEmptyRule<QualifcationConfiguration>(x => x.MappingFile, QualificationMappingFileNotDefined),
            GenericRules.NonEmptyRule<QualifcationConfiguration>(x => x.ReportConfigurationFile, QualificationReportConfigurationFileNotDefined),
            GenericRules.NonEmptyRule<QualifcationConfiguration>(x => x.ObservedDataFolder, QualificationObservedDataFolderNotDefined)
         });
      }
   }
}