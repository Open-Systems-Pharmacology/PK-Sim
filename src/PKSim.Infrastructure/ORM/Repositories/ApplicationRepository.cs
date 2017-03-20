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
   public class ApplicationRepository : StartableRepository<IApplicationBuilder>, IApplicationRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerToApplicationMapper _applicationMapper;
      private readonly IFlatApplicationRepository _flatApplicRepository;

      private readonly ICache<CompositeKey, IApplicationBuilder> _applicationBuilders;

      public ApplicationRepository(IFlatContainerRepository flatContainerRepository, IFlatContainerToApplicationMapper applicationMapper, IFlatApplicationRepository flatApplicRepository)
      {
         _flatContainerRepository = flatContainerRepository;
         _applicationMapper = applicationMapper;
         _flatApplicRepository = flatApplicRepository;
         _applicationBuilders = new Cache<CompositeKey, IApplicationBuilder>();
      }

      public override IEnumerable<IApplicationBuilder> All()
      {
         Start();
         return _applicationBuilders;
      }

      protected override void DoStart()
      {
         foreach (var flatApplicContainer in _flatContainerRepository.All().Where(c => string.Equals(c.Type, CoreConstants.ContainerType.Application)))
         {
            var applicationType = applicationTypeFrom(flatApplicContainer);
            if (string.IsNullOrEmpty(applicationType))
               continue;

            var applicBuilder = _applicationMapper.MapFrom(flatApplicContainer);

            //parent container of each application container is formulation!
            var formulationType = applicBuilder.ParentContainer.Name;

            _applicationBuilders.Add(new CompositeKey(applicationType, formulationType), applicBuilder);
         }
      }

      private string applicationTypeFrom(FlatContainer flatApplicContainer)
      {
         return (from flatApplication in _flatApplicRepository.All()
                 where string.Equals(flatApplication.Name, flatApplicContainer.Name)
                 select flatApplication.ApplicationType).FirstOrDefault();
      }

      public IApplicationBuilder ApplicationFrom(string applicationType, string formulationType)
      {
         Start();
         return _applicationBuilders[new CompositeKey(applicationType, formulationType)];
      }
   }
}