using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ISimulationExportTask
   {
      /// <summary>
      ///    Exports individual simulation results to excel
      /// </summary>
      void ExportResultsToExcel(IndividualSimulation individualSimulation);

      /// <summary>
      ///    Exports simulation results to csv file selected by the user
      /// </summary>
      Task ExportResultsToCSVAsync(Simulation simulation);

      /// <summary>
      ///    Exports simulation results to the csv file with path <paramref name="fileName"/>
      /// </summary>
      Task ExportResultsToCSVAsync(Simulation simulation, string fileName);

      /// <summary>
      ///    save the simulation as xml format (the one saved in the pksim project)
      /// </summary>
      void ExportSimulationToXml(Simulation simulation);

      /// <summary>
      ///    save the simulation as sim model xml  format (the one used in SimModel or matlab)
      /// </summary>
      void ExportSimulationToSimModelXml(Simulation simulation);

      /// <summary>
      ///    save the simulation as sim model xml  format (the one used in SimModel or matlab)
      /// </summary>
      void ExportSimulationToSimModelXml(Simulation simulation, string fileName);


      void CreateSimulationReport(Simulation simulation);

      /// <summary>
      ///    Exports simulation pk-analyses to csv file
      /// </summary>
      Task ExportPKAnalysesToCSVAsync(PopulationSimulation populationSimulation);
   }
}