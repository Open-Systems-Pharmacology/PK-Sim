using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.ParameterIdentifications;
using ModelParameterIdentificationRunMode = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentificationRunMode;
using SnapshotParameterIdentificationRunMode = PKSim.Core.Snapshots.ParameterIdentificationRunMode;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationRunModeMapper : SnapshotMapperBase<ModelParameterIdentificationRunMode, SnapshotParameterIdentificationRunMode>
   {
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;

      public ParameterIdentificationRunModeMapper(CalculationMethodCacheMapper calculationMethodCacheMapper)
      {
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
      }

      public override async Task<SnapshotParameterIdentificationRunMode> MapToSnapshot(ModelParameterIdentificationRunMode runMode)
      {
         if (runMode == null)
            return null;

         var snapshot = await SnapshotFrom(runMode, x => { x.Name = runMode.GetType().Name; });
         await mapRunModeParameters(snapshot, runMode);
         return snapshot;
      }

      private async Task mapRunModeParameters(SnapshotParameterIdentificationRunMode snapshot, ModelParameterIdentificationRunMode runMode)
      {
         switch (runMode)
         {
            case MultipleParameterIdentificationRunMode multipleParameterIdentificationRunMode:
               snapshot.NumberOfRuns = multipleParameterIdentificationRunMode.NumberOfRuns;
               break;
            case CategorialParameterIdentificationRunMode categorialParameterIdentificationRunMode:
               if (categorialParameterIdentificationRunMode.AllTheSame)
                  snapshot.AllTheSameSelection = await _calculationMethodCacheMapper.MapToSnapshot(categorialParameterIdentificationRunMode.AllTheSameSelection);
               else
                  snapshot.CalculationMethods = await calculationMethodsCacheFor(categorialParameterIdentificationRunMode);
               break;
         }
      }

      private async Task<Dictionary<string, CalculationMethodCache>> calculationMethodsCacheFor(CategorialParameterIdentificationRunMode categorialParameterIdentificationRunMode1)
      {
         var cache = new Dictionary<string, CalculationMethodCache>();
         foreach (var kv in categorialParameterIdentificationRunMode1.CalculationMethodsCache.KeyValues)
         {
            cache[kv.Key] = await _calculationMethodCacheMapper.MapToSnapshot(kv.Value);
         }

         return cache;
      }

      public override Task<ModelParameterIdentificationRunMode> MapToModel(SnapshotParameterIdentificationRunMode snapshot)
      {
         throw new NotImplementedException();
      }
   }
}