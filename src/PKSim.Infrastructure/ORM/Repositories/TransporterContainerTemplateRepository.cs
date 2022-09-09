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
   public class TransporterContainerTemplateRepository : StartableRepository<TransporterContainerTemplate>, ITransporterContainerTemplateRepository
   {
      private readonly IFlatTransporterContainerTemplateRepository _flatTransporterContainerTemplateRepository;
      private readonly IFlatProteinSynonymRepository _flatProteinSynonymRepository;
      private readonly IList<TransporterContainerTemplate> _allTemplates;
      private readonly List<string> _allTransporterNames = new List<string>();

      public TransporterContainerTemplateRepository(IFlatTransporterContainerTemplateRepository flatTransporterContainerTemplateRepository,
         IFlatProteinSynonymRepository flatProteinSynonymRepository)
      {
         _flatTransporterContainerTemplateRepository = flatTransporterContainerTemplateRepository;
         _flatProteinSynonymRepository = flatProteinSynonymRepository;
         _allTemplates = new List<TransporterContainerTemplate>();
      }

      public TransportType TransportTypeFor(string speciesName, string transporterName)
      {
         var transporterTemplates = allTransporterTemplates(speciesName, transporterName).ToList();

         if (!transporterTemplates.Any())
            return TransportType.Efflux;

         return transporterTemplates[0].TransportType;
      }

      private IEnumerable<TransporterContainerTemplate> allTransporterTemplates(string speciesName, string geneName)
      {
         return All().Where(t => t.Species == speciesName).Where(t => t.IsTemplateFor(geneName));
      }

      public bool HasTransporterTemplateFor(string speciesName, string transporterName)
      {
         return allTransporterTemplates(speciesName, transporterName).Any();
      }

      public IEnumerable<TransporterContainerTemplate> TransportersFor(string speciesName, string containerName, string geneName)
      {
         return from t in allTransporterTemplates(speciesName, geneName)
            where t.OrganName.Equals(containerName)
            select t;
      }

      public IEnumerable<TransporterContainerTemplate> TransportersFor(string speciesName, string containerName)
      {
         return All().Where(t => t.Species == speciesName).Where(t => t.OrganName == containerName);
      }

      public IReadOnlyList<string> AllTransporterNames => _allTransporterNames;

      public override IEnumerable<TransporterContainerTemplate> All()
      {
         Start();
         return _allTemplates;
      }

      protected override void DoStart()
      {
         var flatTemplatesGroupByKeys = from flatTemplateBySpecies in _flatTransporterContainerTemplateRepository.All().GroupBy(x => x.Species)
            from flatTemplateByGene in flatTemplateBySpecies.GroupBy(x => x.Gene)
            from flatTemplatePerOrgan in flatTemplateByGene.GroupBy(x => x.OrganName)
            from flatTemplatePerCompartment in flatTemplatePerOrgan.GroupBy(x => x.CompartmentName)
            from flatTemplatePerMembrane in flatTemplatePerCompartment.GroupBy(x => x.MembraneLocation)
            from flatTemplatePerTransport in flatTemplatePerMembrane.GroupBy(x => x.TransportType)
            select flatTemplatePerTransport.ToList();

         flatTemplatesGroupByKeys.Each(t => _allTemplates.Add(mapFrom(t)));

         cacheTransporterNames();
      }

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

      private TransporterContainerTemplate mapFrom(IList<FlatTransporterContainerTemplate> flatTemplatesGroupByKeys)
      {
         //the enumeration contains at least one element per construction. We use the first element to initialize the template
         var flatTemplate = flatTemplatesGroupByKeys.First();

         var template = new TransporterContainerTemplate
         {
            CompartmentName = flatTemplate.CompartmentName,
            Gene = flatTemplate.Gene,
            OrganName = flatTemplate.OrganName,
            Species = flatTemplate.Species,
            TransportType = flatTemplate.TransportType,
            MembraneLocation = flatTemplate.MembraneLocation,
         };

         _flatProteinSynonymRepository.AddSynonymsTo(template);

         return template;
      }
   }
}