using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class VersionChecker : IVersionChecker
   {
      public string ProductName { get; set; }
      public string CurrentVersion { get; set; }
      public string VersionFileUrl { get; set; }
      public VersionInfo LatestVersion { get; private set; }

      public Task<bool> NewVersionIsAvailableAsync()
      {
         return Task.Run(() =>
            {
               retrieveLatestVersion();
               return newVersionIsAvailable();
            });
      }

      public bool NewVersionIsAvailable()
      {
         if (LatestVersion != null)
            return newVersionIsAvailable();

         try
         {
            retrieveLatestVersion();
            return newVersionIsAvailable();
         }
         catch (Exception e)
         {
            throw new OSPSuiteException($"Could not retrieve version information for {ProductName} at this time.", e);
         }
      }

      private void retrieveLatestVersion()
      {
         using (var wc = new WebClient())
         {
            var contentItemXmlText = wc.DownloadString(VersionFileUrl);
            var doc = XDocument.Parse(contentItemXmlText);
            LatestVersion = retrieveVersionFrom(doc);
         }
      }

      private bool newVersionIsAvailable()
      {
         if (LatestVersion == null)
            return false;

         var curVersion = new Version(CurrentVersion);
         return curVersion.CompareTo(new Version(LatestVersion.Version)) < 0;
      }


      private VersionInfo retrieveVersionFrom(XDocument xDocument)
      {
         XElement applicationNode = xDocument.Descendants("application").FirstOrDefault(e => string.Equals(e.GetAttribute("name"), ProductName));
         if (applicationNode == null)
            throw new OSPSuiteException($"{ProductName} node not available");

         return new VersionInfo {Version = applicationNode.Element("version").Value};
      }
   }
}