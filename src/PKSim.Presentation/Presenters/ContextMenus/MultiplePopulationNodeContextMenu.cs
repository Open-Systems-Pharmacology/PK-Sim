using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultiplePopulationNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Population>
   {
      public MultiplePopulationNodeContextMenu(IReadOnlyList<Population> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }

      public MultiplePopulationNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Population>> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }
   }

   public class MultiplePopulationNodeContextMenuFactory : MultipleNodeContextMenuFactory<Population>
   {
      private readonly IExecutionContext _executionContext;

      public MultiplePopulationNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Population> buildingBlocks, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultiplePopulationNodeContextMenu(buildingBlocks, _executionContext);
      }
   }
}