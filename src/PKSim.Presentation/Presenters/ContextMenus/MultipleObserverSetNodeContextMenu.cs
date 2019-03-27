using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleObserverSetNodeContextMenu : MultipleBuildingBlockNodeContextMenu<ObserverSet>
   {
      public MultipleObserverSetNodeContextMenu(IReadOnlyList<ObserverSet> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }

      public MultipleObserverSetNodeContextMenu(IReadOnlyList<NamedBuildingBlock<ObserverSet>> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }
   }

   public class MultipleObserverSetNodeContextMenuFactory : MultipleNodeContextMenuFactory<ObserverSet>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleObserverSetNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ObserverSet> buildingBlocks, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleObserverSetNodeContextMenu(buildingBlocks, _executionContext);
      }
   }
}