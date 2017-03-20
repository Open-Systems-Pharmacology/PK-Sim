using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ApplicationTransportRepository : StartableRepository<PKSimTransport>, IApplicationTransportRepository
   {
      private readonly IFlatProcessRepository _flatProcessesRepo;
      private readonly IFlatProcessToPassiveTransportMapper _passiveTransportMapper;
      private readonly IFlatApplicationProcessRepository _flatApplicationProcessRepo;
      private readonly IList<PKSimTransport> _applicationTransports = new List<PKSimTransport>();

      public ApplicationTransportRepository(IFlatProcessRepository flatProcessesRepo,
         IFlatProcessToPassiveTransportMapper passiveTransportMapper,
         IFlatApplicationProcessRepository flatApplicationProcessRepo)
      {
         _flatProcessesRepo = flatProcessesRepo;
         _passiveTransportMapper = passiveTransportMapper;
         _flatApplicationProcessRepo = flatApplicationProcessRepo;
      }

      public override IEnumerable<PKSimTransport> All()
      {
         Start();
         return _applicationTransports;
      }

      protected override void DoStart()
      {
         foreach (var flatProc in _flatProcessesRepo.All().Where(processIsApplication))
         {
            _applicationTransports.Add(_passiveTransportMapper.MapFrom(flatProc));
         }
      }

      public IEnumerable<PKSimTransport> TransportsFor(string applicationName)
      {
         var applicProcessNames = _flatApplicationProcessRepo.ProcessNamesFor(applicationName);

         return All().Where(proc => applicProcessNames.Contains(proc.Name));
      }

      private bool processIsApplication(FlatProcess process)
      {
         return process.ProcessType == CoreConstants.ProcessType.APPLICATION;
      }
   }
}