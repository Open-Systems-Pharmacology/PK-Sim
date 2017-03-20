using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatRelativeObjectPathRepository : IMetaDataRepository<FlatRelativeObjectPath>
   {
      FlatRelativeObjectPath FlatRelativeObjectPathFor(long pathId);
   }

   public class FlatRelativeObjectPathRepository : MetaDataRepository<FlatRelativeObjectPath>, IFlatRelativeObjectPathRepository
   {
      private readonly ICache<long, FlatRelativeObjectPath> _flatObjectPaths;

      public FlatRelativeObjectPathRepository(IDbGateway dbGateway,
         IDataTableToMetaDataMapper<FlatRelativeObjectPath> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewObjectPaths)
      {
         _flatObjectPaths = new Cache<long, FlatRelativeObjectPath>();
      }

      protected override void PerformPostStartProcessing()
      {
         foreach (var flatObjectPath in AllElements())
         {
            _flatObjectPaths.Add(flatObjectPath.PathId, flatObjectPath);
         }
      }

      public FlatRelativeObjectPath FlatRelativeObjectPathFor(long pathId)
      {
         Start();
         return _flatObjectPaths[pathId];
      }
   }
}