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
   public class PassiveTransportRepository : StartableRepository<PKSimTransport>, IPassiveTransportRepository
   {
      private readonly IFlatProcessRepository _flatProcessesRepo;
      private readonly IFlatProcessToPassiveTransportMapper _passiveTransportMapper;
      private readonly IFlatModelProcessRepository _modelProcessRepo;
      private readonly IList<PKSimTransport> _passiveTransports = new List<PKSimTransport>();

      public PassiveTransportRepository(IFlatProcessRepository flatProcessesRepo,
                                        IFlatProcessToPassiveTransportMapper passiveTransportMapper,
                                        IFlatModelProcessRepository modelProcessRepo)
      {
         _flatProcessesRepo = flatProcessesRepo;
         _passiveTransportMapper = passiveTransportMapper;
         _modelProcessRepo = modelProcessRepo;
      }

      public override IEnumerable<PKSimTransport> All()
      {
         Start();
         return _passiveTransports;
      }

      protected override void DoStart()
      {
         foreach (var flatProc in _flatProcessesRepo.All().Where(processIsPassiveTransport))
         {
            _passiveTransports.Add(_passiveTransportMapper.MapFrom(flatProc));
         }
      }

      private bool processIsPassiveTransport(FlatProcess process)
      {
         return (process.ProcessType == CoreConstants.ProcessType.PASSIVE) && 
                 process.ActionType.Equals(ProcessActionType.Transport);
      }

      public IEnumerable<PKSimTransport> AllFor(ModelProperties modelProperties)
      {
         Start();

         return from passiveTransport in All()
                where _modelProcessRepo.Contains(modelProperties.ModelConfiguration.ModelName, passiveTransport.Name)
                where modelProperties.ContainsCalculationMethod(passiveTransport.CalculationMethod)
                select passiveTransport;
      }
   }
}