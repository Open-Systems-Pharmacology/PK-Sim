using System;
using System.Runtime.Versioning;
using Meziantou.Framework.Win32;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Services;

namespace PKSim.UI.Services
{
   /// <summary>
   ///    Last resort of the credential chain: asks the user with the native Windows credential dialog. When the user ticks
   ///    "Remember my credentials", the credential is written by Windows into the Credential Manager under the host name,
   ///    where <c>WindowsCredentialManagerTemplateCredentialProvider</c> finds it next time. PK-Sim itself never persists it.
   /// </summary>
   public class WindowsCredentialPromptTemplateCredentialProvider : ITemplateCredentialProvider
   {
      private readonly IOSPSuiteLogger _logger;

      public WindowsCredentialPromptTemplateCredentialProvider(IOSPSuiteLogger logger)
      {
         _logger = logger;
      }

      public int Priority => 100;
      public bool IsInteractive => true;

      public TemplateCredential CredentialFor(Uri uri)
      {
         if (!OperatingSystem.IsWindows())
            return null;

         return promptFor(uri);
      }

      [SupportedOSPlatform("windows")]
      private TemplateCredential promptFor(Uri uri)
      {
         try
         {
            var result = CredentialManager.PromptForCredentials(
               owner: IntPtr.Zero,
               messageText: PKSimConstants.UI.TemplateSourceCredentialsMessage(uri.Host),
               captionText: PKSimConstants.UI.TemplateSourceCredentialsCaption,
               userName: null,
               saveCredential: CredentialSaveOption.Unselected);

            if (result == null || string.IsNullOrEmpty(result.Password))
               return null;

            if (result.CredentialSaved == CredentialSaveOption.Selected)
               CredentialManager.WriteCredential(uri.Host, result.UserName, result.Password, PKSimConstants.UI.TemplateSourceCredentialsCaption, CredentialPersistence.LocalMachine);

            return new TemplateCredential(result.UserName, result.Password);
         }
         catch (Exception e)
         {
            _logger.AddDebug($"The credential prompt for '{uri.Host}' failed: {e.Message}");
            return null;
         }
      }
   }
}
