using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Core.Services;
using FakeItEasy;
using PKSim.Infrastructure.Services;
using PKSim.Presentation;
using PKSim.Presentation.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SettingsLoader : ContextSpecification<SettingsLoader>
   {
      protected IUserSettingsPersistor _userSettingsPersistor;
      protected IApplicationSettingsPersitor _applicationSettingsPersistor;
      protected IPKSimConfiguration _configuration;
      protected IUserSettings _userSettings;
      private Func<string, bool> _oldFileExists;

      protected string _updateFile;
      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileExists = FileHelper.FileExists;

      }
      protected override void Context()
      {
         _userSettingsPersistor = A.Fake<IUserSettingsPersistor>();
         _applicationSettingsPersistor = A.Fake<IApplicationSettingsPersitor>();
         _configuration =A.Fake<IPKSimConfiguration>();
         _updateFile = FileHelper.GenerateTemporaryFileName();
         _userSettings = A.Fake<IUserSettings>();
         A.CallTo(() => _userSettings.TemplateDatabasePath).Returns("DbPath");

         A.CallTo(() => _configuration.TemplateSystemDatabasePath).Returns("SystemPath");
         A.CallTo(() => _userSettingsPersistor.Load()).Returns(_userSettings);
         
         sut = new SettingsLoader(_userSettingsPersistor, _applicationSettingsPersistor, _configuration);
  
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileExists;
         FileHelper.DeleteFile(_updateFile);
      }
   }

   
   public class When_starting_the_user_config_loader : concern_for_SettingsLoader
   {

      protected override void Context()
      {
         base.Context();
         //file check should not be performed
         FileHelper.FileExists = s => { return s != _updateFile; };
      }
      protected override void Because()
      {
         sut.Start();
      }

      [Observation]
      public void should_load_the_user_settins_for_the_current_user()
      {
         A.CallTo(() => _userSettingsPersistor.Load()).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_application_settings()
      {
         A.CallTo(() => _applicationSettingsPersistor.Load()).MustHaveHappened();
      }


   }

   
   public class When_loading_the_settings_with_a_user_template_database_that_does_not_exist_in_the_user_profile : concern_for_SettingsLoader
   {
      private Action<string, string> _oldCopy;
      private bool _didPerformCopy;

      protected override void Context()
      {
         base.Context();
         //in this scenario, each call to the method should return false
         FileHelper.FileExists = s => false;
         _oldCopy = FileHelper.Copy;

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
         sut.Start();
      }

      [Observation]
      public void should_copy_the_template_database_to_the_user_profile()
      {
         _didPerformCopy.ShouldBeTrue();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         FileHelper.Copy =_oldCopy;
      }
 
   }
}