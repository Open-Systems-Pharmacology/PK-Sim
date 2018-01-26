using System.Data;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class SimulationResultsToDataTableConverter : ISimulationResultsToDataTableConverter
   {
      private readonly IEntitiesInContainerRetriever _quantityRetriever;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;
      private readonly IDimension _timeDimension;

      public SimulationResultsToDataTableConverter(IDimensionRepository dimensionRepository, IEntitiesInContainerRetriever quantityRetriever, IDisplayUnitRetriever displayUnitRetriever)
      {
         _quantityRetriever = quantityRetriever;
         _displayUnitRetriever = displayUnitRetriever;
         _timeDimension = dimensionRepository.Time;
      }

      public Task<DataTable> ResultsToDataTable(Simulation simulation)
      {
         return Task.Run(() => createResultDataToExport(simulation));
      }

      public Task<DataTable> PKAnalysesToDataTable(PopulationSimulation populationSimulation)
      {
         return Task.Run(() => createPKAnalysesDataToExport(populationSimulation));
      }

      private DataTable createPKAnalysesDataToExport(PopulationSimulation populationSimulation)
      {
         var dataTable = new DataTable(populationSimulation.Name);
         if (!populationSimulation.HasPKAnalyses)
            return dataTable;

         var pkAnalyses = populationSimulation.PKAnalyses;
         dataTable.AddColumn<int>(CoreConstants.SimulationResults.INDIVIDUAL_ID);
         dataTable.AddColumn<string>(CoreConstants.SimulationResults.QUANTITY_PATH);
         dataTable.AddColumn<string>(CoreConstants.SimulationResults.PARAMETER);
         dataTable.AddColumn<string>(CoreConstants.SimulationResults.VALUE);
         dataTable.AddColumn<string>(CoreConstants.SimulationResults.UNIT);

         dataTable.BeginLoadData();
         foreach (var pkParameter in pkAnalyses.All())
         {
            var parameter = pkParameter;
            var unit = _displayUnitRetriever.PreferredUnitFor(parameter);
            parameter.Values.Each((value, index) =>
            {
               var row = dataTable.NewRow();
               row[CoreConstants.SimulationResults.INDIVIDUAL_ID] = index;
               row[CoreConstants.SimulationResults.QUANTITY_PATH] = parameter.QuantityPath;
               row[CoreConstants.SimulationResults.PARAMETER] = parameter.Name;
               row[CoreConstants.SimulationResults.VALUE] = parameter.ConvertToUnit(value, unit).ConvertedTo<string>();
               row[CoreConstants.SimulationResults.UNIT] = unit.Name;
               dataTable.Rows.Add(row);
            });
         }
         dataTable.EndLoadData();
         return dataTable;
      }

      private DataTable createResultDataToExport(Simulation simulation)
      {
         //Id	Time	Output1	Output2	...	OutputN
         var dataTable = new DataTable(simulation.Name);
         if (!simulation.HasResults)
            return dataTable;

         var simulationResults = simulation.Results;

         var allQuantities = _quantityRetriever.QuantitiesFrom(simulation);
         var timeColumnName = Constants.NameWithUnitFor(CoreConstants.SimulationResults.TIME, _timeDimension.BaseUnit);
         var quantityPathCache = new Cache<string, string>();
         dataTable.AddColumn<int>(CoreConstants.SimulationResults.INDIVIDUAL_ID);
         dataTable.AddColumn<string>(timeColumnName);

         var allQuantityPaths = simulationResults.AllQuantityPaths();
         foreach (var quantityPath in allQuantityPaths)
         {
            var quantity = allQuantities[quantityPath];
            if (quantity == null) continue;

            //export results in base unit so that they can be computed automatically from matlab scripts
            quantityPathCache[quantityPath] = Constants.NameWithUnitFor(quantityPath, quantity.Dimension.BaseUnit);
            dataTable.AddColumn<string>(quantityPathCache[quantityPath]);
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
               row[CoreConstants.SimulationResults.INDIVIDUAL_ID] = individualResults.IndividualId;
               row[timeColumnName] = simulationResults.Time[i].ConvertedTo<string>();

               foreach (var quantityPath in allQuantityPaths)
               {
                  var quantity = allQuantities[quantityPath];
                  if (quantity == null) continue;

                  row[quantityPathCache[quantityPath]] = allQuantitiesCache[quantityPath][i].ConvertedTo<string>();
               }
               dataTable.Rows.Add(row);
            }
         }
         dataTable.EndLoadData();
         return dataTable;
      }
   }
}