using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleProtocolNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Protocol>
   {
      public MultipleProtocolNodeContextMenu(IReadOnlyList<Protocol> protocols, IExecutionContext executionContext, IContainer container) : base(protocols, executionContext, container)
      {
      }

      public MultipleProtocolNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Protocol>> protocols, IExecutionContext executionContext, IContainer container) : base(protocols, executionContext, container)
      {
      }
   }

   public class MultipleProtocolNodeContextMenuFactory : MultipleNodeContextMenuFactory<Protocol>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleProtocolNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Protocol> protocols, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleProtocolNodeContextMenu(protocols, _executionContext, _container);
      }
   }

}