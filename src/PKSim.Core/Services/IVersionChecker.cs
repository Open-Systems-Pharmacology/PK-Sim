using System.Threading.Tasks;

namespace PKSim.Core.Services
{
   public interface IVersionChecker
   {
      /// <summary>
      ///    Name of the product as registered in the version file
      /// </summary>
      string ProductName { get; set; }

      /// <summary>
      ///    Current version of the product in format x.y.z
      /// </summary>
      string CurrentVersion { get; set; }

      /// <summary>
      ///    Url of the version file
      /// </summary>
      string VersionFileUrl { get; set; }

      /// <summary>
      ///    Returns the latest version available for the registered product
      /// </summary>
      VersionInfo LatestVersion { get; }

      /// <summary>
      ///    Returns true if a new version was found otherwise false. The latest version can be retrieved from LatestVersion
      /// </summary>
      Task<bool> NewVersionIsAvailableAsync();
   }
}