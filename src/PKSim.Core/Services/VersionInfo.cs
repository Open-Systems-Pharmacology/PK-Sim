namespace PKSim.Core.Services
{
   public class VersionInfo
   {
      public string Version { get; set; }
      public string Description { get; set; }

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