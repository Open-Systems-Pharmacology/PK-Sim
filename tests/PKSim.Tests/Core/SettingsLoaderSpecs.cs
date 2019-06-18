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
      protected IApplicationSettingsPersistor _applicationSettingsPersistor;
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
         _applicationSettingsPersistor = A.Fake<IApplicationSettingsPersistor>();
         _configuration =A.Fake<IPKSimConfiguration>();
         _updateFile = FileHelper.GenerateTemporaryFileName();
         _userSettings = A.Fake<IUserSettings>();
         A.CallTo(() => _userSettings.TemplateDatabasePath).Returns("DbPath");

         A.CallTo(() => _configuration.TemplateSystemDatabasePath).Returns("SystemPath");
         A.CallTo(() => _userSettingsPersistor.Load()).Returns(_userSettings);
         
         sut = new SettingsLoader(_userSettingsPersistor, _applicationSettingsPersistor);
  
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

}