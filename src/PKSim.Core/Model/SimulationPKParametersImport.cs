using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class SimulationPKParametersImport : ImportLogger
   {
      public virtual PKAnalysesImportFile PKAnalysesFile { get; private set; }
      public virtual IEnumerable<QuantityPKParameter> PKParameters { get; private set; }

      public SimulationPKParametersImport(IEnumerable<QuantityPKParameter> pkParameters, PKAnalysesImportFile pkAnalysesFile)
      {
         PKAnalysesFile = pkAnalysesFile;
         PKParameters = pkParameters;
      }

      /// <summary>
      ///    Status of import action. Its value indicates whether the import was successful or not
      /// </summary>
      public override NotificationType Status
      {
         get { return base.Status | PKAnalysesFile.Status; }
      }

      public override IEnumerable<string> Log
      {
         get
         {
            var log = new List<string>(base.Log);
            log.AddRange(PKAnalysesFile.Log);
            return log;
         }
      }
   }
}