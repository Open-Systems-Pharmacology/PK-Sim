using System.Linq;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using SnapshotTableFormula = PKSim.Core.Snapshots.TableFormula;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.TableFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class TableFormulaMapper : SnapshotMapperBase<ModelTableFormula, SnapshotTableFormula>
   {
      private readonly IFormulaFactory _formulaFactory;
      private readonly IDimensionRepository _dimensionRepository;

      public TableFormulaMapper(IFormulaFactory formulaFactory, IDimensionRepository dimensionRepository)
      {
         _formulaFactory = formulaFactory;
         _dimensionRepository = dimensionRepository;
      }

      public override SnapshotTableFormula MapToSnapshot(ModelTableFormula tableFormula)
      {
         return new SnapshotTableFormula
         {
            XDimension = tableFormula.XDimension.Name,
            XUnit = SnapshotValueFor(tableFormula.XDisplayUnit.Name),
            XName = tableFormula.XName,

            YDimension = tableFormula.Dimension.Name,
            YUnit = SnapshotValueFor(tableFormula.YDisplayUnit.Name),
            YName = tableFormula.YName,

            Points = tableFormula.AllPoints().Select(p => snapshotPointFor(tableFormula, p)).ToList(),
         };
      }

      public override ModelTableFormula MapToModel(SnapshotTableFormula snapshotTableFormula)
      {
         var tableFormula = _formulaFactory.CreateTableFormula();
         tableFormula.XDimension = _dimensionRepository.DimensionByName(snapshotTableFormula.XDimension);
         tableFormula.XDisplayUnit = tableFormula.XDimension.Unit(UnitValueFor(snapshotTableFormula.XUnit));
         tableFormula.XName = snapshotTableFormula.XName;

         tableFormula.Dimension = _dimensionRepository.DimensionByName(snapshotTableFormula.YDimension);
         tableFormula.YDisplayUnit = tableFormula.Dimension.Unit(UnitValueFor(snapshotTableFormula.YUnit));
         tableFormula.YName = snapshotTableFormula.YName;

         snapshotTableFormula.Points.Each(p => tableFormula.AddPoint(valuePointFrom(tableFormula, p)));
         return tableFormula;
      }

      private ValuePoint valuePointFrom(ModelTableFormula tableFormula, Point point)
      {
         var x = tableFormula.XBaseValueFor(point.X);
         var y = tableFormula.YBaseValueFor(point.Y);
         return new ValuePoint(x, y) {RestartSolver = point.RestartSolver};
      }

      private static Point snapshotPointFor(ModelTableFormula tableFormula, ValuePoint p) => new Point
      {
         X = tableFormula.XDisplayValueFor(p.X),
         Y = tableFormula.YDisplayValueFor(p.Y),
         RestartSolver = p.RestartSolver
      };
   }
}