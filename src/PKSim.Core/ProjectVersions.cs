using OSPSuite.Core;
using OSPSuite.Utility.Collections;

namespace PKSim.Core
{
   public static class ProjectVersions
   {
      private static readonly Cache<int, ProjectVersion> _knownVersions = new Cache<int, ProjectVersion>(x => x.Version, x => null);

      public const int UNSUPPORTED = 70;
      public static readonly ProjectVersion V7_1_0 = addVersion(71, "7.1.0");
      public static readonly ProjectVersion V7_2_0 = addVersion(72, "7.2.0");
      public static readonly ProjectVersion V7_2_1 = addVersion(73, "7.2.1");
      public static readonly ProjectVersion V7_3_0 = addVersion(74, "7.3.0");
      public static readonly ProjectVersion V7_4_0 = addVersion(75, "7.4.0");
      public static readonly ProjectVersion V8 = addVersion(76, "8");
      public static readonly ProjectVersion V9 = addVersion(77, "9");
      public static readonly ProjectVersion V10 = addVersion(78, "10");
      public static readonly ProjectVersion V11 = addVersion(79, "11");
      public static readonly ProjectVersion V12 = addVersion(80, "12");
      public static readonly ProjectVersion Current = V12;

      private static ProjectVersion addVersion(int versionNumber, string versionDisplay)
      {
         var projectVersion = new ProjectVersion(versionNumber, versionDisplay);
         _knownVersions.Add(projectVersion);
         return projectVersion;
      }

      public static string CurrentAsString => Current.VersionAsString;

      public static bool CanLoadVersion(int projectVersion)
      {
         return (projectVersion <= Current.Version) && _knownVersions.Contains(projectVersion);
      }

      public static bool ProjectIsTooOld(int projectVersion)
      {
         return projectVersion <= UNSUPPORTED;
      }

      public static ProjectVersion FindBy(int version)
      {
         return _knownVersions[version];
      }
   }
}