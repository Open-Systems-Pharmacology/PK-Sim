using System.Collections.Generic;
using OSPSuite.Utility.Collections;
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

      public RemoteTemplateRepository(IPKSimConfiguration configuration, IJsonSerializer jsonSerializer)
      {
         _configuration = configuration;
         _jsonSerializer = jsonSerializer;
      }

      protected override void DoStart()
      {
         var snapshots = _jsonSerializer.Deserialize<RemoteTemplates>(_configuration.RemoteTemplateSummaryPath).Result;
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