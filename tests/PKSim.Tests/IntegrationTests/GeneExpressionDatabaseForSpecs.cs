using System;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using OSPSuite.Utility;
using PKSim.Core;

namespace PKSim.IntegrationTests
{
   public static class GeneExpressionDatabaseForSpecs
   {
      public const string RELEASE = "v3.0.1";
      private const string LATEST_RELEASE_URL = "https://github.com/Open-Systems-Pharmacology/Gene-Expression-Databases/releases/latest";
      private const string CAT_DATABASE = "GENEDB_cat_TPM_ONLY_BgeeRelease_15_0_2023-08-30";

      /// <summary>
      ///    Extracts the cat database of <see cref="RELEASE" /> from the test data into a new temporary file and returns its path.
      /// </summary>
      public static string ExtractCatDatabase()
      {
         var databasePath = FileHelper.GenerateTemporaryFileName();
         using var zip = ZipFile.OpenRead(DomainHelperForSpecs.DataFilePathFor($"{CAT_DATABASE}.zip"));
         zip.Entries.Single().ExtractToFile(databasePath);
         return databasePath;
      }

      public static string LatestRelease()
      {
         using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }) { Timeout = TimeSpan.FromMinutes(1) };
         client.DefaultRequestHeaders.UserAgent.ParseAdd("PKSim.Tests");
         using var response = client.GetAsync(LATEST_RELEASE_URL).GetAwaiter().GetResult();
         var latestReleaseUrl = response.Headers.Location ?? throw new HttpRequestException($"{LATEST_RELEASE_URL} answered {(int) response.StatusCode} instead of redirecting to the latest release.");
         return latestReleaseUrl.Segments.Last();
      }
   }
}
