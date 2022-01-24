using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatDiseaseStateRepository : IMetaDataRepository<FlatDiseaseState>
   {
   }

   public class FlatDiseaseStateRepository : MetaDataRepository<FlatDiseaseState>, IFlatDiseaseStateRepository
   {
      public FlatDiseaseStateRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatDiseaseState> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_DISEASE_STATES)
      {
      }
   }

   public interface IFlatPopulationDiseaseStateRepository : IMetaDataRepository<FlatPopulationDiseaseState>
   {
   }

   public class FlatPopulationDiseaseStateRepository : MetaDataRepository<FlatPopulationDiseaseState>, IFlatPopulationDiseaseStateRepository
   {
      public FlatPopulationDiseaseStateRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatPopulationDiseaseState> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_POPULATION_DISEASE_STATES)
      {
      }
   }
}