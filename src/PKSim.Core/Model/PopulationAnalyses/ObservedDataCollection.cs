using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model.PopulationAnalyses
{
   /// <summary>
   ///    Represents a collection of <see cref="DataRepository" /> also containing <see cref="ObservedDataCurveOptions" /> for
   ///    each observed data columns.
   /// </summary>
   public class ObservedDataCollection : IWithObservedData, IUpdatable, IEnumerable<DataRepository>
   {
      private readonly Cache<string, DataRepository> _allDataRepositories = new Cache<string, DataRepository>(x => x.Id);
      private readonly Cache<string, ObservedDataCurveOptions> _allCurveOptions = new Cache<string, ObservedDataCurveOptions>(x => x.ColumnId, x => null);
      public bool ApplyGroupingToObservedData { get; set; }

      public IEnumerable<ObservedDataCurveOptions> ObservedDataCurveOptions()
      {
         return _allCurveOptions;
      }

      public void AddCurveOptions(ObservedDataCurveOptions curveOptions)
      {
         if (curveOptions != null)
            _allCurveOptions[curveOptions.ColumnId] = curveOptions;
      }

      public virtual bool HasCurveOptionsFor(DataColumn dataColumn)
      {
         return _allCurveOptions.Contains(dataColumn.Id);
      }

      public ObservedDataCurveOptions ObservedDataCurveOptionsFor(DataColumn dataColumn)
      {
         return _allCurveOptions[dataColumn.Id] ?? defaultCurveOptionsFor(dataColumn);
      }

      private ObservedDataCurveOptions defaultCurveOptionsFor(DataColumn dataColumn)
      {
         return new ObservedDataCurveOptions {ColumnId = dataColumn.Id};
      }

      public void AddObservedData(DataRepository observedData)
      {
         if (observedData == null) return;
         if (UsesObservedData(observedData))
            return;

         _allDataRepositories.Add(observedData);
         addDefaultSettingsFor(observedData);
      }

      private void addDefaultSettingsFor(DataRepository observedData)
      {
         observedData.ObservationColumns()
            .Where(c => !_allCurveOptions.Contains(c.Id))
            .Each(c => _allCurveOptions.Add(defaultCurveOptionsFor(c)));
      }

      public IEnumerable<DataRepository> AllObservedData()
      {
         return _allDataRepositories;
      }

      public bool UsesObservedData(DataRepository dataRepository)
      {
         return dataRepository != null && _allDataRepositories.Contains(dataRepository.Id);
      }

      public void RemoveObservedData(DataRepository observedData)
      {
         if (!UsesObservedData(observedData))
            return;

         _allDataRepositories.Remove(observedData.Id);
         observedData.ObservationColumns()
            .Where(c => _allCurveOptions.Contains(c.Id)).ToList()
            .Each(c => _allCurveOptions.Remove(c.Id));
      }

      public void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var sourceObservedDataCollection = source as ObservedDataCollection;
         if (sourceObservedDataCollection == null) return;
         sourceObservedDataCollection.AllObservedData().Each(AddObservedData);
         sourceObservedDataCollection.ObservedDataCurveOptions().Each(AddCurveOptions);
         ApplyGroupingToObservedData = sourceObservedDataCollection.ApplyGroupingToObservedData;
      }

      public void UpdateFrom(ObservedDataCollection observedDataCollection)
      {
         if (observedDataCollection == null) return;
         observedDataCollection.AllObservedData().Each(AddObservedData);
         observedDataCollection.ObservedDataCurveOptions().Each(AddCurveOptions);
         ApplyGroupingToObservedData = observedDataCollection.ApplyGroupingToObservedData;
      }

      public IEnumerator<DataRepository> GetEnumerator()
      {
         return _allDataRepositories.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}