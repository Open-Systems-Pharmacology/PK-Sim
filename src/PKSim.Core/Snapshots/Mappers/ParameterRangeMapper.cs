using SnapshoParameterRange= PKSim.Core.Snapshots.ParameterRange;
using ModelParameterRange= PKSim.Core.Model.ParameterRange;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterRangeMapper: SnapshotMapperBase<ModelParameterRange, SnapshoParameterRange>
   {
      public override SnapshoParameterRange MapToSnapshot(ModelParameterRange parameterRange)
      {
         if (parameterRange == null)
            return null;

         //No range defined. No need to save this range
         if (parameterRange.MaxValueInDisplayUnit == null && parameterRange.MaxValueInDisplayUnit == null)
            return null;

         return SnapshotFrom(parameterRange, snapshot =>
         {
            snapshot.Min = parameterRange.MinValueInDisplayUnit;
            snapshot.Max = parameterRange.MaxValueInDisplayUnit;
            snapshot.Unit = parameterRange.Unit.Name;
         });
      }

      public override ModelParameterRange MapToModel(SnapshoParameterRange snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}