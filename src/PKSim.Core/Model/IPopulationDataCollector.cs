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
      IReadOnlyList<string> AllSimulationNames();
      IReadOnlyList<Compound> Compounds { get; }
   }
}