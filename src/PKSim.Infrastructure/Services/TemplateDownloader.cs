using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class TemplateDownloader : ITemplateDownloader, IDisposable
   {
      private const string USER_AGENT = "PK-Sim";
      private static readonly TimeSpan _timeout = TimeSpan.FromMinutes(2);
      private readonly IContainer _container;
      private readonly IOSPSuiteLogger _logger;
      private readonly HttpClient _httpClient;
      private readonly ConcurrentDictionary<string, TemplateCredential> _sessionCredentials = new ConcurrentDictionary<string, TemplateCredential>(StringComparer.OrdinalIgnoreCase);
      private IReadOnlyList<ITemplateCredentialProvider> _credentialProviders;

      public TemplateDownloader(IContainer container, IOSPSuiteLogger logger) : this(container, logger, new HttpClientHandler())
      {
      }

      protected TemplateDownloader(IContainer container, IOSPSuiteLogger logger, HttpMessageHandler messageHandler)
      {
         _container = container;
         _logger = logger;
         //HttpClient.Timeout stops at the response headers. Each attempt uses its own cancellation token that also covers the body
         _httpClient = new HttpClient(messageHandler, disposeHandler: true) {Timeout = Timeout.InfiniteTimeSpan};
         _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(USER_AGENT);
      }

      public async Task DownloadAsync(Uri uri, string destination, bool allowInteractive = false)
      {
         try
         {
            if (uri.IsFile)
               FileHelper.Copy(uri.LocalPath, destination);
            else
               await downloadHttpAsync(uri, destination, allowInteractive);
         }
         catch (Exception e)
         {
            //a failed download may leave an empty file behind
            FileHelper.DeleteFile(destination);

            if (e is OSPSuiteException)
               throw;

            throw new OSPSuiteException(PKSimConstants.Error.CannotDownloadTemplateLocatedAt(uri.ToString(), e.Message), e);
         }
      }

      private async Task downloadHttpAsync(Uri uri, string destination, bool allowInteractive)
      {
         //basic and bearer authentication are readable by anyone on the wire without https
         var canSendCredentials = uri.Scheme == Uri.UriSchemeHttps;

         if (canSendCredentials && _sessionCredentials.TryGetValue(uri.Host, out var sessionCredential))
         {
            if (await tryDownloadAsync(uri, destination, sessionCredential) == HttpStatusCode.OK)
               return;

            _sessionCredentials.TryRemove(uri.Host, out _);
         }

         var anonymousStatus = await tryDownloadAsync(uri, destination, credential: null);
         if (anonymousStatus == HttpStatusCode.OK)
            return;

         if (!requiresAuthentication(uri, anonymousStatus))
            throw new OSPSuiteException(PKSimConstants.Error.CannotDownloadTemplateLocatedAt(uri.ToString(), statusDescription(anonymousStatus)));

         if (!canSendCredentials)
            throw new OSPSuiteException(PKSimConstants.Error.TemplateCredentialsRequireHttps(uri.ToString()));

         foreach (var provider in credentialProviders(allowInteractive))
         {
            var credential = provider.CredentialFor(uri);
            if (credential == null)
               continue;

            _logger.AddDebug($"Authenticating to '{uri.Host}' with a credential from {provider.GetType().Name}");
            var status = await tryDownloadAsync(uri, destination, credential);
            if (status != HttpStatusCode.OK)
               continue;

            _sessionCredentials[uri.Host] = credential;
            return;
         }

         throw new OSPSuiteException(PKSimConstants.Error.TemplateRequiresAuthentication(uri.ToString(), statusDescription(anonymousStatus)));
      }

      private async Task<HttpStatusCode> tryDownloadAsync(Uri uri, string destination, TemplateCredential credential)
      {
         using (var timeout = new CancellationTokenSource(_timeout))
         using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
         {
            if (credential != null)
               request.Headers.Authorization = authorizationHeaderFor(credential);

            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token))
            {
               if (!response.IsSuccessStatusCode)
                  return response.StatusCode;

               using (var contentStream = await response.Content.ReadAsStreamAsync(timeout.Token))
               using (var fileStream = File.Create(destination))
               {
                  await contentStream.CopyToAsync(fileStream, timeout.Token);
               }

               return HttpStatusCode.OK;
            }
         }
      }

      private static bool requiresAuthentication(Uri uri, HttpStatusCode statusCode)
      {
         switch (statusCode)
         {
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Forbidden:
               return true;
            case HttpStatusCode.NotFound:
               //GitHub answers 404 instead of 401 for private content. Other hosts must not trigger a credential prompt for a wrong url
               return TemplateCredentialHost.IsGitHubHost(uri);
            default:
               return false;
         }
      }

      private static string statusDescription(HttpStatusCode statusCode) => $"{(int) statusCode} {statusCode}";

      private static AuthenticationHeaderValue authorizationHeaderFor(TemplateCredential credential)
      {
         if (!credential.HasUserName)
            return new AuthenticationHeaderValue("Bearer", credential.Secret);

         var basicToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credential.UserName}:{credential.Secret}"));
         return new AuthenticationHeaderValue("Basic", basicToken);
      }

      private IEnumerable<ITemplateCredentialProvider> credentialProviders(bool allowInteractive)
      {
         _credentialProviders ??= _container.ResolveAll<ITemplateCredentialProvider>().OrderBy(x => x.Priority).ToList();
         return _credentialProviders.Where(x => allowInteractive || !x.IsInteractive);
      }

      public void Dispose() => _httpClient.Dispose();
   }
}
