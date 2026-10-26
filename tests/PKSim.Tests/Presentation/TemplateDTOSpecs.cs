using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_TemplateDTO : ContextSpecification<TemplateDTO>
   {
   }

   public class When_displaying_the_source_of_a_remote_template_from_the_osp_library : concern_for_TemplateDTO
   {
      protected override void Context()
      {
         sut = new TemplateDTO(new RemoteTemplate {Name = "Alprazolam"});
      }

      [Observation]
      public void should_display_the_osp_library()
      {
         sut.DatabaseTypeDisplay.ShouldBeEqualTo(PKSimConstants.UI.RemoteTemplates);
      }
   }

   public class When_displaying_the_source_of_a_remote_template_from_an_additional_source : concern_for_TemplateDTO
   {
      protected override void Context()
      {
         sut = new TemplateDTO(new RemoteTemplate {Name = "InHouse", Source = new TemplateSource("Company Library", "https://templates.company.com/templates.json")});
      }

      [Observation]
      public void should_display_the_name_of_the_source()
      {
         sut.DatabaseTypeDisplay.ShouldBeEqualTo("Company Library");
      }
   }

   public class When_displaying_the_source_of_a_local_template : concern_for_TemplateDTO
   {
      protected override void Context()
      {
         sut = new TemplateDTO(new LocalTemplate {Name = "Mine", DatabaseType = TemplateDatabaseType.User});
      }

      [Observation]
      public void should_display_the_user_templates()
      {
         sut.DatabaseTypeDisplay.ShouldBeEqualTo(PKSimConstants.UI.UserTemplates);
      }
   }
}
