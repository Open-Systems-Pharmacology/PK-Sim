using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleIndividualNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Individual>
   {
      public MultipleIndividualNodeContextMenu(IReadOnlyList<Individual> individuals, IExecutionContext executionContext) : base(individuals, executionContext)
      {
      }

      public MultipleIndividualNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Individual>> individuals, IExecutionContext executionContext) : base(individuals, executionContext)
      {
      }
   }

   public class MultipleIndividualNodeContextMenuFactory : MultipleNodeContextMenuFactory<Individual>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleIndividualNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Individual> individuals, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleIndividualNodeContextMenu(individuals, _executionContext);
      }
   }

}