using System;
using System.Threading.Tasks;

namespace PKSim.Core.Services
{
   public interface ITemplateDownloader
   {
      /// <summary>
      ///    Downloads the resource located at <paramref name="uri" /> to <paramref name="destination" />.
      ///    File uris are copied. Http(s) resources are first requested anonymously. If the server refuses, the credentials
      ///    returned by the registered <see cref="ITemplateCredentialProvider" /> are tried in turn. Interactive providers are
      ///    only used when <paramref name="allowInteractive" /> is true. A credential accepted by a host is reused for the
      ///    duration of the session but never persisted.
      /// </summary>
      /// <exception cref="OSPSuite.Utility.Exceptions.OSPSuiteException">if the resource could not be downloaded</exception>
      Task DownloadAsync(Uri uri, string destination, bool allowInteractive = false);
   }
}
