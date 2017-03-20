using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public class SimulationsReporter : OSPSuiteTeXReporter<IReadOnlyCollection<Simulation>>
   {
      private readonly SimulationReporter _simulationReporter;

      public SimulationsReporter(SimulationReporter simulationReporter)
      {
         _simulationReporter = simulationReporter;
      }

      public override IReadOnlyCollection<object> Report(IReadOnlyCollection<Simulation> simulations, OSPSuiteTracker buildTracker)
      {
         var list = new List<object>();

         if (!simulations.Any())
            return list;

         list.Add(new Part(PKSimConstants.ObjectTypes.Simulation.Pluralize()));
         simulations.Each(s => list.AddRange(_simulationReporter.Report(s, buildTracker)));
         return list;
      }
   }
}