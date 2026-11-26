using System;
using System.Runtime.Versioning;
using Meziantou.Framework.Win32;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Services;

namespace PKSim.UI.Services
{
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
               saveToCredentialManager(uri, result);

            return new TemplateCredential(result.UserName, result.Password);
         }
         catch (Exception e)
         {
            _logger.AddDebug($"The credential prompt for '{uri.Host}' failed: {e.Message}");
            return null;
         }
      }

      [SupportedOSPlatform("windows")]
      private void saveToCredentialManager(Uri uri, CredentialResult result)
      {
         try
         {
            //stored under the host name, where WindowsCredentialManagerTemplateCredentialProvider looks it up next time
            CredentialManager.WriteCredential(uri.Host, result.UserName, result.Password, PKSimConstants.UI.TemplateSourceCredentialsCaption, CredentialPersistence.LocalMachine);
         }
         catch (Exception e)
         {
            //remembering is optional: the entered credential is still valid for this session
            _logger.AddWarning($"The credential for '{uri.Host}' could not be saved to the Windows Credential Manager: {e.Message}");
         }
      }
   }
}
