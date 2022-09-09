using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class TransporterTemplateRepository : StartableRepository<TransporterTemplate>, ITransporterTemplateRepository
   {
      private readonly IFlatTransporterTemplateRepository _flatTransporterTemplateRepository;
      private readonly IFlatProteinSynonymRepository _flatProteinSynonymRepository;
      private readonly List<TransporterTemplate> _allTemplates = new List<TransporterTemplate>();
      private readonly List<string> _allTransporterNames = new List<string>();

      public TransporterTemplateRepository(
         IFlatTransporterTemplateRepository flatTransporterTemplateRepository,
         IFlatProteinSynonymRepository flatProteinSynonymRepository)
      {
         _flatTransporterTemplateRepository = flatTransporterTemplateRepository;
         _flatProteinSynonymRepository = flatProteinSynonymRepository;
      }

      protected override void DoStart()
      {
         _flatTransporterTemplateRepository.All().Each(t => _allTemplates.Add(mapFrom(t)));
         cacheTransporterNames();
      }

      public override IEnumerable<TransporterTemplate> All()
      {
         Start();
         return _allTemplates;
      }

      public TransportType TransportTypeFor(string speciesName, string transporterName)
      {
         var transporterTemplate = TransporterTemplateFor(speciesName, transporterName);
         return transporterTemplate?.TransportType ?? TransportType.Efflux;
      }

      public TransporterTemplate TransporterTemplateFor(string speciesName, string transporterName)
      {
         Start();
         return _allTemplates.Find(x => x.Species == speciesName && x.IsTemplateFor(transporterName));
      }

      public bool HasTransporterTemplateFor(string speciesName, string transporterName) =>
         TransporterTemplateFor(speciesName, transporterName) != null;

      private TransporterTemplate mapFrom(FlatTransporterTemplate flatTemplate)
      {
         var template = new TransporterTemplate
         {
            Gene = flatTemplate.Gene,
            Species = flatTemplate.Species,
            TransportType = flatTemplate.TransportType,
         };

         _flatProteinSynonymRepository.AddSynonymsTo(template);

         return template;
      }

      public IReadOnlyList<string> AllTransporterNames => _allTransporterNames;

      private void cacheTransporterNames()
      {
         var transporterNames = new List<string>();
         _allTemplates.Each(x =>
         {
            transporterNames.Add(x.Name);
            transporterNames.AddRange(x.Synonyms);
         });
         _allTransporterNames.AddRange(transporterNames.Distinct());
      }
   }
}