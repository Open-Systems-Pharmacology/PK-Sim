using System.Data;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IPopulationExportTask
   {
      /// <summary>
      ///    Export the given <paramref name="population" /> to a csv file
      /// </summary>
      void ExportToCSV(Population population);

      /// <summary>
      ///    Returns a DataTable containing one row per variable parameter in the <paramref name="population" /> and one column per individual
      ///    The First Columns contains the consolidated path (Starting with Organism for instance) of the parameter
      /// </summary>
      DataTable CreatePopulationDataFor(Population population);

      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to a csv file
      /// </summary>
      void ExportToCSV(PopulationSimulation populationSimulation);

      /// <summary>
      ///    Returns a DataTable containing one row per advanced parameter in the <paramref name="populationSimulation" /> as well as all
      ///    variable parmaeters defined in the underlying population. One column is created per individual
      ///    The First Columns contains the consolidated path (Starting with Organism for instance) of the parameter.
      /// </summary>
      DataTable CreatePopulationDataFor(PopulationSimulation populationSimulation);

      /// <summary>
      ///    Export the given <paramref name="populationSimulation" /> to a folder containing all the necessary files to run the matlab wrapper
      /// </summary>
      void ExportForCluster(PopulationSimulation populationSimulation);
   }
}