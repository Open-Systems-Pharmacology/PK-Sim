using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;

namespace PKSim.Core
{
   public abstract class concern_for_TemplateDatabaseCreator : ContextSpecification<ITemplateDatabaseCreator>
   {
      protected IDialogCreator _dialogCreator;
      protected ICoreUserSettings _userSettings;
      protected IPKSimConfiguration _configuration;

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _userSettings = A.Fake<ICoreUserSettings>();
         _configuration = A.Fake<IPKSimConfiguration>();

         sut = new TemplateDatabaseCreator(_dialogCreator, _userSettings, _configuration);
      }
   }

   public class When_creating_the_default_template_for_the_current_user_and_the_user_template_does_not_exist : concern_for_TemplateDatabaseCreator
   {
      private Action<string, string> _oldCopy;
      private Func<string, bool> _oldFileExists;
      private bool _didPerformCopy;

      protected override void Context()
      {
         base.Context();
         //in this scenario, each call to the method should return false
         _oldFileExists = FileHelper.FileExists;
         _oldCopy = FileHelper.Copy;

         FileHelper.FileExists = s => false;

         FileHelper.Copy = copyAction;
         A.CallTo(() => _configuration.TemplateUserDatabaseTemplatePath).Returns(FileHelper.GenerateTemporaryFileName());
         A.CallTo(() => _configuration.DefaultTemplateUserDatabasePath).Returns(FileHelper.GenerateTemporaryFileName());
         A.CallTo(() => _userSettings.TemplateDatabasePath).Returns(_configuration.DefaultTemplateUserDatabasePath);
      }

      private void copyAction(string source, string target)
      {
         _didPerformCopy = string.Equals(source, _configuration.TemplateUserDatabaseTemplatePath) &&
                           string.Equals(target, _userSettings.TemplateDatabasePath);
      }

      protected override void Because()
      {
         sut.CreateDefaultTemplateDatabase();
      }

      [Observation]
      public void should_copy_the_template_database_to_the_user_profile()
      {
         _didPerformCopy.ShouldBeTrue();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         FileHelper.Copy = _oldCopy;
         FileHelper.FileExists = _oldFileExists;
      }
   }

   public class When_creating_the_default_template_for_the_current_user_and_the_user_template_exists_using_the_supported_file_format : concern_for_TemplateDatabaseCreator
   {
      private Action<string, string> _oldCopy;
      private Func<string, bool> _oldFileExists;
      private bool _didPerformCopy;

      protected override void Context()
      {
         base.Context();
         //in this scenario, each call to the method should return false
         _oldFileExists = FileHelper.FileExists;
         _oldCopy = FileHelper.Copy;

         _userSettings.TemplateDatabasePath = FileHelper.GenerateTemporaryFileName();
         FileHelper.FileExists = s => true;

         FileHelper.Copy = copyAction;
      }

      private void copyAction(string source, string target)
      {
         _didPerformCopy = true;
      }

      protected override void Because()
      {
         sut.CreateDefaultTemplateDatabase();
      }

      [Observation]
      public void should_not_copy_the_template_database_to_the_user_profile()
      {
         _didPerformCopy.ShouldBeFalse();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         FileHelper.Copy = _oldCopy;
         FileHelper.FileExists = _oldFileExists;
      }
   }

   public class When_creating_the_default_template_for_the_current_user_and_the_user_template_exists_using_the_old_file_format : concern_for_TemplateDatabaseCreator
   {
      private Action<string, string> _oldCopy;
      private Func<string, bool> _oldFileExists;
      private bool _didPerformCopy;
      private string _oldDbFile;

      protected override void Context()
      {
         base.Context();
         //in this scenario, each call to the method should return false
         _oldFileExists = FileHelper.FileExists;
         _oldCopy = FileHelper.Copy;
         _oldDbFile = $"{FileHelper.GenerateTemporaryFileName()}, {CoreConstants.Filter.MDB_EXTENSION}";
         _userSettings.TemplateDatabasePath = _oldDbFile;
         A.CallTo(() => _configuration.DefaultTemplateUserDatabasePath).Returns(FileHelper.GenerateTemporaryFileName());
         FileHelper.FileExists = s => string.Equals(s, _oldDbFile);

         FileHelper.Copy = copyAction;
      }

      private void copyAction(string source, string target)
      {
         _didPerformCopy = true;
      }

      protected override void Because()
      {
         sut.CreateDefaultTemplateDatabase();
      }

      [Observation]
      public void should_show_a_message_box_indicating_that_template_database_is_not_supported()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void should_copy_the_template_database_to_the_user_profile()
      {
         _didPerformCopy.ShouldBeTrue();
      }

      [Observation]
      public void should_have_updated_the_user_database_path_to_point_to_the_default_location()
      {
         _userSettings.TemplateDatabasePath.ShouldBeEqualTo(_configuration.DefaultTemplateUserDatabasePath);
      }

      public override void Cleanup()
      {
         base.Cleanup();
         FileHelper.Copy = _oldCopy;
         FileHelper.FileExists = _oldFileExists;
      }
   }


}