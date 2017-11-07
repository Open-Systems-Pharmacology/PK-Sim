using System.Data;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPopulationExportTask
   {
      /// <summary>
      ///    Export the given <paramref name="population" /> to a csv file to be selected by the user
      /// </summary>
      void ExportToCSV(Population population);

      /// <summary>
      ///    Export the given <paramref name="population" /> to <paramref name="fileFullPath"/>
      /// </summary>
      void ExportToCSV(Population population, string fileFullPath);

      /// <summary>
      ///    Returns a DataTable containing one row per variable parameter in the <paramref name="population" /> and one column per individual
      ///    The First Columns contains the consolidated path (Starting with Organism for instance) of the parameter
      ///   <param name="population">The population to export</param>
      ///   <param name="includeUnitsInHeader">If true, then the column header will include the parameter base unit</param>
      /// </summary>
      DataTable CreatePopulationDataFor(Population population, bool includeUnitsInHeader = false);

      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to a csv file to be selected by the user
      /// </summary>
      void ExportToCSV(PopulationSimulation populationSimulation);

      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to <paramref name="fileFullPath"/>
      /// </summary>
      void ExportToCSV(PopulationSimulation populationSimulation, string fileFullPath);

      /// <summary>
      ///    Returns a DataTable containing one row per advanced parameter in the <paramref name="populationSimulation" /> as well as all
      ///    variable parmaeters defined in the underlying population. One column is created per individual
      ///    The First Columns contains the consolidated path (Starting with Organism for instance) of the parameter.
      ///   <param name="populationSimulation">The simulation using the population to export</param>
      ///   <param name="includeUnitsInHeader">If true, then the column header will include the parameter base unit</param>
      /// </summary>
      DataTable CreatePopulationDataFor(PopulationSimulation populationSimulation, bool includeUnitsInHeader = false);

      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to a folder containing all the necessary files to run the matlab wrapper
      /// </summary>
      void ExportForCluster(PopulationSimulation populationSimulation);
   }
}