using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_UserSettingsToUserSettingsDTOMapper : ContextSpecification<IUserSettingsToUserSettingsDTOMapper>
   {
      protected IUserSettings _userSettings;
      protected TemplateSource _templateSource;

      protected override void Context()
      {
         _userSettings = A.Fake<IUserSettings>();
         _templateSource = new TemplateSource("Company Library", "https://templates.company.com/lib/templates.json");
         A.CallTo(() => _userSettings.TemplateSources).Returns(new[] {_templateSource});
         sut = new UserSettingsToUserSettingsDTOMapper();
      }
   }

   public class When_mapping_user_settings_to_a_user_settings_dto : concern_for_UserSettingsToUserSettingsDTOMapper
   {
      private UserSettingsDTO _result;

      protected override void Because()
      {
         _result = sut.MapFrom(_userSettings);
      }

      [Observation]
      public void should_map_the_additional_template_sources()
      {
         _result.TemplateSources.Count.ShouldBeEqualTo(1);
         _result.TemplateSources[0].Name.ShouldBeEqualTo(_templateSource.Name);
         _result.TemplateSources[0].Url.ShouldBeEqualTo(_templateSource.Url);
      }
   }

   public class When_updating_user_settings_from_a_user_settings_dto : concern_for_UserSettingsToUserSettingsDTOMapper
   {
      private UserSettingsDTO _userSettingsDTO;

      protected override void Context()
      {
         base.Context();
         _userSettingsDTO = sut.MapFrom(_userSettings);
         _userSettingsDTO.TemplateSources.Add(new TemplateSourceDTO {Name = "Other", Url = @"C:\Templates\templates.json"});
      }

      protected override void Because()
      {
         sut.UpdateUserSettingsFrom(_userSettingsDTO, _userSettings);
      }

      [Observation]
      public void should_replace_the_template_sources_with_the_edited_ones()
      {
         A.CallTo(() => _userSettings.ClearTemplateSources()).MustHaveHappened()
            .Then(A.CallTo(() => _userSettings.AddTemplateSource(_templateSource)).MustHaveHappened())
            .Then(A.CallTo(() => _userSettings.AddTemplateSource(new TemplateSource("Other", @"C:\Templates\templates.json"))).MustHaveHappened());
      }
   }
}
