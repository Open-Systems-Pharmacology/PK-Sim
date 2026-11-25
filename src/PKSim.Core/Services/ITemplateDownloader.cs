using System;
using System.Threading.Tasks;

namespace PKSim.Core.Services
{
   public interface ITemplateDownloader
   {
      Task DownloadAsync(Uri uri, string destination, bool allowInteractive = false);
   }
}
