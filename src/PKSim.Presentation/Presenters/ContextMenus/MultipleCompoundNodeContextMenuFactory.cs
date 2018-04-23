using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleCompoundNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Compound>
   {
      public MultipleCompoundNodeContextMenu(IReadOnlyList<Compound> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }

      public MultipleCompoundNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Compound>> buildingBlocks, IExecutionContext executionContext)
         : base(buildingBlocks, executionContext)
      {
      }
   }

   public class MultipleCompoundNodeContextMenuFactory : MultipleNodeContextMenuFactory<Compound>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleCompoundNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Compound> buildingBlocks, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleCompoundNodeContextMenu(buildingBlocks, _executionContext);
      }
   }
}