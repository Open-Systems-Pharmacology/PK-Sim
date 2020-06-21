using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core;

namespace PKSim.Infrastructure.Serialization
{
   public class InvalidProjectVersionException : PKSimException
   {
      public InvalidProjectVersionException(int projectVersion) :
         base(PKSimConstants.Error.ProjectVersionCannotBeLoaded(projectVersion, ProjectVersions.Current,
            ProjectVersions.ProjectIsTooOld(projectVersion), Constants.PRODUCT_SITE_DOWNLOAD))
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