using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{


   public class MultipleExpressionProfileNodeContextMenu : MultipleBuildingBlockNodeContextMenu<ExpressionProfile>
   {
      public MultipleExpressionProfileNodeContextMenu(IReadOnlyList<ExpressionProfile> expressionProfiles, IExecutionContext executionContext, IContainer container) : base(expressionProfiles, executionContext, container)
      {
      }

      public MultipleExpressionProfileNodeContextMenu(IReadOnlyList<NamedBuildingBlock<ExpressionProfile>> expressionProfiles, IExecutionContext executionContext, IContainer container) : base(expressionProfiles, executionContext, container)
      {
      }
   }

   public class MultipleExpressionProfileNodeContextMenuFactory : MultipleNodeContextMenuFactory<ExpressionProfile>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleExpressionProfileNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<ExpressionProfile> events, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleExpressionProfileNodeContextMenu(events, _executionContext, _container);
      }
   }
}