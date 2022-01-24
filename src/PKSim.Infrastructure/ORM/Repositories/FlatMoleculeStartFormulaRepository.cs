using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatMoleculeStartFormulaRepository : IMetaDataRepository<FlatMoleculeStartFormula>
   {
   }

   public class FlatMoleculeStartFormulaRepository : MetaDataRepository<FlatMoleculeStartFormula>, IFlatMoleculeStartFormulaRepository
   {
      public FlatMoleculeStartFormulaRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatMoleculeStartFormula> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_MOLECULE_START_FORMULAS)
      {
      }
   }
}
