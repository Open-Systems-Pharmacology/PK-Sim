using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Presenters.ContextMenus;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualMoleculesPresenter : IMoleculesPresenter, IIndividualItemPresenter
   {
   }

   public class IndividualMoleculesPresenter : MoleculesPresenter<Individual>, IIndividualMoleculesPresenter
   {
      public IndividualMoleculesPresenter(IMoleculesView view, IMoleculeExpressionTask<Individual> moleculeExpressionTask, ITreeNodeFactory treeNodeFactory, 
         ITreeNodeContextMenuFactory contextMenuFactory, IDialogCreator dialogCreator, IEntityTask entityTask,
         IRootNodeToIndividualExpressionsPresenterMapper<Individual> expressionsPresenterMapper, INoItemInSelectionPresenter noItemInSelectionPresenter)
         : base(view, moleculeExpressionTask, treeNodeFactory, contextMenuFactory, dialogCreator, entityTask, expressionsPresenterMapper, noItemInSelectionPresenter)
      {
      }

      public void EditIndividual(Individual individual)
      {
         Edit(individual);
      }
   }
}