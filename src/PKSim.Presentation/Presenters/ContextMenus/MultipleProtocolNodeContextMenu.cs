using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleProtocolNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Protocol>
   {
      public MultipleProtocolNodeContextMenu(IReadOnlyList<Protocol> protocols, IExecutionContext executionContext) : base(protocols, executionContext)
      {
      }

      public MultipleProtocolNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Protocol>> protocols, IExecutionContext executionContext) : base(protocols, executionContext)
      {
      }
   }

   public class MultipleProtocolNodeContextMenuFactory : MultipleNodeContextMenuFactory<Protocol>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleProtocolNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Protocol> protocols, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleProtocolNodeContextMenu(protocols, _executionContext);
      }
   }

}