using System.Threading.Tasks;
using SnapshotCurve = PKSim.Core.Snapshots.Curve;
using ModelCurve = OSPSuite.Core.Chart.Curve;

namespace PKSim.Core.Snapshots.Mappers
{
   public class CurveMapper : SnapshotMapperBase<ModelCurve, SnapshotCurve>
   {
      public override Task<Curve> MapToSnapshot(ModelCurve curve)
      {
         return SnapshotFrom(curve, x =>
         {
            x.Name = SnapshotValueFor(curve.Name);
            x.X = curve.xData?.PathAsString;
            x.Y = curve.yData?.PathAsString;
            x.CurveOptions = curve.CurveOptions;
         });
      }

      public override Task<ModelCurve> MapToModel(SnapshotCurve snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}