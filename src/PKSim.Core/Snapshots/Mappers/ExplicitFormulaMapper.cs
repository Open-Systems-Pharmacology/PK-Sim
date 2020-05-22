using System.Threading.Tasks;
using SnapshotExplicitFormula = PKSim.Core.Snapshots.ExplicitFormula;
using ModelExplicitFormula = OSPSuite.Core.Domain.Formulas.ExplicitFormula;


namespace PKSim.Core.Snapshots.Mappers
{
   public class ExplicitFormulaMapper : ObjectBaseSnapshotMapperBase<ModelExplicitFormula, SnapshotExplicitFormula>
   {
      public override Task<SnapshotExplicitFormula> MapToSnapshot(ModelExplicitFormula model)
      {
         throw new System.NotImplementedException();
      }

      public override Task<ModelExplicitFormula> MapToModel(SnapshotExplicitFormula snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}