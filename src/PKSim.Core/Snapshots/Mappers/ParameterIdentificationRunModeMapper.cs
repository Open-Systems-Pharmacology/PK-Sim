using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Snapshots;
using ModelParameterIdentificationRunMode = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentificationRunMode;
using SnapshotParameterIdentificationRunMode = OSPSuite.Core.Snapshots.ParameterIdentificationRunMode;


namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationRunModeMapper : OSPSuite.Core.Snapshots.Mappers.ParameterIdentificationRunModeMapper
   {
      private readonly CalculationMethodCacheMapper _calculationMethodCacheMapper;

      public ParameterIdentificationRunModeMapper(CalculationMethodCacheMapper calculationMethodCacheMapper)
      {
         _calculationMethodCacheMapper = calculationMethodCacheMapper;
      }

      protected override async Task MapRunModeParameters(SnapshotParameterIdentificationRunMode snapshot, ModelParameterIdentificationRunMode runMode)
      {
         switch (runMode)
         {
            case CategorialParameterIdentificationRunMode categorialParameterIdentificationRunMode:
               if (categorialParameterIdentificationRunMode.AllTheSame)
                  snapshot.AllTheSameSelection = await _calculationMethodCacheMapper.MapToSnapshot(categorialParameterIdentificationRunMode.AllTheSameSelection);
               else
                  snapshot.CalculationMethods = await calculationMethodsCacheFor(categorialParameterIdentificationRunMode);
               break;
            default:
               await base.MapRunModeParameters(snapshot, runMode);
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

      protected override ModelParameterIdentificationRunMode RunModeFrom(SnapshotParameterIdentificationRunMode snapshot)
      {
         if (snapshot.AllTheSameSelection != null)
            return allTheSameFrom(snapshot);

         if (snapshot.CalculationMethods != null)
            return categorialRunModeFrom(snapshot);

         return new StandardParameterIdentificationRunMode();
      }

      private ModelParameterIdentificationRunMode categorialRunModeFrom(SnapshotParameterIdentificationRunMode snapshot)
      {
         var runMode = new CategorialParameterIdentificationRunMode { AllTheSame = false };

         foreach (var kv in snapshot.CalculationMethods)
         {
            var calculationMethodCache = new OSPSuite.Core.Domain.CalculationMethodCache();
            updateCalculationMethodCache(calculationMethodCache, kv.Value);
            runMode.CalculationMethodsCache[kv.Key] = calculationMethodCache;
         }

         return runMode;
      }

      private ModelParameterIdentificationRunMode allTheSameFrom(SnapshotParameterIdentificationRunMode snapshot)
      {
         var runMode = new CategorialParameterIdentificationRunMode { AllTheSame = true };
         updateCalculationMethodCache(runMode.AllTheSameSelection, snapshot.AllTheSameSelection);
         return runMode;
      }

      private void updateCalculationMethodCache(OSPSuite.Core.Domain.CalculationMethodCache modelCalculationMethodCache, CalculationMethodCache snapshotCalculationMethodCache)
      {
         _calculationMethodCacheMapper.UpdateCalculationMethodCache(modelCalculationMethodCache, snapshotCalculationMethodCache, oneCalculationMethodPerCategory: false);
      }
   }
}