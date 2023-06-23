using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleSimulationNodeContextMenuFactory : MultipleNodeContextMenuFactory<ClassifiableSimulation>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleSimulationNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ClassifiableSimulation> classifiableSimulations, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleSimulationNodeContextMenu(classifiableSimulations.Select(x => x.Subject).ToList(), _executionContext, _container);
      }
   }

   public class MultipleSimulationNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Simulation>
   {
      public MultipleSimulationNodeContextMenu(IReadOnlyList<Simulation> simulations, IExecutionContext executionContext, IContainer container)
         : base(simulations, executionContext, container)
      {
      }

      public MultipleSimulationNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Simulation>> simulations, IExecutionContext executionContext, IContainer container)
         : base(simulations, executionContext, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<NamedBuildingBlock<Simulation>> simulations, IExecutionContext executionContext)
      {
         yield return CompareBuildingBlocks(simulations, executionContext);

         yield return AddToJournal(simulations);

         yield return startParameterIdentification(simulations);

         yield return DeleteSelectedBuildingBlockMenuItem(simulations);
      }

      private IMenuBarItem startParameterIdentification(IReadOnlyList<NamedBuildingBlock<Simulation>> simulations)
      {
         return ParameterIdentificationContextMenuItems.CreateParameterIdentificationFor(simulations.Select(x => x.BuildingBlock), _container);
      }
   }
}