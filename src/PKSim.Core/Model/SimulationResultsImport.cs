using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core.Model
{
   public class SimulationResultsImport : ImportLogger
   {
      public virtual SimulationResults SimulationResults { get; private set; }
      public virtual IList<SimulationResultsFile> SimulationResultsFiles { get; private set; }

      public SimulationResultsImport()
      {
         SimulationResults = new SimulationResults();
         SimulationResultsFiles = new List<SimulationResultsFile>();
      }

      /// <summary>
      ///    Status of import action. Its value indicates whether the import was successful or not
      /// </summary>
      public override NotificationType Status
      {
         get { return SimulationResultsFiles.Aggregate(base.Status, (notification, simResultsFile) => notification | simResultsFile.Status); }
      }

      public override IEnumerable<string> Log
      {
         get
         {
            var log = new List<string>(base.Log);
            foreach (var simulationResultsFile in SimulationResultsFiles)
            {
               log.AddRange(simulationResultsFile.Log);
            }
            return log;
         }
      }

      public virtual bool HasError
      {
         get { return Status.Is(NotificationType.Error); }
      }
   }
}