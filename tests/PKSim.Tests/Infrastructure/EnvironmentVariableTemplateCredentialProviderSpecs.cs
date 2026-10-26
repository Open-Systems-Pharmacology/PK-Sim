using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services.TemplateCredentials;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_EnvironmentVariableTemplateCredentialProvider : ContextSpecification<EnvironmentVariableTemplateCredentialProvider>
   {
      protected Uri _uri;
      protected string _hostVariable;
      private string _originalHostValue;
      private string _originalGitHubToken;
      protected TemplateCredential _result;

      protected override void Context()
      {
         _uri = new Uri("https://templates.company.com/lib/templates.json");
         _hostVariable = TemplateCredentialHost.EnvironmentVariableNameFor(_uri);
         _originalHostValue = Environment.GetEnvironmentVariable(_hostVariable);
         _originalGitHubToken = Environment.GetEnvironmentVariable(TemplateCredentialHost.GITHUB_TOKEN_ENVIRONMENT_VARIABLE);
         Environment.SetEnvironmentVariable(_hostVariable, null);
         Environment.SetEnvironmentVariable(TemplateCredentialHost.GITHUB_TOKEN_ENVIRONMENT_VARIABLE, null);
         sut = new EnvironmentVariableTemplateCredentialProvider();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         Environment.SetEnvironmentVariable(_hostVariable, _originalHostValue);
         Environment.SetEnvironmentVariable(TemplateCredentialHost.GITHUB_TOKEN_ENVIRONMENT_VARIABLE, _originalGitHubToken);
      }
   }

   public class When_a_token_is_defined_for_the_host_in_the_environment : concern_for_EnvironmentVariableTemplateCredentialProvider
   {
      protected override void Context()
      {
         base.Context();
         Environment.SetEnvironmentVariable(_hostVariable, " my-token ");
      }

      protected override void Because()
      {
         _result = sut.CredentialFor(_uri);
      }

      [Observation]
      public void should_return_the_token_as_credential_without_user_name()
      {
         _result.Secret.ShouldBeEqualTo("my-token");
         _result.HasUserName.ShouldBeFalse();
      }

      [Observation]
      public void should_not_be_interactive()
      {
         sut.IsInteractive.ShouldBeFalse();
      }
   }

   public class When_no_token_is_defined_for_the_host_in_the_environment : concern_for_EnvironmentVariableTemplateCredentialProvider
   {
      protected override void Because()
      {
         _result = sut.CredentialFor(_uri);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_only_the_github_token_is_defined_in_the_environment : concern_for_EnvironmentVariableTemplateCredentialProvider
   {
      protected override void Context()
      {
         base.Context();
         Environment.SetEnvironmentVariable(TemplateCredentialHost.GITHUB_TOKEN_ENVIRONMENT_VARIABLE, "github-token");
      }

      [Observation]
      public void should_use_it_for_github_content_hosts()
      {
         sut.CredentialFor(new Uri("https://raw.githubusercontent.com/Company/Library/main/templates.json")).Secret.ShouldBeEqualTo("github-token");
      }

      [Observation]
      public void should_not_use_it_for_other_hosts()
      {
         sut.CredentialFor(_uri).ShouldBeNull();
      }
   }
}
