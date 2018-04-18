using OSPSuite.Utility.Collections;
using OSPSuite.Core;

namespace PKSim.Core
{
   public static class ProjectVersions
   {
      private static readonly Cache<int, ProjectVersion> _knownVersions = new Cache<int, ProjectVersion>(x=>x.Version);

      public static readonly ProjectVersion V5_0_1 = addVersion(20, "5.0.1");
      public static readonly ProjectVersion V5_1_3 = addVersion(21, "5.1.3");
      public static readonly ProjectVersion V5_1_4 = addVersion(22, "5.1.4");
      public static readonly ProjectVersion V5_1_5 = addVersion(51, "5.1.5");
      public static readonly ProjectVersion V5_2_1 = addVersion(53, "5.2.1");
      public static readonly ProjectVersion V5_2_2 = addVersion(55, "5.2.2");
      public static readonly ProjectVersion V5_3_1 = addVersion(57, "5.3.1");
      public static readonly ProjectVersion V5_3_2 = addVersion(58, "5.3.2");
      public static readonly ProjectVersion V5_4_1 = addVersion(59, "5.4.1");
      public static readonly ProjectVersion V5_5_1 = addVersion(60, "5.5.1");
      public static readonly ProjectVersion V5_5_2 = addVersion(61, "5.5.2");
      public static readonly ProjectVersion V5_6_1 = addVersion(62, "5.6.1");
      public static readonly ProjectVersion V5_6_2 = addVersion(63, "5.6.2");
      public static readonly ProjectVersion V6_0_1 = addVersion(64, "6.0.1");
      public static readonly ProjectVersion V6_0_2 = addVersion(65, "6.0.2");
      public static readonly ProjectVersion V6_1_2 = addVersion(66, "6.1.2");
      public static readonly ProjectVersion V6_2_1 = addVersion(67, "6.2.1");
      public static readonly ProjectVersion V6_3_1 = addVersion(68, "6.3.1");
      public static readonly ProjectVersion V6_3_2 = addVersion(69, "6.3.2");
      public static readonly ProjectVersion V6_4_1 = addVersion(70, "6.4.1");
      public static readonly ProjectVersion V7_1_0 = addVersion(71, "7.1.0");
      public static readonly ProjectVersion V7_2_0 = addVersion(72, "7.2.0");
      public static readonly ProjectVersion V7_2_1 = addVersion(73, "7.2.1");
      public static readonly ProjectVersion V7_3_0 = addVersion(74, "7.3.0");
      public static readonly ProjectVersion Current = V7_3_0;

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
         
      public static ProjectVersion FindBy(int version)
      {
         return _knownVersions[version];
      }
   }
}