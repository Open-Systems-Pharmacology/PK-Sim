using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;
using FakeItEasy;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Services;
using OSPSuite.Assets;



namespace PKSim.Presentation
{
   public abstract class concern_for_UserSettingsPresenter : ContextSpecification<IUserSettingsPresenter>
   {
      protected IUserSettingsView _view;
      protected IUserSettings _userSettings;
      protected IUserSettingsPersistor _userSettingsPersistor;
      protected ISkinManager _skinManager;
      protected IDialogCreator _dialogCreator;
      protected IPKSimConfiguration _configuration;
      protected ISpeciesRepository _speciesRepostiory;

      protected override void Context()
      {
         _view = A.Fake<IUserSettingsView>();
         _skinManager = A.Fake<ISkinManager>();
         _userSettingsPersistor = A.Fake<IUserSettingsPersistor>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _configuration = A.Fake<IPKSimConfiguration>();
         _userSettings = A.Fake<IUserSettings>();
         _speciesRepostiory =A.Fake<ISpeciesRepository>();
         sut = new UserSettingsPresenter(_view, _userSettings, _skinManager, _userSettingsPersistor, _dialogCreator,  _configuration,_speciesRepostiory);
      }
   }

   
   public class When_resolving_the_list_of_all_available_skins : concern_for_UserSettingsPresenter
   {
      private IEnumerable<string> _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _skinManager.All()).Returns(new[] {"skin1", "skin2"});
      }

      protected override void Because()
      {
         _result = sut.AvailableSkins;
      }

      [Observation]
      public void should_return_the_skins_define_in_the_application()
      {
         _result.ShouldOnlyContainInOrder("skin1", "skin2");
      }
   }

   
   public class When_resolving_the_list_of_all_available_icon_sizes : concern_for_UserSettingsPresenter
   {
      private IEnumerable<IconSize> _result;
      private IEnumerable<IconSize> _availbleIconSizes;

      protected override void Context()
      {
         base.Context();
         _availbleIconSizes = IconSizes.All();
      }

      protected override void Because()
      {
         _result = sut.AvailableIconSizes;
      }

      [Observation]
      public void should_return_the_icon_sizes_define_in_the_application()
      {
         _result.Count().ShouldBeEqualTo(_availbleIconSizes.Count());
      }
   }

   
   public class When_creating_a_new_template_database : concern_for_UserSettingsPresenter
   {
      private string _newFileToCreate;
      private string _defaultTemplateUserDatabase;
      private Action<string, string> _oldCopyAction;
      private bool _copyCalled;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldCopyAction = FileHelper.Copy;
         FileHelper.Copy = (s1, s2) =>
                              { _copyCalled = (s1 == _defaultTemplateUserDatabase && s2 == _newFileToCreate); };
      }

      protected override void Context()
      {
         base.Context();
         _newFileToCreate = "toto";
         _defaultTemplateUserDatabase = "tata";
         A.CallTo(() => _dialogCreator.AskForFileToSave(PKSimConstants.UI.CreateTemplateDatabasePath, CoreConstants.Filter.TEMPLATE_DATABASE_FILE_FILTER, CoreConstants.DirectoryKey.DATABASE, null, null)).Returns(_newFileToCreate);
         A.CallTo(() => _configuration.TemplateUserDatabaseTemplatePath).Returns(_defaultTemplateUserDatabase);
      }

      protected override void Because()
      {
         sut.CreateTemplateDatabase();
      }

      [Observation]
      public void the_user_settings_presenter_should_retrieve_a_new_path_from_the_user_where_the_database_should_be_created()
      {
         A.CallTo(() => _dialogCreator.AskForFileToSave(PKSimConstants.UI.CreateTemplateDatabasePath, CoreConstants.Filter.TEMPLATE_DATABASE_FILE_FILTER, CoreConstants.DirectoryKey.DATABASE, null, null)).MustHaveHappened();
      }

      [Observation]
      public void the_user_settins_presenter_should_save_a_copy_of_the_template_database_into_the_selected_path_()
      {
         _copyCalled.ShouldBeTrue();
      }

      [Observation]
      public void the_user_settins_presenter_should_save_the_information_in_the_user_settings()
      {
         _userSettings.TemplateDatabasePath.ShouldBeEqualTo(_newFileToCreate);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.Copy = _oldCopyAction;
      }
   }


   
   public class When_asked_to_reset_the_layout : concern_for_UserSettingsPresenter
   {
      protected override void Because()
      {
         sut.ResetLayout();
      }

      [Observation]
      public void should_ask_the_user_to_confirm_the_action()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyResetLayout, PKSimConstants.UI.Reset, PKSimConstants.UI.CancelButton)).MustHaveHappened();
      }
   }

   
   public class When_asked_to_reset_the_settings_and_the_user_cancel_the_action : concern_for_UserSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(ViewResult.No);
      }
      protected override void Because()
      {
         sut.ResetLayout();
      }

      [Observation]
      public void should_not_reset_the_settings()
      {
         A.CallTo(() => _userSettings.ResetLayout()).MustNotHaveHappened();

      }
   }


   
   public class When_asked_to_reset_the_settings_and_the_user_cancel_accepts_the_action : concern_for_UserSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(ViewResult.Yes);

      }
      protected override void Because()
      {
         sut.ResetLayout();
      }
      
      [Observation]
      public void should_not_reset_the_settings()
      {
         A.CallTo(() => _userSettings.ResetLayout()).MustHaveHappened();
      }
   }
}