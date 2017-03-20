using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class TransporterContainerTemplateRepository : StartableRepository<TransporterContainerTemplate>, ITransporterContainerTemplateRepository
   {
      private readonly IFlatTransporterContainerTemplateRepository _flatTransporterContainerTemplateRepository;
      private readonly IFlatProteinSynonymRepository _flatProteinSynonymRepository;
      private readonly IList<TransporterContainerTemplate> _allTemplates;

      public TransporterContainerTemplateRepository(IFlatTransporterContainerTemplateRepository flatTransporterContainerTemplateRepository, IFlatProteinSynonymRepository flatProteinSynonymRepository)
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
         return from t in All()
            where t.Species.Equals(speciesName)
            where t.IsTemplateFor(geneName)
            select t;
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
         return TransportersFor(speciesName, containerName, CoreConstants.ActiveTransport.VALID_FOR_ALL_GENES);
      }

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
      }

      private TransporterContainerTemplate mapFrom(IList<FlatTransporterContainerTemplate> flatTemplatesGroupByKeys)
      {
         //the enumeration contains at least one element per construction. We use the first element to initialize the template
         var flatTemplate = flatTemplatesGroupByKeys.First();

         var template = new TransporterContainerTemplate
         {
            CompartmentName = flatTemplate.CompartmentName,
            Gene = flatTemplate.Gene,
            MembraneLocation = flatTemplate.MembraneLocation,
            MembraneLocationDisplayName = flatTemplate.MembraneLocationDisplayName,
            OrganName = flatTemplate.OrganName,
            Species = flatTemplate.Species,
            TransportType = flatTemplate.TransportType
         };

         flatTemplatesGroupByKeys.Select(x => x.ProcessName).Each(template.AddProcessName);
         _flatProteinSynonymRepository.AddSynonymsTo(template);

         return template;
      }
   }
}