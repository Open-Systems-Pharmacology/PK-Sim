using System.Data;
using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationResultsToDataTableConverter
   {
      /// <summary>
      ///    Creates a <see cref="DataTable"/> containing  the results of the simulation. 
      /// </summary>
      /// <remarks>The format of the table will be one column per output and one row per individual</remarks>
      /// <param name="simulation">Simulation containing the results used to create the <see cref="DataTable"/></param>
      Task<DataTable> ResultsToDataTable(Simulation simulation);


      /// <summary>
      ///    Creates a <see cref="DataTable"/> containing the PK-Analyses of the populationSimulation. 
      /// </summary>
      /// <remarks>The format of the table will be IndividualId, QuantityPath, ParameterName, Value, Unit</remarks>
      /// <param name="populationSimulation">Simulation containing the PK-Analyses used to create the <see cref="DataTable"/></param>
      Task<DataTable> PKAnalysesToDataTable(PopulationSimulation populationSimulation);
   }
}