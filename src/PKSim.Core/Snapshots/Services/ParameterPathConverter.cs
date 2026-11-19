using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots.Mappers;

namespace PKSim.Core.Snapshots.Services
{
   public static class ParameterPathConverter
   {
      /// <summary>
      ///    Converts parameter paths saved in snapshots of format V12 or earlier to the current parameter names
      /// </summary>
      public static string ConvertParameterPath(string parameterPath, SnapshotContext snapshotContext)
      {
         if (string.IsNullOrEmpty(parameterPath) || snapshotContext == null || !snapshotContext.IsV12FormatOrEarlier)
            return parameterPath;

         var objectPath = parameterPath.ToObjectPath();
         ParameterMapper.RenameParametersForV13(objectPath);
         return objectPath.ToString();
      }
   }
}
