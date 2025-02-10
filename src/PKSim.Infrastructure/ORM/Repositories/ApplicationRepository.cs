using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ApplicationRepository : StartableRepository<ApplicationBuilder>, IApplicationRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerToApplicationMapper _applicationMapper;
      private readonly IFlatApplicationRepository _flatApplicationRepository;

      private readonly ICache<CompositeKey, ApplicationBuilder> _applicationBuilders;

      public ApplicationRepository(
         IFlatContainerRepository flatContainerRepository, 
         IFlatContainerToApplicationMapper applicationMapper, 
         IFlatApplicationRepository flatApplicationRepository)
      {
         _flatContainerRepository = flatContainerRepository;
         _applicationMapper = applicationMapper;
         _flatApplicationRepository = flatApplicationRepository;
         _applicationBuilders = new Cache<CompositeKey, ApplicationBuilder>();
      }

      public override IEnumerable<ApplicationBuilder> All()
      {
         Start();
         return _applicationBuilders;
      }

      protected override void DoStart()
      {
         foreach (var flatApplicationContainer in _flatContainerRepository.All().Where(c => string.Equals(c.Type, CoreConstants.ContainerType.APPLICATION)))
         {
            var applicationType = applicationTypeFrom(flatApplicationContainer);
            if (string.IsNullOrEmpty(applicationType))
               continue;

            var applicationBuilder = _applicationMapper.MapFrom(flatApplicationContainer);

            //parent container of each application container is formulation!
            var formulationType = applicationBuilder.ParentContainer.Name;

            _applicationBuilders.Add(new CompositeKey(applicationType, formulationType), applicationBuilder);
         }
      }

      private string applicationTypeFrom(FlatContainer flatApplicationContainer)
      {
         return (from flatApplication in _flatApplicationRepository.All()
                 where string.Equals(flatApplication.Name, flatApplicationContainer.Name)
                 select flatApplication.ApplicationType).FirstOrDefault();
      }

      public ApplicationBuilder ApplicationFrom(string applicationType, string formulationType)
      {
         Start();
         return _applicationBuilders[new CompositeKey(applicationType, formulationType)];
      }
   }
}