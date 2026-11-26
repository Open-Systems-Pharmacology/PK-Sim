using System;

namespace PKSim.Core.Services
{
   public static class TemplateCredentialHost
   {
      public const string GITHUB_HOST = "github.com";
      public const string ENVIRONMENT_VARIABLE_PREFIX = "PKSIM_TEMPLATE_TOKEN_";
      public const string GITHUB_TOKEN_ENVIRONMENT_VARIABLE = "GITHUB_TOKEN";

      public static bool IsGitHubHost(Uri uri)
      {
         var host = uri.Host.ToLowerInvariant();
         return host == GITHUB_HOST ||
                host.EndsWith($".{GITHUB_HOST}") ||
                host.EndsWith(".githubusercontent.com");
      }

      //content hosts of GitHub (e.g. raw.githubusercontent.com) are authenticated with the github.com credential
      public static string CredentialHostFor(Uri uri) => IsGitHubHost(uri) ? GITHUB_HOST : uri.Host.ToLowerInvariant();

      public static string EnvironmentVariableNameFor(Uri uri)
      {
         var host = uri.Host.ToUpperInvariant().Replace('.', '_').Replace('-', '_');
         return $"{ENVIRONMENT_VARIABLE_PREFIX}{host}";
      }
   }
}
