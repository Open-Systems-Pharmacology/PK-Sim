using OSPSuite.Core.Domain;
using OSPSuite.Core.Snapshots.Mappers;

namespace PKSim.Core.Snapshots.Services
{
   public static class ParameterPathConverter
   {
      public const string TIME_TO_COMPLETE_GALLBLADDER_REFILLING = "Time to complete gallbladder refilling";
      public const string TIME_TO_COMPLETE_GALLBLADDER_EMPTYING = "Time to complete gallbladder emptying";

      /// <summary>
      ///    Converts parameter paths saved in snapshots of format V12 or earlier to the current parameter names
      /// </summary>
      public static string ConvertParameterPath(string parameterPath, SnapshotContext snapshotContext)
      {
         if (string.IsNullOrEmpty(parameterPath) || snapshotContext == null || !snapshotContext.IsV12FormatOrEarlier)
            return parameterPath;

         var refillingSuffix = $"{ObjectPath.PATH_DELIMITER}{TIME_TO_COMPLETE_GALLBLADDER_REFILLING}";
         if (parameterPath == TIME_TO_COMPLETE_GALLBLADDER_REFILLING)
            return TIME_TO_COMPLETE_GALLBLADDER_EMPTYING;

         if (!parameterPath.EndsWith(refillingSuffix))
            return parameterPath;

         return $"{parameterPath.Substring(0, parameterPath.Length - refillingSuffix.Length)}{ObjectPath.PATH_DELIMITER}{TIME_TO_COMPLETE_GALLBLADDER_EMPTYING}";
      }
   }
}
