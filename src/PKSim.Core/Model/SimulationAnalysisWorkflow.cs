using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class SimulationAnalysisWorkflow : IWithName, IVisitable<IVisitor>
   {
      private readonly List<ISimulationAnalysis> _allAnalyses;
      public OutputSelections OutputSelections { get; set; }
      public string Name { get; set; }

      public SimulationAnalysisWorkflow()
      {
         _allAnalyses = new List<ISimulationAnalysis>();
         OutputSelections = new OutputSelections();
      }

      public IReadOnlyList<ISimulationAnalysis> AllAnalyses
      {
         get { return _allAnalyses; }
      }

      public void Add(ISimulationAnalysis simulationAnalysis)
      {
         _allAnalyses.Add(simulationAnalysis);
      }

      public void AcceptVisitor(IVisitor visitor)
      {
         visitor.Visit(this);
         _allAnalyses.OfType<IVisitable<IVisitor>>().Each(x => x.AcceptVisitor(visitor));
      }
   }
}