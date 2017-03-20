using System.Collections.Generic;
using PKSim.Core.Chart;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Mappers
{
   public interface IDataRepositoryToObservedCurveDataMapper
   {
      IReadOnlyList<ObservedCurveData> MapFrom(DataRepository observedData, ObservedDataCollection observedDataCollection, IDimension yAxisDimension);
   }
}