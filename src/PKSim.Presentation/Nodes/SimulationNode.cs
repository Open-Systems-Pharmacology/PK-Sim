using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation.Nodes
{
   public class SimulationNode : ObjectWithIdAndNameNode<ClassifiableSimulation>
   {
      public SimulationNode(ClassifiableSimulation classifiableSimulation)
         : base(classifiableSimulation)
      {
      }

      public Simulation Simulation => Tag.Simulation;
   }
}