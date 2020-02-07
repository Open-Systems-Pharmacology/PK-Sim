using System.IO;
using System.Linq;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPopulationSimulationExportTask
   {
      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to a folder containing all the necessary files to run the
      ///    matlab wrapper
      /// </summary>
      void ExportForCluster(PopulationSimulation populationSimulation, string fileName, string description = null);

      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to a folder containing all the necessary files to run the
      ///    matlab wrapper
      /// </summary>
      void ExportForCluster(PopulationSimulation populationSimulation, FileSelection fileSelection);
   }

   public class PopulationSimulationExportTask : IPopulationSimulationExportTask
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly ICloner _cloner;
      private readonly IDialogCreator _dialogCreator;
      private readonly IMoBiExportTask _mobiExportTask;
      private readonly IPopulationExportTask _populationExportTask;

      public PopulationSimulationExportTask(
         ILazyLoadTask lazyLoadTask,
         ISimulationSettingsRetriever simulationSettingsRetriever,
         ICloner cloner,
         IDialogCreator dialogCreator,
         IMoBiExportTask mobiExportTask,
         IPopulationExportTask populationExportTask)
      {
         _lazyLoadTask = lazyLoadTask;
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _cloner = cloner;
         _dialogCreator = dialogCreator;
         _mobiExportTask = mobiExportTask;
         _populationExportTask = populationExportTask;
      }

      public void ExportForCluster(PopulationSimulation populationSimulation, FileSelection populationExport)
      {
         if (populationExport == null)
            return;
         ExportForCluster(populationSimulation, populationExport.FilePath, populationExport.Description);
      }

      public void ExportForCluster(PopulationSimulation populationSimulation, string populationFolder, string fileDescription = null)
      {
         if (string.IsNullOrEmpty(populationFolder))
            return;

         _lazyLoadTask.Load(populationSimulation);

         if (settingsRequired(populationSimulation))
         {
            var outputSelections = _simulationSettingsRetriever.SettingsFor(populationSimulation);
            if (outputSelections == null)
               return;

            populationSimulation.OutputSelections.UpdatePropertiesFrom(outputSelections, _cloner);
         }

         var existingFiles = Directory.GetFiles(populationFolder);
         if (existingFiles.Any())
         {
            if (_dialogCreator.MessageBoxYesNo(PKSimConstants.UI.DeleteFilesIn(populationFolder)).Equals(ViewResult.No))
               return;

            existingFiles.Each(FileHelper.DeleteFile);
         }

         var fileName = populationSimulation.Name;
         var modelFileFullPath = Path.Combine(populationFolder, $"{fileName}.pkml");
         var agingFileFullPath = Path.Combine(populationFolder, $"{fileName}{CoreConstants.Population.TABLE_PARAMETER_EXPORT}.csv");

         //Model
         _mobiExportTask.ExportSimulationToPkmlFile(populationSimulation, modelFileFullPath);

         var comments = _populationExportTask.CreateProjectMetaInfoFrom(fileDescription);

         //all values
         var dataTable = _populationExportTask.CreatePopulationDataFor(populationSimulation);
         dataTable.ExportToCSV(Path.Combine(populationFolder, $"{fileName}.csv"), comments: comments);

         //all aging data
         var agingData = populationSimulation.AgingData.ToDataTable();
         if (agingData.Rows.Count > 0)
            agingData.ExportToCSV(agingFileFullPath, comments: comments);
      }
      
      private bool settingsRequired(Simulation simulation)
      {
         if (simulation.OutputSelections == null)
            return true;

         return !simulation.OutputSelections.HasSelection;
      }
   }
}