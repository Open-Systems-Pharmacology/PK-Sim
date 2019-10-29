using PKSim.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public class DataNamingService 
   {
      private readonly IWithIdRepository _withIdRepository;

      public DataNamingService(IWithIdRepository withIdRepository)
      {
         _withIdRepository = withIdRepository;
      }

      public string GetTimeName()
      {
         return "Time";
      }

      public string GetNewRepositoryName()
      {
         return PKSimConstants.UI.SimulationResults;
      }

      public string GetEntityName(string id)
      {
         if (!_withIdRepository.ContainsObjectWithId(id))
            return id;

         var entity = _withIdRepository.Get<IObjectBase>(id);
         return entity.Name;
      }
   }
}