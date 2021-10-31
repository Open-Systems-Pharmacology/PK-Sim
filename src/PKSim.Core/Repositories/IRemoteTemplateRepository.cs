using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IRemoteTemplateRepository : IStartableRepository<RemoteTemplate>
   {
      string Version { get; }
      IReadOnlyList<RemoteTemplate> AllTemplatesFor(TemplateType templateType);
      Task<T> LoadTemplateAsync<T>(RemoteTemplate remoteTemplate);
      Task<IReadOnlyList<RemoteTemplate>> AllReferenceTemplatesForAsync<T>(T loadedTemplate);
   }
}