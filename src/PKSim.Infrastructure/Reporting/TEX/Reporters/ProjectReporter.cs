using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Reporters
{
   public class ProjectReporter : OSPSuiteTeXReporter<PKSimProject>
   {
      private readonly BuildingBlocksReporter _buildingBlocksReporter;
      private readonly SimulationsReporter _simulationsReporter;
      private readonly SimulationComparisonsReporter _simulationComparisonsReporter;
      private readonly ObservedDataCollectionReporter _observedDataReporter;

      public ProjectReporter(BuildingBlocksReporter buildingBlocksReporter, SimulationsReporter simulationsReporter, SimulationComparisonsReporter simulationComparisonsReporter, ObservedDataCollectionReporter observedDataReporter)
      {
         _buildingBlocksReporter = buildingBlocksReporter;
         _simulationsReporter = simulationsReporter;
         _simulationComparisonsReporter = simulationComparisonsReporter;
         _observedDataReporter = observedDataReporter;
      }

      public override IReadOnlyCollection<object> Report(PKSimProject project, OSPSuiteTracker tracker)
      {
         var list = new List<object>();
         var buildingBlocks = project.All(PKSimBuildingBlockType.Template).ToList();
         if (buildingBlocks.Any())
         {
            list.Add(new Part(PKSimConstants.UI.BuildingBlocks));
            list.AddRange(_buildingBlocksReporter.Report(buildingBlocks, tracker));
         }
         list.AddRange(_simulationsReporter.Report(project.All<Simulation>().ToList(), tracker));
         list.AddRange(_simulationComparisonsReporter.Report(project.AllSimulationComparisons.ToList(), tracker));
         list.AddRange(_observedDataReporter.Report(project.AllObservedData.ToList(), tracker));
         return list;
      }
   }
}