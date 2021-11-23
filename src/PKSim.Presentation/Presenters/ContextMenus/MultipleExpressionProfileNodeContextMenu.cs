using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{


   public class MultipleExpressionProfileNodeContextMenu : MultipleBuildingBlockNodeContextMenu<ExpressionProfile>
   {
      public MultipleExpressionProfileNodeContextMenu(IReadOnlyList<ExpressionProfile> expressionProfiles, IExecutionContext executionContext) : base(expressionProfiles, executionContext)
      {
      }

      public MultipleExpressionProfileNodeContextMenu(IReadOnlyList<NamedBuildingBlock<ExpressionProfile>> expressionProfiles, IExecutionContext executionContext) : base(expressionProfiles, executionContext)
      {
      }
   }

   public class MultipleExpressionProfileNodeContextMenuFactory : MultipleNodeContextMenuFactory<ExpressionProfile>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleExpressionProfileNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ExpressionProfile> events, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleExpressionProfileNodeContextMenu(events, _executionContext);
      }
   }
}