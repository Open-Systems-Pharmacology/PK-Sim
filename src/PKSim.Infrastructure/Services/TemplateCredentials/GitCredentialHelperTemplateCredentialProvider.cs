using System;
using System.Collections.Generic;
using System.Diagnostics;
using OSPSuite.Core.Services;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services.TemplateCredentials
{
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
            //git may not be installed: another provider may still have a credential
            _logger.AddDebug($"git credential helper could not be queried for '{host}': {e.Message}");
            return null;
         }
      }

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
         //Only stored credentials are of interest. Without a helper answer, git falls back to an askpass program (GIT_ASKPASS,
         //core.askPass, SSH_ASKPASS) before honoring GIT_TERMINAL_PROMPT: it would prompt, or return the program output as credential.
         //An empty core.askPass disables that fallback once GIT_ASKPASS is out of the way
         var startInfo = new ProcessStartInfo("git", "-c core.askPass= credential fill")
         {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
         };

         startInfo.Environment.Remove("GIT_ASKPASS");
         startInfo.Environment.Remove("SSH_ASKPASS");
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

         //both streams have to be drained to avoid a dead lock, even if the error output is not used
         error.GetAwaiter().GetResult();
         return process.ExitCode == 0 ? output.GetAwaiter().GetResult() : null;
      }
   }
}
