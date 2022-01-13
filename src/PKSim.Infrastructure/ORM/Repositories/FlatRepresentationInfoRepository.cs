using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
    public interface IFlatRepresentationInfoRepository : IMetaDataRepository<RepresentationInfo>
    {
    }

    public class FlatRepresentationInfoRepository : MetaDataRepository<RepresentationInfo>, IFlatRepresentationInfoRepository
    {
       public FlatRepresentationInfoRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<RepresentationInfo> mapper) :
            base(dbGateway, mapper, CoreConstants.ORM.VIEW_REPRESENTATION_INFOS)
        {
        }
    }
}