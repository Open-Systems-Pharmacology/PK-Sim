using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Model
{
   public interface IPopulationDataCollector : IAnalysable, IVectorialParametersContainer
   {
      IReadOnlyList<QuantityValues> AllOutputValuesFor(string quantityPath);
      IReadOnlyList<double> AllPKParameterValuesFor(string quantityPath, string pkParameter);
      QuantityPKParameter PKParameterFor(string quantityPath, string pkParameter);
      IReadOnlyList<QuantityPKParameter> AllPKParametersFor(string quantityPath);
      bool HasPKParameterFor(string quantityPath, string pkParameter);
      double? MolWeightFor(string quantityPath);
      IReadOnlyList<string> AllSimulationNames { get; }
      IReadOnlyList<Compound> Compounds { get; }
      /// <summary>
      /// Returns true if supports different aggregation methods, e.g. based on aggregated curve or aggregated from all individuals
      /// </summary>
      bool SupportsMultipleAggregations { get; }
   }
}