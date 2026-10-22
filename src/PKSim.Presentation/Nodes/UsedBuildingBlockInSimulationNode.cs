using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.Nodes
{
   public class UsedBuildingBlockInSimulationNode : AbstractNode
   {
      private readonly string _usedBuildingBlockId;
      private readonly IBuildingBlockRetriever _buildingBlockRetriever;
      public Simulation Simulation { get; }

      public UsedBuildingBlockInSimulationNode(Simulation simulation, UsedBuildingBlock usedBuildingBlock, IBuildingBlockRetriever buildingBlockRetriever)
      {
         //Do not store the used building block id itself since lazy loading might overwrite building block in simulation. 
         //We will always retrieve the building block from its id
         _usedBuildingBlockId = usedBuildingBlock.Id;
         _buildingBlockRetriever = buildingBlockRetriever;
         Simulation = simulation;
      }

      public override string Id => _usedBuildingBlockId;

      public override object TagAsObject => UsedBuildingBlock;

      /// <summary>
      ///    Return the used building block in the simulation represented by the node
      /// </summary>
      public UsedBuildingBlock UsedBuildingBlock => Simulation.UsedBuildingBlockById(_usedBuildingBlockId);

      /// <summary>
      ///    Return the template building block in the project that the used building block was created from, or <c>null</c>
      ///    if no such building block exists. The template is resolved on demand and never cached: the node may be created
      ///    while the template is being lazy loaded (and is thus temporarily not registered) or the template may be swapped
      ///    for another building block with the same id after the node was created.
      /// </summary>
      public IPKSimBuildingBlock TemplateBuildingBlock
      {
         get
         {
            var usedBuildingBlock = UsedBuildingBlock;
            return usedBuildingBlock == null ? null : _buildingBlockRetriever.BuildingBlockWithId(usedBuildingBlock.TemplateId);
         }
      }
   }
}
