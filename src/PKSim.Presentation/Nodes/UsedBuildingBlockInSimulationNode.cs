using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;

namespace PKSim.Presentation.Nodes
{
   public class UsedBuildingBlockInSimulationNode : AbstractNode
   {
      private readonly string _usedBuildingBlockId;
      public IPKSimBuildingBlock TemplateBuildingBlock { get; set; }
      public Simulation Simulation { get; private set; }

      public UsedBuildingBlockInSimulationNode(Simulation simulation, UsedBuildingBlock usedBuildingBlock, IPKSimBuildingBlock templateBuildingBlock)
      {
         TemplateBuildingBlock = templateBuildingBlock;
         //Do not store the used building block id itself since lazy loading might overwrite building block in simulation. 
         //We will always retrieve the building block from its id
         _usedBuildingBlockId = usedBuildingBlock.Id;
         Simulation = simulation;
      }

      public override string Id
      {
         get { return _usedBuildingBlockId; }
      }

      public override object TagAsObject
      {
         get { return UsedBuildingBlock; }
      }

      /// <summary>
      ///    Return the used building block in the simulation represented by the node
      /// </summary>
      public UsedBuildingBlock UsedBuildingBlock
      {
         get { return Simulation.UsedBuildingBlockById(_usedBuildingBlockId); }
      }
   }
}