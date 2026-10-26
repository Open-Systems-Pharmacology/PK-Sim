using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_TemplateSource : ContextSpecification<TemplateSource>
   {
      protected override void Context()
      {
         sut = new TemplateSource("Company Library", "https://templates.company.com/lib/templates.json");
      }
   }

   public class When_cloning_a_template_source : concern_for_TemplateSource
   {
      private TemplateSource _clone;

      protected override void Because()
      {
         _clone = sut.Clone();
      }

      [Observation]
      public void should_return_a_new_instance_with_the_same_name_and_url()
      {
         _clone.ShouldNotBeEqualTo(null);
         ReferenceEquals(_clone, sut).ShouldBeFalse();
         _clone.Name.ShouldBeEqualTo(sut.Name);
         _clone.Url.ShouldBeEqualTo(sut.Url);
      }

      [Observation]
      public void should_be_equal_to_the_original_source()
      {
         _clone.Equals(sut).ShouldBeTrue();
         _clone.GetHashCode().ShouldBeEqualTo(sut.GetHashCode());
      }
   }

   public class When_comparing_template_sources : concern_for_TemplateSource
   {
      [Observation]
      public void should_not_be_equal_to_a_source_with_another_url()
      {
         sut.Equals(new TemplateSource(sut.Name, "https://other.com/templates.json")).ShouldBeFalse();
      }

      [Observation]
      public void should_not_be_equal_to_a_source_with_another_name()
      {
         sut.Equals(new TemplateSource("Other", sut.Url)).ShouldBeFalse();
      }
   }

   public class When_checking_if_the_url_of_a_template_source_is_valid : concern_for_TemplateSource
   {
      [TestCase("https://templates.company.com/lib/templates.json")]
      [TestCase("http://intranet/templates.json")]
      [TestCase(@"C:\Templates\templates.json")]
      [TestCase(@"\\server\share\templates\templates.json")]
      [TestCase("  https://templates.company.com/lib/templates.json  ")]
      public void should_accept_http_urls_and_local_paths(string url)
      {
         TemplateSource.IsValidUrl(url).ShouldBeTrue();
         new TemplateSource("Name", url).HasValidUrl.ShouldBeTrue();
      }

      [TestCase("ftp://templates.company.com/templates.json")]
      [TestCase("templates.json")]
      [TestCase("not a url")]
      [TestCase("")]
      [TestCase(null)]
      public void should_reject_anything_else(string url)
      {
         TemplateSource.IsValidUrl(url).ShouldBeFalse();
         new TemplateSource("Name", url).HasValidUrl.ShouldBeFalse();
      }
   }

   public class When_retrieving_the_uri_of_a_template_source_defined_with_a_local_path : concern_for_TemplateSource
   {
      protected override void Context()
      {
         sut = new TemplateSource("Local", @"C:\Templates\templates.json");
      }

      [Observation]
      public void should_return_a_file_uri_pointing_to_the_path()
      {
         sut.Uri.IsFile.ShouldBeTrue();
         sut.Uri.LocalPath.ShouldBeEqualTo(@"C:\Templates\templates.json");
      }
   }

   public class When_retrieving_the_uri_of_a_template_source_with_an_invalid_url : concern_for_TemplateSource
   {
      protected override void Context()
      {
         sut = new TemplateSource("Invalid", "not a url");
      }

      [Observation]
      public void should_return_null()
      {
         sut.Uri.ShouldBeNull();
      }
   }

   public class When_retrieving_the_key_of_a_template_source : concern_for_TemplateSource
   {
      [Observation]
      public void should_return_the_same_key_for_the_same_url_independently_of_case_and_surrounding_spaces()
      {
         var sameSource = new TemplateSource("Other name", $"  {sut.Url.ToUpperInvariant()} ");
         sameSource.Key.ShouldBeEqualTo(sut.Key);
      }

      [Observation]
      public void should_return_a_different_key_for_another_url()
      {
         new TemplateSource(sut.Name, "https://other.com/templates.json").Key.ShouldNotBeEqualTo(sut.Key);
      }

      [Observation]
      public void should_return_a_short_key_usable_as_folder_name()
      {
         sut.Key.Length.ShouldBeEqualTo(16);
         sut.Key.ShouldBeEqualTo(sut.Key.ToLowerInvariant());
      }
   }
}
