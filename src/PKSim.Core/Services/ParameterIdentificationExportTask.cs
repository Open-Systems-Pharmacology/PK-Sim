using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Core.Services
{
   public interface IParameterIdentificationExportTask
   {
      Task ExportParameterIdentification(ParameterIdentification parameterIdentification, string exportDirectory);
   }

   public class ParameterIdentificationExportTask : IParameterIdentificationExportTask
   {
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IMoBiExportTask _exportTask;
      private readonly ISnapshotTask _snapshotTask;
      private readonly IPKMLPersistor _pkmlPersistor;

      public ParameterIdentificationExportTask(
         ILazyLoadTask lazyLoadTask,
         IMoBiExportTask exportTask,
         ISnapshotTask snapshotTask,
         IPKMLPersistor pkmlPersistor)
      {
         _lazyLoadTask = lazyLoadTask;
         _exportTask = exportTask;
         _snapshotTask = snapshotTask;
         _pkmlPersistor = pkmlPersistor;
      }

      public async Task ExportParameterIdentification(ParameterIdentification parameterIdentification, string exportDirectory)
      {
         _lazyLoadTask.Load(parameterIdentification);
         var simulations = parameterIdentification.AllSimulations.Cast<Simulation>().ToList();
         var observedDataList = parameterIdentification.AllObservedData.ToList();

         var observedDataFolder = Path.Combine(exportDirectory, "Data");
         DirectoryHelper.CreateDirectory(observedDataFolder);

         var simulationFolder = Path.Combine(exportDirectory, "Simulations");
         DirectoryHelper.CreateDirectory(simulationFolder);

         var tasks = simulations.Select(x => exportSimulation(x, simulationFolder)).ToList();
         tasks.Add(exportParameterIdentification(parameterIdentification, exportDirectory));
         tasks.AddRange(observedDataList.Select(x => exportUsedObservedData(x, observedDataFolder)));

         await Task.WhenAll(tasks);
      }

      private async Task exportUsedObservedData(DataRepository observedData, string exportDirectory)
      {
         var targetFile = fileWithExtensions(exportDirectory, observedData.Name, Constants.Filter.JSON_EXTENSION);
         await _snapshotTask.ExportModelToSnapshot(observedData, targetFile);
         targetFile = fileWithExtensions(exportDirectory, observedData.Name, Constants.Filter.PKML_EXTENSION);
         _pkmlPersistor.SaveToPKML(observedData, targetFile);
      }

      private async Task exportParameterIdentification(ParameterIdentification parameterIdentification, string exportDirectory)
      {
         var targetFile = fileWithExtensions(exportDirectory, parameterIdentification.Name, Constants.Filter.JSON_EXTENSION);
         await _snapshotTask.ExportModelToSnapshot(parameterIdentification, targetFile);
         targetFile = fileWithExtensions(exportDirectory, parameterIdentification.Name, Constants.Filter.PKML_EXTENSION);
         _pkmlPersistor.SaveToPKML(parameterIdentification, targetFile);
      }

      private Task exportSimulation(Simulation simulation, string exportDirectory)
      {
         var targetFile = fileWithExtensions(exportDirectory, simulation.Name, Constants.Filter.PKML_EXTENSION);
         return _exportTask.ExportSimulationToPkmlFileAsync(simulation, targetFile);
      }

      private string fileWithExtensions(string folder, string fileName, string extensions)
      {
         return Path.Combine(folder, $"{FileHelper.RemoveIllegalCharactersFrom(fileName)}{extensions}");
      }
   }
}