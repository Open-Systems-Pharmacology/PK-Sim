using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public class ObservedDataCollectionReporter : OSPSuiteTeXReporter<IReadOnlyCollection<DataRepository>>
   {
      public override IReadOnlyCollection<object> Report(IReadOnlyCollection<DataRepository> allObservedData, OSPSuiteTracker tracker)
      {
         var report = new List<object>();
         if (!allObservedData.Any())
            return report;

         report.Add(new Part(PKSimConstants.UI.ObservedData));
         report.AddRange(allObservedData);
         return report;
      }
   }
}