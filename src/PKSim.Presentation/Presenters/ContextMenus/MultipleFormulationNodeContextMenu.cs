using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleFormulationNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Formulation>
   {
      public MultipleFormulationNodeContextMenu(IReadOnlyList<Formulation> formulations, IExecutionContext executionContext, IContainer container) : base(formulations, executionContext, container)
      {
      }

      public MultipleFormulationNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Formulation>> formulations, IExecutionContext executionContext, IContainer container) : base(formulations, executionContext, container)
      {
      }
   }

   public class MultipleFormulationNodeContextMenuFactory : MultipleNodeContextMenuFactory<Formulation>
   {
      private readonly IExecutionContext _executionContext;
      private readonly IContainer _container;

      public MultipleFormulationNodeContextMenuFactory(IExecutionContext executionContext, IContainer container)
      {
         _executionContext = executionContext;
         _container = container;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Formulation> formulations, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleFormulationNodeContextMenu(formulations, _executionContext, _container);
      }
   }
}