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
      RemoteTemplate TemplateBy(TemplateType templateType, string name);
      Task<T> LoadTemplateAsync<T>(RemoteTemplate remoteTemplate);
      IReadOnlyList<RemoteTemplate> AllReferenceTemplatesFor<T>(RemoteTemplate remoteTemplate, T loadedTemplate);
   }
}