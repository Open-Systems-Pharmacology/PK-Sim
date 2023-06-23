using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Nodes;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.UICommands;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleComparisonNodeContextMenu : ContextMenu<IReadOnlyList<ISimulationComparison>, IExecutionContext>
   {
      public MultipleComparisonNodeContextMenu(IReadOnlyList<ISimulationComparison> simulationComparisons, IExecutionContext executionContext, IContainer container) : base(simulationComparisons, executionContext, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<ISimulationComparison> simulationComparisons, IExecutionContext executionContext)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.Delete)
            .WithCommandFor<DeleteSimulationComparisonsUICommand, IReadOnlyList<ISimulationComparison>>(simulationComparisons, _container)
            .WithIcon(ApplicationIcons.Delete);
      }
   }

   public class MultipleComparisonNodeContextMenuFactory : MultipleNodeContextMenuFactory<ClassifiableComparison>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleComparisonNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ClassifiableComparison> classifiableComparisons, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleComparisonNodeContextMenu(classifiableComparisons.Select(x => x.Subject).ToList(), _executionContext, _container);
      }
   }
}