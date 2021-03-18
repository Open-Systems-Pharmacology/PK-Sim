using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Infrastructure.Import.Services;
using ImporterConfiguration = OSPSuite.Core.Import.ImporterConfiguration;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIDataImporter : IDataImporter
   {
      public IEnumerable<DataRepository> ImportDataSets(IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos, DataImporterSettings dataImporterSettings)
      {
         return Enumerable.Empty<DataRepository>();
      }

      (IReadOnlyList<DataRepository> DataRepositories, ImporterConfiguration Configuration) IDataImporter.ImportDataSets(IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos,
         DataImporterSettings dataImporterSettings)
      {
         return (Enumerable.Empty<DataRepository>().ToList(), null);
      }

      public IReadOnlyList<DataRepository> ImportFromConfiguration(ImporterConfiguration configuration, IReadOnlyList<MetaDataCategory> metaDataCategories,
         IReadOnlyList<ColumnInfo> columnInfos, DataImporterSettings dataImporterSettings)
      {
         return Enumerable.Empty<DataRepository>().ToList();
      }

      public (IEnumerable<DataRepository> newDataSets, IEnumerable<DataRepository> overwrittenDataSets, IEnumerable<DataRepository> dataSetsToBeDeleted) ReloadFromConfiguration(IEnumerable<DataRepository> dataSetsToImport,
         IEnumerable<DataRepository> existingDataSets)
      {
         return (Enumerable.Empty<DataRepository>(), Enumerable.Empty<DataRepository>(), Enumerable.Empty<DataRepository>());
      }
   }
}