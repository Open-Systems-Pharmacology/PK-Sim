using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class RemoteTemplateRepository : StartableRepository<Template>, IRemoteTemplateRepository
   {
      private readonly IPKSimConfiguration _configuration;
      private readonly IJsonSerializer _jsonSerializer;
      private readonly List<Template> _allTemplates = new List<Template>();
      public string Version { get; private set; }

      public IReadOnlyList<Template> AllTemplatesFor(TemplateType templateType)
      {
         Start();
         return _allTemplates.Where(x => x.Type.Is(templateType)).ToList();
      }

      public T LoadTemplate<T>(Template template)
      {
         throw new NotImplementedException();
      }

      public RemoteTemplateRepository(IPKSimConfiguration configuration, IJsonSerializer jsonSerializer)
      {
         _configuration = configuration;
         _jsonSerializer = jsonSerializer;
      }

      protected override void DoStart()
      {
         var snapshots = Task.Run(() => _jsonSerializer.Deserialize<RemoteTemplates>(_configuration.RemoteTemplateSummaryPath)).Result;
         snapshots.Templates.Each(x => x.DatabaseType = TemplateDatabaseType.Remote);
         _allTemplates.AddRange(snapshots.Templates);
         Version = snapshots.Version;
      }

      public override IEnumerable<Template> All()
      {
         Start();
         return _allTemplates;
      }
   }
}