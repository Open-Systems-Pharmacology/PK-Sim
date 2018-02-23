using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services;
using ModelOutputMapping = OSPSuite.Core.Domain.ParameterIdentifications.OutputMapping;
using SnapshotOutputMapping = PKSim.Core.Snapshots.OutputMapping;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputMappingMapper : SnapshotMapperBase<ModelOutputMapping, SnapshotOutputMapping>
   {
      public override Task<SnapshotOutputMapping> MapToSnapshot(ModelOutputMapping outputMapping)
      {
         return SnapshotFrom(outputMapping, x =>
         {
            x.Scaling = outputMapping.Scaling;
            x.Weight = SnapshotValueFor(outputMapping.Weight, Constants.DEFAULT_WEIGHT);
            x.Path = outputMapping.FullOutputPath;
            x.ObservedData = outputMapping.WeightedObservedData?.Name;
            x.Weights = weightsFrom(outputMapping.WeightedObservedData);
         });
      }

      private float[] weightsFrom(WeightedObservedData weightedObservedData)
      {
         var weights = weightedObservedData?.Weights;
         if (weights == null)
            return null;

         if (weights.All(x => ValueComparer.AreValuesEqual(x, Constants.DEFAULT_WEIGHT)))
            return null;

         return weights;
      }

      public override Task<ModelOutputMapping> MapToModel(SnapshotOutputMapping snapshot)
      {
         throw new NotImplementedException();
      }
   }
}