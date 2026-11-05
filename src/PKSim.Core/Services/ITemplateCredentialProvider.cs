using System;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Credential used to download a template or a template summary from a protected source. Instances only live in memory
   ///    for the duration of the session. PK-Sim never persists them.
   /// </summary>
   public class TemplateCredential
   {
      public string UserName { get; }
      public string Secret { get; }

      public TemplateCredential(string userName, string secret)
      {
         UserName = userName;
         Secret = secret;
      }

      /// <summary>
      ///    A credential without user name is sent as bearer token, otherwise as basic authentication
      /// </summary>
      public bool HasUserName => !string.IsNullOrEmpty(UserName);
   }

   /// <summary>
   ///    Resolves a credential for a given uri from a store that is not owned by PK-Sim (environment, OS credential vault, git
   ///    credential helper, user prompt...). Providers are queried by ascending <see cref="Priority" /> until one of them
   ///    returns a credential accepted by the server.
   /// </summary>
   public interface ITemplateCredentialProvider
   {
      /// <summary>
      ///    Providers with a lower priority are queried first
      /// </summary>
      int Priority { get; }

      /// <summary>
      ///    An interactive provider requires the user attention (e.g. prompt). Interactive providers are only queried when
      ///    the download was explicitly triggered by the user
      /// </summary>
      bool IsInteractive { get; }

      /// <summary>
      ///    Returns a credential that may grant access to <paramref name="uri" /> or null if the provider has none
      /// </summary>
      TemplateCredential CredentialFor(Uri uri);
   }
}
