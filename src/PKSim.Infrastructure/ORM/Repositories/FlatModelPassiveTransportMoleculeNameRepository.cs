using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatModelPassiveTransportMoleculeNameRepository : IMetaDataRepository<FlatModelPassiveTransportMoleculeName>
   {
   }

   public class FlatModelPassiveTransportMoleculeNameRepository : MetaDataRepository<FlatModelPassiveTransportMoleculeName>, IFlatModelPassiveTransportMoleculeNameRepository
   {
      public FlatModelPassiveTransportMoleculeNameRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatModelPassiveTransportMoleculeName> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_MODEL_PASSIVE_TRANSPORT_MOLECULE_NAMES)
      {
      }
   }
}