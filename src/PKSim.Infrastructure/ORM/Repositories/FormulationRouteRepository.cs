using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFormulationRouteRepository : IMetaDataRepository<FormulationRoute>
   {
   }

   public class FormulationRouteRepository : MetaDataRepository<FormulationRoute>, IFormulationRouteRepository
   {
      public FormulationRouteRepository(IDbGateway dbGateway,IDataTableToMetaDataMapper<FormulationRoute> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_FORMULATION_ROUTES)
      {
      }
   }
}