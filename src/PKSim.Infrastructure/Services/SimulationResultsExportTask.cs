using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BTS.Utility.Collections;
using BTS.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using SBSuite.Core.Domain.Data;
using SBSuite.Core.Domain.UnitSystem;
using SBSuite.Core.Extensions;

namespace PKSim.Infrastructure.Services
{
   public class SimulationResultsExportTask : ISimulationResultsExportTask
   {
      private readonly IEntitiesInContainerRetriever _quantityRetriever;
      private readonly IDimension _timeDimension;

      public SimulationResultsExportTask(IDimensionRepository dimensionRepository, IEntitiesInContainerRetriever quantityRetriever)
      {
         _quantityRetriever = quantityRetriever;
         _timeDimension = dimensionRepository.Time;
      }

      public Task ExportToCSV(string fileFullPath, Simulation simulation)
      {
         return Task.Run(() =>
         {
            var dataTable = createDataToExport(simulation);
            dataTable.ExportToCSV(fileFullPath);
         });
      }

      private DataTable createDataToExport(Simulation simulation)
      {
         //Id	Time	Output1	Output2	...	OutputN

         var dataTable = new DataTable();
         if (!simulation.HasResults)
            return dataTable;

         var simulationResults = simulation.Results;

         var allQuantities = _quantityRetriever.QuantitiesFrom(simulation);
         dataTable.AddColumn<int>(CoreConstants.SimulationResults.IndividualId);
         dataTable.AddColumn<string>(CoreConstants.NameWithUnitFor(CoreConstants.SimulationResults.Time, _timeDimension.BaseUnit));

         var allQuantityPaths = simulationResults.AllQuantityPaths();
         foreach (var quantityPath in allQuantityPaths)
         {
            var quantity = allQuantities[quantityPath];
            if (quantity == null) continue;
            dataTable.AddColumn<string>(CoreConstants.NameWithUnitFor(quantityPath, quantity.Dimension.BaseUnit));
         }

         dataTable.BeginLoadData();
         int numberOfValues = simulationResults.Time.Length;

         foreach (var individualResults in simulationResults.OrderBy(x => x.IndividualId))
         {
            var allQuantitiesCache = new Cache<string, QuantityValues>(x => x.QuantityPath);
            allQuantitiesCache.AddRange(individualResults);

            for (int i = 0; i < numberOfValues; i++)
            {
               var row = dataTable.NewRow();
               row[CoreConstants.SimulationResults.IndividualId] = individualResults.IndividualId;
               row[CoreConstants.NameWithUnitFor(CoreConstants.SimulationResults.Time, _timeDimension.BaseUnit)] = simulationResults.Time[i].ConvertedTo<string>();

               foreach (var quantityPath in allQuantityPaths)
               {
                  var quantity = allQuantities[quantityPath];
                  if (quantity == null) continue;

                  row[CoreConstants.NameWithUnitFor(quantityPath, quantity.Dimension.BaseUnit)] = allQuantitiesCache[quantityPath][i].ConvertedTo<string>();
               }
               dataTable.Rows.Add(row);
            }
         }
         dataTable.EndLoadData();
         return dataTable;
      }
   }
}