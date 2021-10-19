using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class VersionInfo : IWithName
   {
      public string Version { get; set; }
      public string Name { get; set; }

      public static implicit operator string(VersionInfo versionInfo)
      {
         return versionInfo.ToString();
      }

      public override string ToString()
      {
         return Version;
      }
   }
}