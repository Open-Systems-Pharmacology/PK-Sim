using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using ModelObservedDataCollection = PKSim.Core.Model.PopulationAnalyses.ObservedDataCollection;
using ModelObservedDataCurveOptions = PKSim.Core.Model.PopulationAnalyses.ObservedDataCurveOptions;
using SnapshotObservedDataCollection = PKSim.Core.Snapshots.ObservedDataCollection;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ObservedDataCollectionMappper : SnapshotMapperBase<ModelObservedDataCollection, SnapshotObservedDataCollection, SimulationAnalysisContext>
   {
      private readonly CurveOptionsMapper _curveOptionsMapper;

      public ObservedDataCollectionMappper(CurveOptionsMapper curveOptionsMapper)
      {
         _curveOptionsMapper = curveOptionsMapper;
      }

      public override async Task<SnapshotObservedDataCollection> MapToSnapshot(ModelObservedDataCollection observedDataCollection)
      {
         var allObservedDataNames = observedDataCollection?.AllObservedData().AllNames().ToArray();
         if (allObservedDataNames == null || !allObservedDataNames.Any())
            return null;

         var snapshot = await SnapshotFrom(observedDataCollection, x =>
         {
            x.ObservedData = allObservedDataNames;
            x.ApplyGrouping = observedDataCollection.ApplyGroupingToObservedData;
         });

         snapshot.CurveOptions = await curveOptionSnapshotsFrom(observedDataCollection);
         return snapshot;
      }

      private async Task<ObservedDataCurveOptions[]> curveOptionSnapshotsFrom(ModelObservedDataCollection observedDataCollection)
      {
         var allObservedDatacoluns = observedDataCollection.AllObservedData().SelectMany(x => x.AllButBaseGrid())
            .Where(observedDataCollection.HasCurveOptionsFor)
            .Select(x => new
            {
               Path = x.PathAsString,
               CurveOptions = observedDataCollection.ObservedDataCurveOptionsFor(x),
            });

         var list = new List<ObservedDataCurveOptions>();
         foreach (var column in allObservedDatacoluns)
         {
            var curveOptions = column.CurveOptions;
            list.Add(new ObservedDataCurveOptions
            {
               Caption = SnapshotValueFor(curveOptions.Caption),
               Path = column.Path,
               CurveOptions = await _curveOptionsMapper.MapToSnapshot(curveOptions.CurveOptions)
            });
         }

         return list.ToArray();
      }

      public override async Task<ModelObservedDataCollection> MapToModel(SnapshotObservedDataCollection snapshot, SimulationAnalysisContext simulationAnalysisContext)
      {
         if (snapshot == null)
            return null;

         var observedDataCollection = new ModelObservedDataCollection {ApplyGroupingToObservedData = snapshot.ApplyGrouping};
         var allObservedDataRepositories = snapshot.ObservedData.Select(x => simulationAnalysisContext.DataRepositories.FindByName(x)).Where(x => x != null).ToList();
         allObservedDataRepositories.Each(observedDataCollection.AddObservedData);

         var allObservedDataColumns = allObservedDataRepositories.SelectMany(x => x.ObservationColumns()).ToList();
         var observedDataCurveOptions = await Task.WhenAll(snapshot.CurveOptions.Select(x => curveOptionsFrom(x, allObservedDataColumns)));
         observedDataCurveOptions.Each(observedDataCollection.AddCurveOptions);
         return observedDataCollection;
      }

      private async Task<ModelObservedDataCurveOptions> curveOptionsFrom(ObservedDataCurveOptions snapshot, List<OSPSuite.Core.Domain.Data.DataColumn> allObservedDataColumns)
      {
         var column = allObservedDataColumns.Find(x => string.Equals(x.PathAsString, snapshot.Path));
         if (column == null)
            return null;

         var observedDataCurveOption = new ModelObservedDataCurveOptions
         {
            ColumnId = column.Id,
            Caption = snapshot.Caption,
         };
         observedDataCurveOption.CurveOptions.UpdateFrom(await _curveOptionsMapper.MapToModel(snapshot.CurveOptions));
         return observedDataCurveOption;
      }
   }
}