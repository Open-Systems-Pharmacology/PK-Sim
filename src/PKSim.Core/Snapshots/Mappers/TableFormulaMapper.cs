using System.Linq;
using OSPSuite.Core.Domain.Formulas;
using SnapshotTableFormula = PKSim.Core.Snapshots.TableFormula;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.TableFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class TableFormulaMapper : SnapshotMapperBase<ModelTableFormula, SnapshotTableFormula>
   {
      public override SnapshotTableFormula MapToSnapshot(ModelTableFormula tableFormula)
      {
         return new SnapshotTableFormula
         {
            XDimension = tableFormula.XDimension.Name,
            XUnit = tableFormula.XDisplayUnit.Name,
            XName = tableFormula.XName,

            YDimension = tableFormula.Dimension.Name,
            YUnit = tableFormula.YDisplayUnit.Name,
            YName = tableFormula.YName,

            Points = tableFormula.AllPoints().Select(p => snapshotPointFor(tableFormula, p)).ToList(),
         };
      }

      private static Point snapshotPointFor(ModelTableFormula tableFormula, ValuePoint p) => new Point
      {
         X = tableFormula.XDisplayValueFor(p.X),
         Y = tableFormula.YDisplayValueFor(p.Y),
         RestartSolver = p.RestartSolver
      };
   }
}