using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class RandomPopulationContextMenu : PopulationContextMenu
   {
      public RandomPopulationContextMenu(RandomPopulation randomPopulation) : base(randomPopulation)
      {
      }
   }

   public class RandomPopulationTreeNodeContextMenuFactory : NodeContextMenuFactory<RandomPopulation>
   {
      public override IContextMenu CreateFor(RandomPopulation randomPopulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new RandomPopulationContextMenu(randomPopulation);
      }
   }
}