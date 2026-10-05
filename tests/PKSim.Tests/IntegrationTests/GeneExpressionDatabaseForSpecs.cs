using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using PKSim.Core;

namespace PKSim.IntegrationTests
{
   public static class GeneExpressionDatabaseForSpecs
   {
      public const string RELEASE = "v3.0.1";
      private const string RELEASES_URL = "https://github.com/Open-Systems-Pharmacology/Gene-Expression-Databases/releases";
      private const string CAT_DATABASE = "GENEDB_cat_TPM_ONLY_BgeeRelease_15_0_2023-08-30";

      private static readonly Lazy<string> _catDatabasePath = new Lazy<string>(downloadCatDatabase);

      /// <summary>
      ///    Path of the cat database of <see cref="RELEASE" />. It is downloaded once and kept in the temp folder for later runs.
      /// </summary>
      public static string CatDatabasePath => _catDatabasePath.Value;

      public static string LatestRelease()
      {
         using var client = createClient(allowAutoRedirect: false);
         using var response = client.GetAsync($"{RELEASES_URL}/latest").GetAwaiter().GetResult();
         var latestReleaseUrl = response.Headers.Location ?? throw new HttpRequestException($"{RELEASES_URL}/latest answered {(int) response.StatusCode} instead of redirecting to the latest release.");
         return latestReleaseUrl.Segments.Last();
      }

      private static string downloadCatDatabase()
      {
         var folder = Path.Combine(Path.GetTempPath(), "PKSim.Tests", "GeneExpressionDatabases", RELEASE);
         var databasePath = Path.Combine(folder, $"{CAT_DATABASE}{CoreConstants.Filter.GENE_DB_EXTENSION}");
         if (File.Exists(databasePath))
            return databasePath;

         Directory.CreateDirectory(folder);
         var zipPath = Path.Combine(folder, Path.GetRandomFileName());
         var extractedPath = Path.Combine(folder, Path.GetRandomFileName());
         try
         {
            using (var client = createClient(allowAutoRedirect: true))
               File.WriteAllBytes(zipPath, client.GetByteArrayAsync($"{RELEASES_URL}/download/{RELEASE}/{CAT_DATABASE}.zip").GetAwaiter().GetResult());

            using (var zip = ZipFile.OpenRead(zipPath))
               zip.Entries.Single().ExtractToFile(extractedPath);

            File.Move(extractedPath, databasePath);
         }
         catch (IOException) when (File.Exists(databasePath))
         {
         }
         finally
         {
            File.Delete(zipPath);
            File.Delete(extractedPath);
         }

         return databasePath;
      }

      private static HttpClient createClient(bool allowAutoRedirect)
      {
         var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = allowAutoRedirect }) { Timeout = TimeSpan.FromMinutes(5) };
         client.DefaultRequestHeaders.UserAgent.ParseAdd("PKSim.Tests");
         return client;
      }
   }
}
