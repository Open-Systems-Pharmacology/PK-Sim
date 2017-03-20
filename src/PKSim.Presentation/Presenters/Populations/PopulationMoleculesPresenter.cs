using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IPopulationMoleculesPresenter : IMoleculesPresenter, IPopulationItemPresenter
   {
      void EditPopulation(Population population);
   }

   public class PopulationMoleculesPresenter : MoleculesPresenter<Population>, IPopulationMoleculesPresenter
   {
      public PopulationMoleculesPresenter(IMoleculesView view, IMoleculeExpressionTask<Population> moleculeExpressionTask, ITreeNodeFactory treeNodeFactory,
         ITreeNodeContextMenuFactory contextMenuFactory, IDialogCreator dialogCreator, IEntityTask entityTask,
         IRootNodeToIndividualExpressionsPresenterMapper<Population> expressionsPresenterMapper, INoItemInSelectionPresenter noItemInSelectionPresenter)
         : base(view, moleculeExpressionTask, treeNodeFactory, contextMenuFactory, dialogCreator, entityTask, expressionsPresenterMapper, noItemInSelectionPresenter)
      {
      }

      public void EditPopulation(Population population)
      {
         Edit(population);
      }
   }
}