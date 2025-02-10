using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core;
using static OSPSuite.Assets.Captions;

namespace PKSim.Infrastructure.Serialization
{
   public class InvalidProjectVersionException : PKSimException
   {
      public InvalidProjectVersionException(int projectVersion) :
         base(
            ProjectVersionCannotBeLoaded(
               projectVersion,
               ProjectVersions.OldestSupportedVersion.VersionDisplay, 
               ProjectVersions.OldestSupportedVersion.Version,
               ProjectVersions.Current.VersionDisplay, 
               ProjectVersions.Current.Version, 
               Constants.PRODUCT_SITE_DOWNLOAD))
      {
      }
   }

   public class InvalidProjectFileException : PKSimException
   {
      public InvalidProjectFileException() : base(PKSimConstants.Error.ProjectFileIsCorrupt(CoreConstants.PRODUCT_NAME_WITH_TRADEMARK))
      {
      }
   }

   public class InvalidBuildingBlockVersionException : PKSimException
   {
      public InvalidBuildingBlockVersionException(int version) : base(PKSimConstants.Error.BuildingBlockVersionIsTooOld(version))
      {
      }
   }
}