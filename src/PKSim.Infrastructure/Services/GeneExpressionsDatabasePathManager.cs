using System;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Core;

namespace PKSim.Infrastructure.Services
{
   public class GeneExpressionsDatabasePathManager : IGeneExpressionsDatabasePathManager
   {
      private readonly IGeneExpressionDatabase _database;
      private readonly IApplicationSettings _applicationSettings;
      private readonly IGeneExpressionQueries _geneExpressionQueries;

      public GeneExpressionsDatabasePathManager(IGeneExpressionDatabase database, IApplicationSettings applicationSettings, IGeneExpressionQueries geneExpressionQueries)
      {
         _database = database;
         _applicationSettings = applicationSettings;
         _geneExpressionQueries = geneExpressionQueries;
      }

      public bool HasDatabaseFor(Species species)
      {
         return HasDatabaseFor(species.Name);
      }
      
      public bool HasDatabaseFor(string speciesName)
      {
         if (!_applicationSettings.HasExpressionsDatabaseFor(speciesName))
            return false;

         return FileHelper.FileExists(databaseFor(speciesName));
      }

      public IDisposable ConnectToDatabaseFor(string speciesName)
      {
         _geneExpressionQueries.ClearCache();
         return new DatabaseDisposer(_database, databaseFor(speciesName));

      }

      public IDisposable ConnectToDatabaseFor(Species species)
      {
         return ConnectToDatabaseFor(species.Name);
      }

      private string databaseFor(string speciesName)
      {
         return _applicationSettings.SpeciesDatabaseMapsFor(speciesName).DatabaseFullPath;
      }
   }
}