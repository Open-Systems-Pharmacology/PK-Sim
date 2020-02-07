using System.Threading.Tasks;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Services
{
   public interface IMoBiExportTask
   {
      /// <summary>
      ///    Exports the simulation and starts mobi with it
      /// </summary>
      /// <param name="simulation">simulation to export</param>
      void StartWith(Simulation simulation);

      /// <summary>
      /// Starts MoBi with the content file as start parameter. The file contains binary data that should be
      /// decrypted by MoBi
      /// </summary>
      void StartWithContentFile(string contentFileFullPath);

      /// <summary>
      ///    Exports the simulation into a predefined file
      /// </summary>
      void ExportSimulationToPkmlFile(Simulation simulation);

      /// <summary>
      /// Exports the simulation into the file with path <paramref name="fileName"/>
      /// </summary>
      void ExportSimulationToPkmlFile(Simulation simulation,string fileName);

      /// <summary>
      /// Exports the simulation into the file with path <paramref name="fileName"/>
      /// </summary>
      Task SaveSimulationToFileAsync(Simulation simulation, string fileName);

      void UpdateObserverForAllFlag(IObserverBuildingBlock observerBuildingBlock);
   }
}