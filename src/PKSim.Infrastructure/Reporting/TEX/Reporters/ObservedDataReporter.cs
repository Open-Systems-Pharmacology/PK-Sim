using System.Collections.Generic;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public class ObservedDataReporter : OSPSuiteTeXReporter<DataRepository>
   {
      public override IReadOnlyCollection<object> Report(DataRepository dataRepository, OSPSuiteTracker buildTracker)
      {
         return new List<object> { new Chapter(PKSimConstants.UI.ObservedData), dataRepository};
      }
   }
}