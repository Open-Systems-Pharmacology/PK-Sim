using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleObserverSetNodeContextMenu : MultipleBuildingBlockNodeContextMenu<ObserverSet>
   {
      public MultipleObserverSetNodeContextMenu(IReadOnlyList<ObserverSet> buildingBlocks, IExecutionContext executionContext, IContainer container)
         : base(buildingBlocks, executionContext, container)
      {
      }

      public MultipleObserverSetNodeContextMenu(IReadOnlyList<NamedBuildingBlock<ObserverSet>> buildingBlocks, IExecutionContext executionContext, IContainer container)
         : base(buildingBlocks, executionContext, container)
      {
      }
   }

   public class MultipleObserverSetNodeContextMenuFactory : MultipleNodeContextMenuFactory<ObserverSet>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleObserverSetNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ObserverSet> buildingBlocks, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleObserverSetNodeContextMenu(buildingBlocks, _executionContext, _container);
      }
   }
}