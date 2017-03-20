using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ImportPopulationContextMenu : PopulationContextMenu
   {
      public ImportPopulationContextMenu(ImportPopulation randomPopulation)
         : base(randomPopulation)
      {
      }
   }

   public class ImportPopulationTreeNodeContextMenuFactory : NodeContextMenuFactory<ImportPopulation>
   {
      public override IContextMenu CreateFor(ImportPopulation importPopulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new ImportPopulationContextMenu(importPopulation);
      }
   }
}