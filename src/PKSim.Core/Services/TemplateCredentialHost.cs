using System;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Helper mapping the host of a template uri to the identifiers used by the various credential stores
   /// </summary>
   public static class TemplateCredentialHost
   {
      public const string GITHUB_HOST = "github.com";
      public const string ENVIRONMENT_VARIABLE_PREFIX = "PKSIM_TEMPLATE_TOKEN_";
      public const string GITHUB_TOKEN_ENVIRONMENT_VARIABLE = "GITHUB_TOKEN";

      /// <summary>
      ///    Returns true if the uri points to github.com or one of the hosts serving github content (e.g.
      ///    raw.githubusercontent.com)
      /// </summary>
      public static bool IsGitHubHost(Uri uri)
      {
         var host = uri.Host.ToLowerInvariant();
         return host == GITHUB_HOST ||
                host.EndsWith($".{GITHUB_HOST}") ||
                host.EndsWith(".githubusercontent.com");
      }

      /// <summary>
      ///    Host under which credential stores (git credential helpers, OS vault) typically keep the credential granting access
      ///    to the uri. Content hosts of github are all authenticated with the github.com credential
      /// </summary>
      public static string CredentialHostFor(Uri uri) => IsGitHubHost(uri) ? GITHUB_HOST : uri.Host.ToLowerInvariant();

      /// <summary>
      ///    Name of the environment variable that may hold a token for the host of the uri. For example
      ///    PKSIM_TEMPLATE_TOKEN_RAW_GITHUBUSERCONTENT_COM
      /// </summary>
      public static string EnvironmentVariableNameFor(Uri uri)
      {
         var host = uri.Host.ToUpperInvariant().Replace('.', '_').Replace('-', '_');
         return $"{ENVIRONMENT_VARIABLE_PREFIX}{host}";
      }
   }
}
