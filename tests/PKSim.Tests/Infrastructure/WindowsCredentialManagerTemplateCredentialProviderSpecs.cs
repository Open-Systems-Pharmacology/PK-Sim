using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using PKSim.Infrastructure.Services.TemplateCredentials;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_WindowsCredentialManagerTemplateCredentialProvider : ContextSpecification<WindowsCredentialManagerTemplateCredentialProvider>
   {
      protected override void Context()
      {
         sut = new WindowsCredentialManagerTemplateCredentialProvider(A.Fake<IOSPSuiteLogger>());
      }
   }

   public class When_resolving_the_targets_to_look_up_in_the_windows_credential_manager : concern_for_WindowsCredentialManagerTemplateCredentialProvider
   {
      [Observation]
      public void should_look_up_the_host_and_the_github_credential_targets_for_github_content()
      {
         var targets = WindowsCredentialManagerTemplateCredentialProvider.TargetsFor(new Uri("https://raw.githubusercontent.com/Company/Library/main/templates.json"));
         targets.ShouldOnlyContainInOrder("raw.githubusercontent.com", "git:https://raw.githubusercontent.com", "github.com", "git:https://github.com");
      }

      [Observation]
      public void should_look_up_the_host_and_the_git_credential_manager_target_for_other_hosts()
      {
         var targets = WindowsCredentialManagerTemplateCredentialProvider.TargetsFor(new Uri("https://Templates.Company.com/templates.json"));
         targets.ShouldOnlyContainInOrder("templates.company.com", "git:https://templates.company.com");
      }
   }

   public class When_reading_a_credential_that_does_not_exist_in_the_windows_credential_manager : concern_for_WindowsCredentialManagerTemplateCredentialProvider
   {
      [Observation]
      public void should_return_null()
      {
         sut.CredentialFor(new Uri("https://no-credential-for-this-host.invalid/templates.json")).ShouldBeNull();
      }

      [Observation]
      public void should_not_be_interactive()
      {
         sut.IsInteractive.ShouldBeFalse();
      }
   }
}
