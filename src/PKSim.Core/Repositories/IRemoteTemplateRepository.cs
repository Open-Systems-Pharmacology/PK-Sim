using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IRemoteTemplateRepository : IStartableRepository<Template>
   {
      string Version { get; }
      IReadOnlyList<Template> AllTemplatesFor(TemplateType templateType);
      T LoadTemplate<T>(Template template);
   }
}