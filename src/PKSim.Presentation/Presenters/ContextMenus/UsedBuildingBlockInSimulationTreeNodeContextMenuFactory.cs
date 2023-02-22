using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public abstract class UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<TBuildingBlock> : IContextMenuSpecificationFactory<ITreeNode>
   {
      protected readonly IContainer _container;

      protected UsedBuildingBlockInSimulationTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

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
      public UsedIndividualInSimulationTreeNodeContextMenuFactory(IContainer container) : base(container)
      {
      }

      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Individual individual)
      {
         return new UsedBuildingBlockInSimulationContextMenu<Individual>(simulation, usedBuildingBlock, individual, _container);
      }
   }

   public class UsedCompoundInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Compound>
   {
      public UsedCompoundInSimulationTreeNodeContextMenuFactory(IContainer container) : base(container)
      {
      }

      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Compound compound)
      {
         return new UsedBuildingBlockInSimulationContextMenu<Compound>(simulation, usedBuildingBlock, compound, _container);
      }
   }

   public class UsedFormulationInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Formulation>
   {
      public UsedFormulationInSimulationTreeNodeContextMenuFactory(IContainer container) : base(container)
      {
      }

      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Formulation formulation)
      {
         return new UsedBuildingBlockInSimulationContextMenu<Formulation>(simulation, usedBuildingBlock, formulation, _container);
      }
   }

   public class UsedProtocolInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Protocol>
   {
      public UsedProtocolInSimulationTreeNodeContextMenuFactory(IContainer container) : base(container)
      {
      }

      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Protocol protocol)
      {
         return new ProtocolInSimulationContextMenu(simulation, usedBuildingBlock, protocol, _container);
      }
   }

   public class UsedEventInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<PKSimEvent>
   {
      public UsedEventInSimulationTreeNodeContextMenuFactory(IContainer container) : base(container)
      {
      }

      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, PKSimEvent pkSimEvent)
      {
         return new UsedBuildingBlockInSimulationContextMenu<PKSimEvent>(simulation, usedBuildingBlock, pkSimEvent, _container);
      }
   }

   public class UsedPopulationInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<Population>
   {
      public UsedPopulationInSimulationTreeNodeContextMenuFactory(IContainer container) : base(container)
      {
      }

      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, Population population)
      {
         return new PopulationInSimulationContextMenu(simulation, usedBuildingBlock, population, _container);
      }
   }

   public class UsedObserverSetInSimulationTreeNodeContextMenuFactory : UsedBuildingBlockInSimulationTreeNodeContextMenuFactory<ObserverSet>
   {
      public UsedObserverSetInSimulationTreeNodeContextMenuFactory(IContainer container) : base(container)
      {
      }

      protected override IContextMenu CreateFor(Simulation simulation, UsedBuildingBlock usedBuildingBlock, ObserverSet observerSet)
      {
         return new UsedBuildingBlockInSimulationContextMenu<ObserverSet>(simulation, usedBuildingBlock, observerSet, _container);
      }
   }
}