using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class VersionChecker : IVersionChecker
   {
      private readonly IJsonSerializer _jsonSerializer;
      public string ProductName { get; set; }
      public string CurrentVersion { get; set; }
      public string VersionFileUrl { get; set; }
      public VersionInfo LatestVersion { get; private set; }

      public VersionChecker(IJsonSerializer jsonSerializer)
      {
         _jsonSerializer = jsonSerializer;
      }

      public async Task<bool> NewVersionIsAvailableAsync()
      {
         try
         {
            await retrieveLatestVersion();
            return newVersionIsAvailable();

         }
         catch (Exception)
         {
            return false;
         }
      }

      private async Task retrieveLatestVersion()
      {
         using (var wc = new WebClient())
         {
            var jsonContent = await wc.DownloadStringTaskAsync(VersionFileUrl);
            var versions = await _jsonSerializer.DeserializeAsArrayFromString<VersionInfo>(jsonContent);
            LatestVersion = retrieveVersionFrom(versions);
         }
      }

      private bool newVersionIsAvailable()
      {
         if (LatestVersion == null)
            return false;

         var curVersion = new Version(CurrentVersion);
         return curVersion.CompareTo(new Version(LatestVersion.Version)) < 0;
      }


      private VersionInfo retrieveVersionFrom(IEnumerable<VersionInfo> allVersionInfos)
      {
         var versionInfo = allVersionInfos.FindByName(ProductName);
         if (versionInfo == null)
            throw new OSPSuiteException($"{ProductName} node not available");

         return versionInfo;
      }
   }
}