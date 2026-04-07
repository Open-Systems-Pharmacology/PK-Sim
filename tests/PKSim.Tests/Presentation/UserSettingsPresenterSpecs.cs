using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;

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
      protected ISpeciesRepository _speciesRepository;
      protected IUserSettingsToUserSettingsDTOMapper _mapper;

      protected override void Context()
      {
         _view = A.Fake<IUserSettingsView>();
         _skinManager = A.Fake<ISkinManager>();
         _userSettingsPersistor = A.Fake<IUserSettingsPersistor>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _configuration = A.Fake<IPKSimConfiguration>();
         _userSettings = A.Fake<IUserSettings>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _mapper = new UserSettingsToUserSettingsDTOMapper();
         sut = new UserSettingsPresenter(_view, _userSettings, _skinManager, _userSettingsPersistor, _dialogCreator, _configuration, _speciesRepository, _mapper);
      }
   }

   public class When_resolving_the_list_of_all_available_skins : concern_for_UserSettingsPresenter
   {
      private IEnumerable<string> _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _skinManager.All()).Returns(new[] { "skin1", "skin2" });
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
      private IEnumerable<IconSize> _availableIconSizes;

      protected override void Context()
      {
         base.Context();
         _availableIconSizes = IconSizes.All();
      }

      protected override void Because()
      {
         _result = sut.AvailableIconSizes;
      }

      [Observation]
      public void should_return_the_icon_sizes_define_in_the_application()
      {
         _result.Count().ShouldBeEqualTo(_availableIconSizes.Count());
      }
   }

   public class When_editing_settings : concern_for_UserSettingsPresenter
   {
      private UserSettingsDTO _capturedDTO;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _userSettings.DecimalPlace).Returns((uint)3);
         A.CallTo(() => _userSettings.ActiveSkin).Returns("TestSkin");
         A.CallTo(() => _userSettings.DefaultSpecies).Returns("Human");
         A.CallTo(() => _view.BindTo(A<UserSettingsDTO>.Ignored))
            .Invokes(call => _capturedDTO = call.GetArgument<UserSettingsDTO>(0));
      }

      protected override void Because()
      {
         sut.EditSettings();
      }

      [Observation]
      public void should_bind_the_view_to_a_dto_with_values_from_user_settings()
      {
         _capturedDTO.ShouldNotBeNull();
         _capturedDTO.DecimalPlace.ShouldBeEqualTo((uint)3);
         _capturedDTO.ActiveSkin.ShouldBeEqualTo("TestSkin");
         _capturedDTO.DefaultSpecies.ShouldBeEqualTo("Human");
      }
   }

   public class When_saving_settings : concern_for_UserSettingsPresenter
   {
      private UserSettingsDTO _capturedDTO;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _view.BindTo(A<UserSettingsDTO>.Ignored))
            .Invokes(call => _capturedDTO = call.GetArgument<UserSettingsDTO>(0));

         sut.EditSettings();

         _capturedDTO.DecimalPlace = 7;
         _capturedDTO.ActiveSkin = "NewSkin";
         _capturedDTO.AbsTol = 1e-10;
      }

      protected override void Because()
      {
         sut.SaveSettings();
      }

      [Observation]
      public void should_update_user_settings_from_the_dto_before_persisting()
      {
         _userSettings.DecimalPlace.ShouldBeEqualTo((uint)7);
         _userSettings.ActiveSkin.ShouldBeEqualTo("NewSkin");
         _userSettings.AbsTol.ShouldBeEqualTo(1e-10);
      }

      [Observation]
      public void should_persist_the_user_settings()
      {
         A.CallTo(() => _userSettingsPersistor.Save(_userSettings)).MustHaveHappened();
      }
   }

   public class When_creating_a_new_template_database : concern_for_UserSettingsPresenter
   {
      private string _newFileToCreate;
      private string _defaultTemplateUserDatabase;
      private Action<string, string> _oldCopyAction;
      private bool _copyCalled;
      private UserSettingsDTO _capturedDTO;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldCopyAction = FileHelper.Copy;
         FileHelper.Copy = (s1, s2) => { _copyCalled = (s1 == _defaultTemplateUserDatabase && s2 == _newFileToCreate); };
      }

      protected override void Context()
      {
         base.Context();
         _newFileToCreate = "toto";
         _defaultTemplateUserDatabase = "tata";
         A.CallTo(() => _dialogCreator.AskForFileToSave(PKSimConstants.UI.CreateTemplateDatabasePath, CoreConstants.Filter.TEMPLATE_DATABASE_FILE_FILTER, CoreConstants.DirectoryKey.DATABASE, null, null)).Returns(_newFileToCreate);
         A.CallTo(() => _configuration.TemplateUserDatabaseTemplatePath).Returns(_defaultTemplateUserDatabase);
         A.CallTo(() => _view.BindTo(A<UserSettingsDTO>.Ignored))
            .Invokes(call => _capturedDTO = call.GetArgument<UserSettingsDTO>(0));

         sut.EditSettings();
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
      public void the_user_settings_presenter_should_save_a_copy_of_the_template_database_into_the_selected_path()
      {
         _copyCalled.ShouldBeTrue();
      }

      [Observation]
      public void the_user_settings_presenter_should_update_the_dto_template_database_path()
      {
         _capturedDTO.TemplateDatabasePath.ShouldBeEqualTo(_newFileToCreate);
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
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyResetLayout, PKSimConstants.UI.Reset, PKSimConstants.UI.CancelButton, ViewResult.Yes)).MustHaveHappened();
      }
   }

   public class When_asked_to_reset_the_settings_and_the_user_cancel_the_action : concern_for_UserSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, ViewResult.Yes)).Returns(ViewResult.No);
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
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored, ViewResult.Yes)).Returns(ViewResult.Yes);
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