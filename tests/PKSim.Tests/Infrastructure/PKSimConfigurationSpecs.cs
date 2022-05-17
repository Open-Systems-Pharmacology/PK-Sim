using System;
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

   public class When_the_pk_sim_configuration_is_being_instantiated : concern_for_PKSimConfiguration
   {
      [Observation]
      public void should_return_the_expected_path_for_the_user_database_template()
      {
         sut.TemplateUserDatabaseTemplatePath.Contains(CoreConstants.TEMPLATE_USER_DATABASE_TEMPLATE).ShouldBeTrue();
      }
   }

   public class When_retrieving_the_full_version_of_the_assembly : concern_for_PKSimConfiguration
   {
      [Observation]
      public void should_return_a_string_containing_the_revision_number_or_the_info_number()
      {
         sut.FullVersion.StartsWith($"{sut.Version}").ShouldBeTrue();
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
         _possibleApplicationPaths.Last().StartsWith(Path.Combine(EnvironmentHelper.ApplicationDataFolder(), CoreConstants.APPLICATION_FOLDER_PATH, "8.0")).ShouldBeTrue();
      }
   }

   public class When_retrieving_the_path_of_the_pksim_database : concern_for_PKSimConfiguration
   {
      [Observation]
      public void should_return_the_path_in_the_application_folder_if_the_file_exists_in_the_application_folder()
      {
         doWhilePreservingFileExists(() =>
         {
            var appDataFile = Path.Combine(sut.AllUsersFolderPath, CoreConstants.PK_SIM_DB_FILE);
            FileHelper.FileExists = s => string.Equals(s, appDataFile);
            sut = new PKSimConfiguration();
            sut.PKSimDbPath.ShouldBeEqualTo(appDataFile);
         });
      }

      [Observation]
      public void should_return_the_local_folder_if_the_file_exists_locally()
      {
         doWhilePreservingFileExists(() =>
         {
            var localFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CoreConstants.PK_SIM_DB_FILE);
            FileHelper.FileExists = s => string.Equals(s, localFile);
            sut = new PKSimConfiguration();
            sut.PKSimDbPath.ShouldBeEqualTo(localFile);
         });
      }

      [Observation]
      public void should_return_the_path_in_local_folder_if_the_file_does_not_exists_in_the_application_folder_and_does_not_exists_locally()
      {
         doWhilePreservingFileExists(() =>
         {
            var localFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CoreConstants.PK_SIM_DB_FILE);
            FileHelper.FileExists = s => false;
            sut = new PKSimConfiguration();
            sut.PKSimDbPath.ShouldBeEqualTo(localFile);
         });
      }

      private void doWhilePreservingFileExists(Action action)
      {
         var oldFileExists = FileHelper.FileExists;
         try
         {
            action();
         }
         finally
         {
            FileHelper.FileExists = oldFileExists;
         }
      }
   }
}