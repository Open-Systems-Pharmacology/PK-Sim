using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleSimulationNodeContextMenuFactory : MultipleNodeContextMenuFactory<ClassifiableSimulation>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleSimulationNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ClassifiableSimulation> classifiableSimulations, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleSimulationNodeContextMenu(classifiableSimulations.Select(x => x.Subject).ToList(), _executionContext);
      }
   }

   public class MultipleSimulationNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Simulation>
   {
      public MultipleSimulationNodeContextMenu(IReadOnlyList<Simulation> simulations, IExecutionContext executionContext)
         : base(simulations, executionContext)
      {
      }

      public MultipleSimulationNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Simulation>> simulations, IExecutionContext executionContext)
         : base(simulations, executionContext)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<NamedBuildingBlock<Simulation>> simulations, IExecutionContext executionContext)
      {
         yield return CompareBuildingBlocks(simulations, executionContext);

         yield return AddToJournal(simulations);

         yield return startParameterIdentifcation(simulations);

         yield return DeleteSelectedBuildingBlockMenuItem(simulations);
      }

      private static IMenuBarItem startParameterIdentifcation(IReadOnlyList<NamedBuildingBlock<Simulation>> simulations)
      {
         return ParameterIdentificationContextMenuItems.CreateParameterIdentificationFor(simulations.Select(x => x.BuildingBlock));
      }
   }
}