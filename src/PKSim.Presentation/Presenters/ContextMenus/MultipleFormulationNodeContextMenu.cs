using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class MultipleFormulationNodeContextMenu : MultipleBuildingBlockNodeContextMenu<Formulation>
   {
      public MultipleFormulationNodeContextMenu(IReadOnlyList<Formulation> formulations, IExecutionContext executionContext) : base(formulations, executionContext)
      {
      }

      public MultipleFormulationNodeContextMenu(IReadOnlyList<NamedBuildingBlock<Formulation>> formulations, IExecutionContext executionContext) : base(formulations, executionContext)
      {
      }
   }

   public class MultipleFormulationNodeContextMenuFactory : MultipleNodeContextMenuFactory<Formulation>
   {
      private readonly IExecutionContext _executionContext;

      public MultipleFormulationNodeContextMenuFactory(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      protected override IContextMenu CreateFor(IReadOnlyList<Formulation> formulations, IPresenterWithContextMenu<IReadOnlyList<ITreeNode>> presenter)
      {
         return new MultipleFormulationNodeContextMenu(formulations, _executionContext);
      }
   }
}