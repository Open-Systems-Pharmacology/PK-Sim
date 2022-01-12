using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class VersionChecker : IVersionChecker
   {
      private readonly IJsonSerializer _jsonSerializer;
      private IReadOnlyList<VersionInfo> _allVersions = new List<VersionInfo>();
      public string ProductName { get; set; }
      public string CurrentVersion { get; set; }
      public string VersionFileUrl { get; set; }

      public VersionChecker(IJsonSerializer jsonSerializer)
      {
         _jsonSerializer = jsonSerializer;
      }

      public VersionInfo LatestVersion => LatestVersionFor(ProductName);

      public VersionInfo LatestVersionFor(string productName) => _allVersions.FindByName(productName);

      public async Task DownloadLatestVersionInfoAsync()
      {
         try
         {
            using (var wc = new WebClient())
            {
               var jsonContent = await wc.DownloadStringTaskAsync(VersionFileUrl);
               _allVersions = await _jsonSerializer.DeserializeAsArrayFromString<VersionInfo>(jsonContent);
            }
         }
         catch (Exception)
         {
            //we do nothing if we cannot download the file
         }
      }

      public bool NewVersionIsAvailable => NewVersionIsAvailableFor(ProductName, CurrentVersion);

      public bool NewVersionIsAvailableFor(string productName, string currentVersion)
      {
         var latestVersion = LatestVersionFor(productName);
         if (latestVersion == null)
            return false;

         var curVersion = new Version(currentVersion);
         return curVersion.CompareTo(new Version(latestVersion.Version)) < 0;
      }
   }
}