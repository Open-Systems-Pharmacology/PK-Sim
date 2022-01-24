using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatMoleculeParameterRepository : IMetaDataRepository<FlatMoleculeParameter>
   {
   }

   public class FlatMoleculeParameterRepository : MetaDataRepository<FlatMoleculeParameter>, IFlatMoleculeParameterRepository
   {
      public FlatMoleculeParameterRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatMoleculeParameter> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_MOLECULE_PARAMETERS)
      {
      }
   }
}