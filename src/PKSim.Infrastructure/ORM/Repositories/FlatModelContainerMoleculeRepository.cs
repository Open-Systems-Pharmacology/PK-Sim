using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatModelContainerMoleculeRepository : IMetaDataRepository<FlatModelContainerMolecule>
   {
   }

   public class FlatModelContainerMoleculeRepository : MetaDataRepository<FlatModelContainerMolecule>, IFlatModelContainerMoleculeRepository
   {
      public FlatModelContainerMoleculeRepository(IDbGateway dbGateway,
                                          IDataTableToMetaDataMapper<FlatModelContainerMolecule> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewModelContainerMolecules)
      {
      }

   }
}
