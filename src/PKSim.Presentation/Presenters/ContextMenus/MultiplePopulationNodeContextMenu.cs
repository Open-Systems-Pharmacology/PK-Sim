using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultiplePopulationNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Population>
   {
      public MultiplePopulationNodeContextMenu(IReadOnlyList<Population> buildingBlocks, IExecutionContext executionContext, IContainer container)
         : base(buildingBlocks, executionContext, container)
      {
      }

      public MultiplePopulationNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Population>> buildingBlocks, IExecutionContext executionContext, IContainer container)
         : base(buildingBlocks, executionContext, container)
      {
      }
   }

   public class MultiplePopulationNodeContextMenuFactory : MultipleNodeContextMenuFactory<Population>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultiplePopulationNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Population> buildingBlocks, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultiplePopulationNodeContextMenu(buildingBlocks, _executionContext, _container);
      }
   }
}