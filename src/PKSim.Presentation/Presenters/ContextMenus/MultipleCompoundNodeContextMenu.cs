using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleCompoundNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Compound>
   {
      public MultipleCompoundNodeContextMenu(IReadOnlyList<Compound> buildingBlocks, IExecutionContext executionContext, IContainer container)
         : base(buildingBlocks, executionContext, container)
      {
      }

      public MultipleCompoundNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Compound>> buildingBlocks, IExecutionContext executionContext, IContainer container)
         : base(buildingBlocks, executionContext, container)
      {
      }
   }

   public class MultipleCompoundNodeContextMenuFactory : MultipleNodeContextMenuFactory<Compound>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleCompoundNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Compound> buildingBlocks, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleCompoundNodeContextMenu(buildingBlocks, _executionContext, _container);
      }
   }
}