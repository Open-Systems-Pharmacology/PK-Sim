using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services.TemplateCredentials;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_GitCredentialHelperTemplateCredentialProvider : ContextSpecification<GitCredentialHelperTemplateCredentialProvider>
   {
      protected override void Context()
      {
         sut = new GitCredentialHelperTemplateCredentialProvider(A.Fake<IOSPSuiteLogger>());
      }
   }

   public class When_parsing_the_output_of_git_credential_fill : concern_for_GitCredentialHelperTemplateCredentialProvider
   {
      [Observation]
      public void should_return_user_name_and_password()
      {
         var credential = GitCredentialHelperTemplateCredentialProvider.ParseCredentialOutput("protocol=https\nhost=github.com\nusername=someone\npassword=ghp_token\n");
         credential.UserName.ShouldBeEqualTo("someone");
         credential.Secret.ShouldBeEqualTo("ghp_token");
      }

      [Observation]
      public void should_support_windows_line_endings()
      {
         var credential = GitCredentialHelperTemplateCredentialProvider.ParseCredentialOutput("username=someone\r\npassword=secret\r\n");
         credential.UserName.ShouldBeEqualTo("someone");
         credential.Secret.ShouldBeEqualTo("secret");
      }

      [Observation]
      public void should_keep_equal_signs_contained_in_the_password()
      {
         GitCredentialHelperTemplateCredentialProvider.ParseCredentialOutput("username=u\npassword=abc==\n").Secret.ShouldBeEqualTo("abc==");
      }

      [Observation]
      public void should_return_null_when_no_password_was_returned()
      {
         GitCredentialHelperTemplateCredentialProvider.ParseCredentialOutput("protocol=https\nhost=github.com\n").ShouldBeNull();
         GitCredentialHelperTemplateCredentialProvider.ParseCredentialOutput(string.Empty).ShouldBeNull();
         GitCredentialHelperTemplateCredentialProvider.ParseCredentialOutput(null).ShouldBeNull();
      }
   }

   public class When_querying_the_git_credential_helper_for_a_host : concern_for_GitCredentialHelperTemplateCredentialProvider
   {
      [Observation]
      public void should_not_throw_whatever_git_setup_is_found_on_the_machine()
      {
         sut.CredentialFor(new Uri("https://no-credential-for-this-host.invalid/templates.json"));
      }

      [Observation]
      public void should_not_be_interactive()
      {
         sut.IsInteractive.ShouldBeFalse();
      }
   }
}
