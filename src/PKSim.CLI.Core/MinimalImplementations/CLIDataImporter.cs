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

      public (IReadOnlyList<DataRepository> DataRepositories, ImporterConfiguration Configuration) ImportDataSets(IReadOnlyList<MetaDataCategory> metaDataCategories, IReadOnlyList<ColumnInfo> columnInfos,
         DataImporterSettings dataImporterSettings, string moleculeName, string molWeightName)
      {
         return (Enumerable.Empty<DataRepository>().ToList(), null);
      }

      public IReadOnlyList<DataRepository> ImportFromConfiguration(ImporterConfiguration configuration, bool promptForConfirmation, IReadOnlyList<MetaDataCategory> metaDataCategories,
         IReadOnlyList<ColumnInfo> columnInfos, DataImporterSettings dataImporterSettings, string moleculeName, string molWeightName)
      {
         return Enumerable.Empty<DataRepository>().ToList();
      }
   }
}