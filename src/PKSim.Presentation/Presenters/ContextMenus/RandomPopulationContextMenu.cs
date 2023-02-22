using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class RandomPopulationContextMenu : PopulationContextMenu
   {
      public RandomPopulationContextMenu(RandomPopulation randomPopulation, IContainer container) : base(randomPopulation, container)
      {
      }
   }

   public class RandomPopulationTreeNodeContextMenuFactory : NodeContextMenuFactory<RandomPopulation>
   {
      private readonly IContainer _container;

      public RandomPopulationTreeNodeContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public override IContextMenu CreateFor(RandomPopulation randomPopulation, IPresenterWithContextMenu<ITreeNode> presenter)
      {
         return new RandomPopulationContextMenu(randomPopulation, _container);
      }
   }
}