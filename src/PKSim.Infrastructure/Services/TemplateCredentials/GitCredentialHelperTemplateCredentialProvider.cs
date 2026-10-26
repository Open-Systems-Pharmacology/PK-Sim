using System;
using System.Collections.Generic;
using System.Diagnostics;
using OSPSuite.Core.Services;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services.TemplateCredentials
{
   /// <summary>
   ///    Asks the git credential helpers configured on the machine (Git Credential Manager, osxkeychain, store, ...) for the
   ///    credential they hold for the host. Anyone who already pushes to the host from this machine has nothing to configure.
   ///    The helpers are queried in non interactive mode so that git never prompts on its own.
   /// </summary>
   public class GitCredentialHelperTemplateCredentialProvider : ITemplateCredentialProvider
   {
      private const string USER_NAME_KEY = "username";
      private const string PASSWORD_KEY = "password";
      private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(15);
      private readonly IOSPSuiteLogger _logger;

      public GitCredentialHelperTemplateCredentialProvider(IOSPSuiteLogger logger)
      {
         _logger = logger;
      }

      public int Priority => 30;
      public bool IsInteractive => false;

      public TemplateCredential CredentialFor(Uri uri)
      {
         var host = TemplateCredentialHost.CredentialHostFor(uri);
         try
         {
            return ParseCredentialOutput(runGitCredentialFill(host));
         }
         catch (Exception e)
         {
            //git is not installed or cannot be started. Nothing to do, another provider may be able to help
            _logger.AddDebug($"git credential helper could not be queried for '{host}': {e.Message}");
            return null;
         }
      }

      /// <summary>
      ///    Parses the key=value output of <c>git credential fill</c>
      /// </summary>
      public static TemplateCredential ParseCredentialOutput(string output)
      {
         if (string.IsNullOrWhiteSpace(output))
            return null;

         var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
         foreach (var line in output.Split('\n'))
         {
            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
               continue;

            values[line.Substring(0, separatorIndex).Trim()] = line.Substring(separatorIndex + 1).Trim();
         }

         if (!values.TryGetValue(PASSWORD_KEY, out var password) || string.IsNullOrEmpty(password))
            return null;

         values.TryGetValue(USER_NAME_KEY, out var userName);
         return new TemplateCredential(userName, password);
      }

      private static string runGitCredentialFill(string host)
      {
         var startInfo = new ProcessStartInfo("git", "credential fill")
         {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
         };

         //never let git or the credential manager prompt the user
         startInfo.Environment["GIT_TERMINAL_PROMPT"] = "0";
         startInfo.Environment["GCM_INTERACTIVE"] = "never";

         using var process = Process.Start(startInfo);
         if (process == null)
            return null;

         process.StandardInput.Write($"protocol=https\nhost={host}\n\n");
         process.StandardInput.Close();

         var output = process.StandardOutput.ReadToEndAsync();
         var error = process.StandardError.ReadToEndAsync();

         if (!process.WaitForExit((int) _timeout.TotalMilliseconds))
         {
            process.Kill(entireProcessTree: true);
            return null;
         }

         //error output is read to avoid dead locks, its content is of no interest
         error.GetAwaiter().GetResult();
         return process.ExitCode == 0 ? output.GetAwaiter().GetResult() : null;
      }
   }
}
