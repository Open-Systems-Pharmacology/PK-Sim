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

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleComparisonNodeContextMenu : ContextMenu<IReadOnlyList<ISimulationComparison>, IExecutionContext>
   {
      public MultipleComparisonNodeContextMenu(IReadOnlyList<ISimulationComparison> simulationComparisons, IExecutionContext executionContext) : base(simulationComparisons, executionContext)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IReadOnlyList<ISimulationComparison> simulationComparisons, IExecutionContext executionContext)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.Delete)
            .WithCommandFor<DeleteSimulationComparisonsUICommand, IReadOnlyList<ISimulationComparison>>(simulationComparisons)
            .WithIcon(ApplicationIcons.Delete);
      }
   }

   public class MultipleComparisonNodeContextMenuFactory : MultipleNodeContextMenuFactory<ClassifiableComparison>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleComparisonNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ClassifiableComparison> classifiableComparisons, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleComparisonNodeContextMenu(classifiableComparisons.Select(x => x.Subject).ToList(), _executionContext);
      }
   }
}