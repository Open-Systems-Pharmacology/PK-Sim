using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleEventNodeContextMenu : MultipleBuildingBlockNodeContextMenu<PKSimEvent>
   {
      public MultipleEventNodeContextMenu(IReadOnlyList<PKSimEvent> events, IExecutionContext executionContext, IContainer container) : base(events, executionContext, container)
      {
      }

      public MultipleEventNodeContextMenu(IReadOnlyList<NamedBuildingBlock<PKSimEvent>> events, IExecutionContext executionContext, IContainer container) : base(events, executionContext, container)
      {
      }
   }

   public class MultipleEventNodeContextMenuFactory : MultipleNodeContextMenuFactory<PKSimEvent>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleEventNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<PKSimEvent> events, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleEventNodeContextMenu(events, _executionContext, _container);
      }
   }
}