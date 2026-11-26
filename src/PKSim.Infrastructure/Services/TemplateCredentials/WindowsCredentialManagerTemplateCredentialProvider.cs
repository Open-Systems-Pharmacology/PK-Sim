using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using Meziantou.Framework.Win32;
using OSPSuite.Core.Services;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services.TemplateCredentials
{
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

      //git:https://<host> is the target under which Git Credential Manager stores its credentials
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
