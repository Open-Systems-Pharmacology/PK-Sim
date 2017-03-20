using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Services
{
   public interface ISimulationResultsCreator
   {
      SimulationResults CreateResultsFrom(DataRepository dataRepository);
   }

   public class SimulationResultsCreator : ISimulationResultsCreator
   {
      public SimulationResults CreateResultsFrom(DataRepository dataRepository)
      {
         if (dataRepository.IsNull())
            return new NullSimulationResults();

         var results = new SimulationResults();
         var baseGrid = dataRepository.BaseGrid;
         results.Time = valuesFrom(baseGrid);
         foreach (var columnsForIndividual in dataRepository.AllButBaseGrid().GroupBy(selectIndividualIndex))
         {
            var individualResults = new IndividualResults {IndividualId = columnsForIndividual.Key,Time = results.Time};
            foreach (var dataColumn in columnsForIndividual)
            {
               individualResults.Add(valuesFrom(dataColumn, pathWithoutIndividualIndex(dataColumn, columnsForIndividual.Key)));
            }
            individualResults.UpdateQuantityTimeReference();
            results.Add(individualResults);
         }
         return results;
      }

      private string pathWithoutIndividualIndex(DataColumn dataColumn, int index)
      {
         var path = dataColumn.QuantityInfo.Path.ToList();

         if (path.Last() == index.ConvertedTo<string>())
            path.RemoveAt(path.Count - 1);

         //remove simulation name
         path.RemoveAt(0);

         return path.ToPathString();
      }

      private int selectIndividualIndex(DataColumn column)
      {
         var lastEntry = column.QuantityInfo.Path.Last();
         int index = 0;
         int.TryParse(lastEntry, out index);
         return index;
      }

      private QuantityValues valuesFrom(DataColumn dataColumn)
      {
         return valuesFrom(dataColumn, dataColumn.QuantityInfo.PathAsString);
      }

      private QuantityValues valuesFrom(DataColumn dataColumn, string quantityPath)
      {
         return new QuantityValues
         {
            ColumnId = dataColumn.Id,
            QuantityPath = quantityPath,
            Values = dataColumn.Values.ToArray(),
         };
      }
   }
}