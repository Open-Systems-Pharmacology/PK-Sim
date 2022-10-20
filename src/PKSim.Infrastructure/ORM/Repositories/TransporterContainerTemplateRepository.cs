using System.Collections.Generic;
using System.Linq;
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
      private readonly ITransporterTemplateRepository _transporterTemplateRepository;
      private readonly IList<TransporterContainerTemplate> _allTemplates;

      public TransporterContainerTemplateRepository(
         IFlatTransporterContainerTemplateRepository flatTransporterContainerTemplateRepository,
         ITransporterTemplateRepository transporterTemplateRepository
      )
      {
         _flatTransporterContainerTemplateRepository = flatTransporterContainerTemplateRepository;
         _transporterTemplateRepository = transporterTemplateRepository;
         _allTemplates = new List<TransporterContainerTemplate>();
      }

      private IEnumerable<TransporterContainerTemplate> allTransporterTemplates(string speciesName, string geneName)
      {
         var transporterTemplate = _transporterTemplateRepository.TransporterTemplateFor(speciesName, geneName);
         if (transporterTemplate == null)
            return Enumerable.Empty<TransporterContainerTemplate>();

         return All().Where(t => t.Species == speciesName && t.Gene == transporterTemplate.Gene);
      }

      public TransporterContainerTemplate TransporterContainerTemplateFor(string speciesName, string containerName, string geneName)
      {
         return allTransporterTemplates(speciesName, geneName).FirstOrDefault(t => t.ContainerName.Equals(containerName));
      }

      public override IEnumerable<TransporterContainerTemplate> All()
      {
         Start();
         return _allTemplates;
      }

      protected override void DoStart()
      {
         _flatTransporterContainerTemplateRepository.All().Each(t => _allTemplates.Add(mapFrom(t)));
      }

      private TransporterContainerTemplate mapFrom(FlatTransporterContainerTemplate flatContainerTemplate)
      {
         return new TransporterContainerTemplate
         {
            Gene = flatContainerTemplate.Gene,
            ContainerName = flatContainerTemplate.ContainerName,
            Species = flatContainerTemplate.Species,
            TransportType = flatContainerTemplate.TransportType,
            MembraneLocation = flatContainerTemplate.MembraneLocation,
         };
      }
   }
}