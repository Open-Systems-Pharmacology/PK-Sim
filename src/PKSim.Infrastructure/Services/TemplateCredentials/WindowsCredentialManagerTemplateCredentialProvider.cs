using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Meziantou.Framework.Win32;
using OSPSuite.Core.Services;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services.TemplateCredentials
{
   /// <summary>
   ///    Reads a generic credential from the Windows Credential Manager. The credential is looked up under the host of the
   ///    uri (e.g. <c>raw.githubusercontent.com</c>), the credential host (e.g. <c>github.com</c>) and the targets used by
   ///    Git Credential Manager (e.g. <c>git:https://github.com</c>). Users or IT create the entry once
   ///    (<c>cmdkey /generic:raw.githubusercontent.com /user:name /pass:token</c> or the Credential Manager control panel).
   ///    PK-Sim only reads it.
   /// </summary>
   public class WindowsCredentialManagerTemplateCredentialProvider : ITemplateCredentialProvider
   {
      private readonly IOSPSuiteLogger _logger;

      public WindowsCredentialManagerTemplateCredentialProvider(IOSPSuiteLogger logger)
      {
         _logger = logger;
      }

      public int Priority => 20;
      public bool IsInteractive => false;

      public TemplateCredential CredentialFor(Uri uri)
      {
         if (!OperatingSystem.IsWindows())
            return null;

         return readCredential(uri);
      }

      /// <summary>
      ///    Targets under which the credential for the uri may be stored, by decreasing specificity
      /// </summary>
      public static IReadOnlyList<string> TargetsFor(Uri uri)
      {
         var host = uri.Host.ToLowerInvariant();
         var credentialHost = TemplateCredentialHost.CredentialHostFor(uri);
         return new[]
            {
               host,
               $"git:https://{host}",
               credentialHost,
               $"git:https://{credentialHost}",
            }
            .Distinct()
            .ToList();
      }

      [SupportedOSPlatform("windows")]
      private TemplateCredential readCredential(Uri uri)
      {
         foreach (var target in TargetsFor(uri))
         {
            try
            {
               var credential = CredentialManager.ReadCredential(target);
               if (credential == null || string.IsNullOrEmpty(credential.Password))
                  continue;

               _logger.AddDebug($"Using credential '{target}' from the Windows Credential Manager for '{uri.Host}'");
               return new TemplateCredential(credential.UserName, credential.Password);
            }
            catch (Exception e)
            {
               _logger.AddDebug($"Credential '{target}' could not be read from the Windows Credential Manager: {e.Message}");
            }
         }

         return null;
      }
   }
}
