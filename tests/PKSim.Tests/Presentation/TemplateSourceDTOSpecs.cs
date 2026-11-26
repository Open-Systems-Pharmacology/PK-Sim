using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_TemplateSourceDTO : ContextSpecification<TemplateSourceDTO>
   {
      protected override void Context()
      {
         sut = new TemplateSourceDTO {Name = "Company Library", Url = "https://templates.company.com/lib/templates.json"};
      }
   }

   public class When_validating_a_template_source_dto : concern_for_TemplateSourceDTO
   {
      [Observation]
      public void should_be_valid_when_name_and_url_are_defined()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_be_invalid_without_name()
      {
         sut.Name = string.Empty;
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_invalid_with_an_invalid_url()
      {
         sut.Url = "not a url";
         sut.IsValid().ShouldBeFalse();
      }

      [Observation]
      public void should_be_valid_with_a_local_path()
      {
         sut.Url = @"C:\Templates\templates.json";
         sut.IsValid().ShouldBeTrue();
      }
   }

   public class When_creating_a_template_source_dto_from_a_template_source : concern_for_TemplateSourceDTO
   {
      private TemplateSource _templateSource;

      protected override void Context()
      {
         _templateSource = new TemplateSource("Company Library", "https://templates.company.com/lib/templates.json");
      }

      protected override void Because()
      {
         sut = new TemplateSourceDTO(_templateSource);
      }

      [Observation]
      public void should_copy_name_and_url()
      {
         sut.Name.ShouldBeEqualTo(_templateSource.Name);
         sut.Url.ShouldBeEqualTo(_templateSource.Url);
      }
   }

   public class When_converting_a_template_source_dto_to_a_template_source : concern_for_TemplateSourceDTO
   {
      private TemplateSource _result;

      protected override void Context()
      {
         base.Context();
         sut.Name = "  Company Library ";
         sut.Url = " https://templates.company.com/lib/templates.json  ";
      }

      protected override void Because()
      {
         _result = sut.ToTemplateSource();
      }

      [Observation]
      public void should_trim_name_and_url()
      {
         _result.Name.ShouldBeEqualTo("Company Library");
         _result.Url.ShouldBeEqualTo("https://templates.company.com/lib/templates.json");
      }
   }
}
