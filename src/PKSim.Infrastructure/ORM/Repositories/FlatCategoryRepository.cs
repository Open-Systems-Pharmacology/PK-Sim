using System.Linq;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatCategoryRepository : IMetaDataRepository<FlatCategory>
   {
      FlatCategory FindBy(string name);
   }

   public class FlatCategoryRepository : MetaDataRepository<FlatCategory>, IFlatCategoryRepository
   {
      public FlatCategoryRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatCategory> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewCategory)
      {
      }

      public FlatCategory FindBy(string name)
      {
         return All().FirstOrDefault(x => string.Equals(x.Name, name));
      }
   }
}