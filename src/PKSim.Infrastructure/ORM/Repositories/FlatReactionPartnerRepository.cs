using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatReactionPartnerRepository : IMetaDataRepository<FlatReactionPartner>
   {
   }

   public class FlatReactionPartnerRepository : MetaDataRepository<FlatReactionPartner>, IFlatReactionPartnerRepository
   {
      public FlatReactionPartnerRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatReactionPartner> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewReactionPartners)
      {
      }
   }

}
