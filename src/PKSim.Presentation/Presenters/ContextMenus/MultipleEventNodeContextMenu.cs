using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleEventNodeContextMenu : MultipleBuildingBlockNodeContextMenu<PKSimEvent>
   {
      public MultipleEventNodeContextMenu(IReadOnlyList<PKSimEvent> events, IExecutionContext executionContext) : base(events, executionContext)
      {
      }

      public MultipleEventNodeContextMenu(IReadOnlyList<NamedBuildingBlock<PKSimEvent>> events, IExecutionContext executionContext) : base(events, executionContext)
      {
      }
   }

   public class MultipleEventNodeContextMenuFactory : MultipleNodeContextMenuFactory<PKSimEvent>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleEventNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<PKSimEvent> events, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleEventNodeContextMenu(events, _executionContext);
      }
   }
}