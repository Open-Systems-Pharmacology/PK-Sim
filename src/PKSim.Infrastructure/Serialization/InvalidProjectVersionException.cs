using PKSim.Assets;
using PKSim.Core;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Serialization
{
   public class InvalidProjectVersionException : PKSimException
   {
      public InvalidProjectVersionException(int projectVersion) : base(PKSimConstants.Error.ProjectVersionCannotBeLoaded(projectVersion, ProjectVersions.Current,
         Constants.PRODUCT_SITE_DOWNLOAD))
      {
      }
   }

   public class InvalidProjectFileException : PKSimException
   {
      public InvalidProjectFileException() : base(PKSimConstants.Error.ProjectFileIsCorrupt(CoreConstants.ProductNameWithTrademark))
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