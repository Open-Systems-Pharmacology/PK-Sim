using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using SnapshotCurve = PKSim.Core.Snapshots.Curve;
using ModelCurve = OSPSuite.Core.Chart.Curve;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using ModelDataColumn = OSPSuite.Core.Domain.Data.DataColumn;

namespace PKSim.Core.Snapshots.Mappers
{
 
   public class CurveMapper : SnapshotMapperBase<ModelCurve, SnapshotCurve, CurveChartContext>
   {
      private readonly IDimensionFactory _dimensionFactory;

      public CurveMapper(IDimensionFactory dimensionFactory)
      {
         _dimensionFactory = dimensionFactory;
      }

      public override Task<SnapshotCurve> MapToSnapshot(ModelCurve curve)
      {
         return SnapshotFrom(curve, x =>
         {
            x.Name = SnapshotValueFor(curve.Name);
            x.X = curve.xData?.PathAsString;
            x.Y = curve.yData?.PathAsString;
            x.CurveOptions = curve.CurveOptions;
         });
      }

      public override Task<ModelCurve> MapToModel(SnapshotCurve snapshot, CurveChartContext curveChartContext)
      {
         var curve = new ModelCurve {Name = snapshot.Name};
         curve.CurveOptions.UpdateFrom(snapshot.CurveOptions);

         var yData = findCurveWithPath(snapshot.Y, curveChartContext.DataRepositories);
         curve.SetyData(yData, _dimensionFactory);

         ModelDataColumn xData = yData?.BaseGrid;
         if(!string.Equals(snapshot.X, xData?.Name))
            xData = findCurveWithPath(snapshot.X, curveChartContext.DataRepositories);

         curve.SetxData(xData, _dimensionFactory);
         return Task.FromResult(curve);
      }

      private ModelDataColumn findCurveWithPath(string path, IReadOnlyList<ModelDataRepository> dataRepositories)
      {
         return dataRepositories.SelectMany(x => x.Columns).FirstOrDefault(x => string.Equals(x.QuantityInfo.PathAsString, path));
      }
   }
}