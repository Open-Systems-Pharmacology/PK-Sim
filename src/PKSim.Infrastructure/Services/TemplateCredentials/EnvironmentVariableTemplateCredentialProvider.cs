using System;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services.TemplateCredentials
{
   public class EnvironmentVariableTemplateCredentialProvider : ITemplateCredentialProvider
   {
      public int Priority => 10;
      public bool IsInteractive => false;

      public TemplateCredential CredentialFor(Uri uri)
      {
         var token = Environment.GetEnvironmentVariable(TemplateCredentialHost.EnvironmentVariableNameFor(uri));

         if (string.IsNullOrWhiteSpace(token) && TemplateCredentialHost.IsGitHubHost(uri))
            token = Environment.GetEnvironmentVariable(TemplateCredentialHost.GITHUB_TOKEN_ENVIRONMENT_VARIABLE);

         return string.IsNullOrWhiteSpace(token) ? null : new TemplateCredential(userName: null, secret: token.Trim());
      }
   }
}
