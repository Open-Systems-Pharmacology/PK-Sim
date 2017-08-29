using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Repositories;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using SnapshotTableFormula = PKSim.Core.Snapshots.TableFormula;
using ModelTableFormula = OSPSuite.Core.Domain.Formulas.TableFormula;

namespace PKSim.Core.Snapshots.Mappers
{
   public class TableFormulaMapper : ObjectBaseSnapshotMapperBase<ModelTableFormula, SnapshotTableFormula>
   {
      private readonly IFormulaFactory _formulaFactory;
      private readonly IDimensionRepository _dimensionRepository;

      public TableFormulaMapper(IFormulaFactory formulaFactory, IDimensionRepository dimensionRepository)
      {
         _formulaFactory = formulaFactory;
         _dimensionRepository = dimensionRepository;
      }

      public override Task<SnapshotTableFormula> MapToSnapshot(ModelTableFormula tableFormula)
      {
         return SnapshotFrom(tableFormula, snapshot =>
         {
            UpdateSnapshotProperties(snapshot, tableFormula);
         });
      }

      public override Task<ModelTableFormula> MapToModel(SnapshotTableFormula snapshotTableFormula)
      {
         var tableFormula = _formulaFactory.CreateTableFormula();
         UpdateModelProperties(tableFormula, snapshotTableFormula);
         return Task.FromResult(tableFormula);
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

      public virtual void UpdateModelProperties(ModelTableFormula tableFormula, SnapshotTableFormula snapshotTableFormula)
      {
         tableFormula.XDimension = _dimensionRepository.DimensionByName(snapshotTableFormula.XDimension);
         tableFormula.XDisplayUnit = tableFormula.XDimension.Unit(ModelValueFor(snapshotTableFormula.XUnit));
         tableFormula.XName = snapshotTableFormula.XName;

         tableFormula.Dimension = _dimensionRepository.DimensionByName(snapshotTableFormula.YDimension);
         tableFormula.YDisplayUnit = tableFormula.Dimension.Unit(ModelValueFor(snapshotTableFormula.YUnit));
         tableFormula.YName = snapshotTableFormula.YName;

         snapshotTableFormula.UseDerivedValues = tableFormula.UseDerivedValues;

         snapshotTableFormula.Points.Each(p => tableFormula.AddPoint(valuePointFrom(tableFormula, p)));
      }

      public virtual void UpdateSnapshotProperties(SnapshotTableFormula snapshot, ModelTableFormula tableFormula)
      {
         snapshot.XDimension = tableFormula.XDimension.Name;
         snapshot.XUnit = SnapshotValueFor(tableFormula.XDisplayUnit.Name);
         snapshot.XName = tableFormula.XName;

         snapshot.YDimension = tableFormula.Dimension.Name;
         snapshot.YUnit = SnapshotValueFor(tableFormula.YDisplayUnit.Name);
         snapshot.YName = tableFormula.YName;

         snapshot.UseDerivedValues = tableFormula.UseDerivedValues;

         snapshot.Points = tableFormula.AllPoints().Select(p => snapshotPointFor(tableFormula, p)).ToList();
      }
   }
}