using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using PKSim.Assets;
using SnapshotCurve = PKSim.Core.Snapshots.Curve;
using ModelCurve = OSPSuite.Core.Chart.Curve;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;
using ModelDataColumn = OSPSuite.Core.Domain.Data.DataColumn;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CurveMapper : SnapshotMapperBase<ModelCurve, SnapshotCurve, SimulationAnalysisContext>
   {
      private readonly CurveOptionsMapper _curveOptionsMapper;
      private readonly IDimensionFactory _dimensionFactory;
      private readonly ILogger _logger;

      public CurveMapper(CurveOptionsMapper curveOptionsMapper, IDimensionFactory dimensionFactory, ILogger logger)
      {
         _curveOptionsMapper = curveOptionsMapper;
         _dimensionFactory = dimensionFactory;
         _logger = logger;
      }

      public override async Task<SnapshotCurve> MapToSnapshot(ModelCurve curve)
      {
         var snapshot = await SnapshotFrom(curve, x =>
         {
            x.Name = SnapshotValueFor(curve.Name);
            x.X = curve.xData?.PathAsString;
            x.Y = curve.yData?.PathAsString;
         });
         snapshot.CurveOptions = await _curveOptionsMapper.MapToSnapshot(curve.CurveOptions);
         return snapshot;
      }

      public override async Task<ModelCurve> MapToModel(SnapshotCurve snapshot, SimulationAnalysisContext simulationAnalysisContext)
      {
         var curve = new ModelCurve {Name = snapshot.Name};
         var curveOptions = await _curveOptionsMapper.MapToModel(snapshot.CurveOptions);
         curve.CurveOptions.UpdateFrom(curveOptions);

         var yData = findCurveWithPath(snapshot.Y, simulationAnalysisContext.DataRepositories);
         if (yData == null)
         {
            _logger.AddWarning(PKSimConstants.Error.CouldNotFindQuantityWithPath(snapshot.Y));
            return null;
         }

         curve.SetyData(yData, _dimensionFactory);

         ModelDataColumn xData = yData.BaseGrid;
         if (!string.Equals(snapshot.X, xData?.Name))
            xData = findCurveWithPath(snapshot.X, simulationAnalysisContext.DataRepositories);

         if (xData == null)
         {
            _logger.AddWarning(PKSimConstants.Error.CouldNotFindQuantityWithPath(snapshot.X));
            return null;
         }

         curve.SetxData(xData, _dimensionFactory);
         return curve;
      }

      private ModelDataColumn findCurveWithPath(string path, IReadOnlyList<ModelDataRepository> dataRepositories)
      {
         return dataRepositories.SelectMany(x => x.Columns).FirstOrDefault(x => string.Equals(x.QuantityInfo.PathAsString, path));
      }
   }
}