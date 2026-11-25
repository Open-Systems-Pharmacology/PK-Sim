using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services.TemplateCredentials;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_EnvironmentVariableTemplateCredentialProvider : ContextSpecification<EnvironmentVariableTemplateCredentialProvider>
   {
      protected Uri _uri;
      protected Uri _gitHubUri;
      protected string _hostVariable;
      private readonly Dictionary<string, string> _originalValues = new Dictionary<string, string>();
      protected TemplateCredential _result;

      protected override void Context()
      {
         _uri = new Uri("https://templates.company.com/lib/templates.json");
         _gitHubUri = new Uri("https://raw.githubusercontent.com/Company/Library/main/templates.json");
         _hostVariable = TemplateCredentialHost.EnvironmentVariableNameFor(_uri);

         var variablesReadByTheProvider = new[]
         {
            _hostVariable,
            TemplateCredentialHost.EnvironmentVariableNameFor(_gitHubUri),
            TemplateCredentialHost.GITHUB_TOKEN_ENVIRONMENT_VARIABLE
         };

         foreach (var variable in variablesReadByTheProvider)
         {
            _originalValues[variable] = Environment.GetEnvironmentVariable(variable);
            Environment.SetEnvironmentVariable(variable, null);
         }

         sut = new EnvironmentVariableTemplateCredentialProvider();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         foreach (var (variable, value) in _originalValues)
         {
            Environment.SetEnvironmentVariable(variable, value);
         }
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
         sut.CredentialFor(_gitHubUri).Secret.ShouldBeEqualTo("github-token");
      }

      [Observation]
      public void should_not_use_it_for_other_hosts()
      {
         sut.CredentialFor(_uri).ShouldBeNull();
      }
   }
}
