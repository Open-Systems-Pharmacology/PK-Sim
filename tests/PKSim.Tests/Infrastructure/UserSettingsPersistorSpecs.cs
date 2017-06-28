using System;
using OSPSuite.Serializer.Xml;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using FakeItEasy;
using PKSim.Presentation;
using PKSim.Presentation.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_UserSettingsPersistor : ContextSpecification<IUserSettingsPersistor>
   {
      protected IStringSerializer _serializationManager;
      protected IUserSettings _userSettings;
      protected string _validUserConfigXml;
      protected string _invalidUserConfigXml;
      protected IUserSettings _defaultUserSettings;
      protected IPKSimConfiguration _pkSimConfiguration;
      private Func<string, bool> _oldFileExits;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldFileExits = FileHelper.FileExists;

      }
      protected override void Context()
      {
         _userSettings = A.Fake<IUserSettings>();
         _defaultUserSettings = A.Fake<IUserSettings>();
         _serializationManager = A.Fake<IStringSerializer>();
         _pkSimConfiguration = A.Fake<IPKSimConfiguration>();
         A.CallTo(() => _pkSimConfiguration.UserSettingsFilePath).Returns("blah");
         _validUserConfigXml = "<UserConfig/>";
         A.CallTo(() => _serializationManager.Serialize(_userSettings)).Returns(_validUserConfigXml);
         sut = new UserSettingsPersistor(_serializationManager, _defaultUserSettings, _pkSimConfiguration);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         XmlHelper.Reset();
         FileHelper.FileExists =_oldFileExits;
      }
   }

   
   public class When_persisting_the_user_config_to_a_file : concern_for_UserSettingsPersistor
   {
      private bool _saved;

      public override void GlobalContext()
      {
         base.GlobalContext();
         XmlHelper.SaveXmlContentToFile = (content, path) => { _saved = string.Equals(path, _pkSimConfiguration.UserSettingsFilePath); };
      }

      protected override void Because()
      {
         sut.Save(_userSettings);
      }

      [Observation]
      public void should_save_the_user_config_to_the_user_file()
      {
         _saved.ShouldBeEqualTo(true);
      }

      [Observation]
      public void should_save_the_layout_in_the_user_settings()
      {
         A.CallTo(() => _userSettings.SaveLayout()).MustHaveHappened();
      }
   }

   
   public class When_loading_ther_user_config_from_a_config_file_that_does_exist : concern_for_UserSettingsPersistor
   {
      private ICoreUserSettings _result;

      public override void GlobalContext()
      {
         base.GlobalContext();
         FileHelper.FileExists = s => true;
      }

      protected override void Context()
      {
         base.Context();
         sut.Save(_userSettings);
         A.CallTo(() => _pkSimConfiguration.UserSettingsFilePaths).Returns(new[] { "blah" });
         A.CallTo(() => _serializationManager.Deserialize<IUserSettings>(A<string>.Ignored)).Returns(_userSettings);
      }

      protected override void Because()
      {
         _result = sut.Load();
      }

      [Observation]
      public void should_load_the_default_config_from_the_file()
      {
         _result.ShouldBeEqualTo(_userSettings);
      }
  }

   
   public class When_loading_ther_user_config_from_a_config_file_that_does_not_exist : concern_for_UserSettingsPersistor
   {
      private ICoreUserSettings _result;

      public override void GlobalContext()
      {
         base.GlobalContext();
         FileHelper.FileExists = s => false;
      }

      protected override void Because()
      {
         _result = sut.Load();
      }

      [Observation]
      public void should_return_the_default_user_config()
      {
         _result.ShouldBeEqualTo(_defaultUserSettings);
      }

   }

   
   public class When_loading_ther_user_config_from_a_corrupted_config_file : concern_for_UserSettingsPersistor
   {
      private ICoreUserSettings _result;
      private string _content;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _content = "trala";
         XmlHelper.XmlContentFromFile = file => { return _content; };
         FileHelper.FileExists = s => true;
      }

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _serializationManager.Deserialize<ICoreUserSettings>(_content)).Throws(new Exception());
      }

      protected override void Because()
      {
         _result = sut.Load();
      }

      [Observation]
      public void should_return_the_default_user_config()
      {
         _result.ShouldBeEqualTo(_defaultUserSettings);
      }
   }
}