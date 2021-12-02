using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatMoleculeRepository : IMetaDataRepository<FlatMolecule>
   {
      FlatMolecule FindBy(QuantityType moleculeType);
   }

   public class FlatMoleculeRepository : MetaDataRepository<FlatMolecule>, IFlatMoleculeRepository
   {
      public FlatMoleculeRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatMolecule> mapper) : base(dbGateway, mapper,
         CoreConstants.ORM.VIEW_MOLECULES)
      {
      }

      public FlatMolecule FindBy(QuantityType moleculeType)
      {
         Start();
         return All().First(x => x.MoleculeType == moleculeType);
      }
   }
}