using System.Collections.Generic;
using System.IO;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Core;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_PKSimConfiguration : ContextSpecification<IPKSimConfiguration>
   {
      protected override void Context()
      {
         sut = new PKSimConfiguration();
      }
   }

   public class When_retrieving_the_full_version_of_the_assembly : concern_for_PKSimConfiguration
   {
      [Observation]
      public void should_return_a_string_containing_the_revison_number()
      {
         sut.FullVersion.Contains("Build").ShouldBeTrue();
      }
   }

   public class When_retrieving_the_list_of_all_application_paths_available : concern_for_PKSimConfiguration
   {
      private List<string> _possibleApplicationPaths;

      protected override void Because()
      {
         _possibleApplicationPaths = sut.ApplicationSettingsFilePaths.ToList();
      }

      [Observation]
      public void should_return_the_one_for_the_current_version_first_and_the_latest_version_with_a_small_major_last()
      {
         _possibleApplicationPaths[0].ShouldBeEqualTo(sut.ApplicationSettingsFilePath);
         _possibleApplicationPaths.Last().StartsWith(Path.Combine(EnvironmentHelper.ApplicationDataFolder(), CoreConstants.ApplicationFolderPath, "5.6")).ShouldBeTrue();
      }
   }
}