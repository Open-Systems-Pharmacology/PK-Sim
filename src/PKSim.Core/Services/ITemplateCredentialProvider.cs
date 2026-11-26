using System;

namespace PKSim.Core.Services
{
   public class TemplateCredential
   {
      public string UserName { get; }
      public string Secret { get; }

      public TemplateCredential(string userName, string secret)
      {
         UserName = userName;
         Secret = secret;
      }

      public bool HasUserName => !string.IsNullOrEmpty(UserName);
   }

   public interface ITemplateCredentialProvider
   {
      //Providers with a lower value are queried first
      int Priority { get; }

      bool IsInteractive { get; }

      TemplateCredential CredentialFor(Uri uri);
   }
}
