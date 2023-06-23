using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleIndividualNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Individual>
   {
      public MultipleIndividualNodeContextMenu(IReadOnlyList<Individual> individuals, IExecutionContext executionContext, IContainer container) : base(individuals, executionContext, container)
      {
      }

      public MultipleIndividualNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Individual>> individuals, IExecutionContext executionContext, IContainer container) : base(individuals, executionContext, container)
      {
      }
   }

   public class MultipleIndividualNodeContextMenuFactory : MultipleNodeContextMenuFactory<Individual>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleIndividualNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Individual> individuals, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleIndividualNodeContextMenu(individuals, _executionContext, _container);
      }
   }

}