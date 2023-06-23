using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ImportPopulationContextMenu : PopulationContextMenu
   {
      public ImportPopulationContextMenu(ImportPopulation randomPopulation, IContainer container)
         : base(randomPopulation, container)
      {
      }
   }

   public class ImportPopulationTreeNodeContextMenuFactory : NodeContextMenuFactory<ImportPopulation>
   {
      private readonly IContainer _container;

      public ImportPopulationTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(ImportPopulation importPopulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ImportPopulationContextMenu(importPopulation, _container);
      }
   }
}