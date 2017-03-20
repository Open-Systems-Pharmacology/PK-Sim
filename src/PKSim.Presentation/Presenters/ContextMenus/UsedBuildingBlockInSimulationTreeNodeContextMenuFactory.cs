using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<TBuildingBlock> : IContextMenuSpecificationFactory<ITreeNode>
   {
      protected abstract IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, TBuildingBlock buildingBlock);

      public IContextMenu CreateFor(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         var bbInSimulationNode = treeNode.DowncastTo<UsedBuildingBlockInSimulationNode>();
         return CreateFor(bbInSimulationNode.Simulation, bbInSimulationNode.UsedBuildingBlock, bbInSimulationNode.TemplateBuildingBlock.DowncastTo<TBuildingBlock>());
      }

      public bool IsSatisfiedBy(ITreeNode treeNode, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         var buildingBlockNode = treeNode as UsedBuildingBlockInSimulationNode;
         if (buildingBlockNode == null) return false;
         return buildingBlockNode.TemplateBuildingBlock.IsAnImplementationOf<TBuildingBlock>();
      }
   }

   public class UsedIndividualInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Individual>
   {
      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Individual individual)
      {
         return new UsedBuidlingBlockInSimulationContextMenu<Individual>(simulation, usedBuildingBlock, individual);
      }
   }

   public class UsedCompoundInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Compound>
   {
      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Compound compound)
      {
         return new UsedBuidlingBlockInSimulationContextMenu<Compound>(simulation, usedBuildingBlock, compound);
      }
   }

   public class UsedFormulationInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Formulation>
   {
      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Formulation formulation)
      {
         return new UsedBuidlingBlockInSimulationContextMenu<Formulation>(simulation, usedBuildingBlock, formulation);
      }
   }

   public class UsedProtocolInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Protocol>
   {
      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Protocol protocol)
      {
         return new ProtocolInSimulationContextMenu(simulation, usedBuildingBlock, protocol);
      }
   }

   public class UsedEventInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<PKSimEvent>
   {
      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, PKSimEvent pkSimEvent)
      {
         return new UsedBuidlingBlockInSimulationContextMenu<PKSimEvent>(simulation, usedBuildingBlock, pkSimEvent);
      }
   }

   public class UsedPopulationInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Population>
   {
      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Population population)
      {
         return new PopulationInSimulationContextMenu(simulation, usedBuildingBlock, population);
      }
   }
}