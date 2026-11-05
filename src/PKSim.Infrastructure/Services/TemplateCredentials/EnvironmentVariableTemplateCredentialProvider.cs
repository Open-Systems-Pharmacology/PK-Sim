using System;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services.TemplateCredentials
{
   /// <summary>
   ///    Reads a token from the environment. The variable PKSIM_TEMPLATE_TOKEN_&lt;HOST&gt; (host upper cased, dots and dashes
   ///    replaced with underscores) is checked first. For github hosts, GITHUB_TOKEN is used as fallback.
   ///    This is the typical way to configure a token for scripted or CI usage (CLI, R).
   /// </summary>
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
