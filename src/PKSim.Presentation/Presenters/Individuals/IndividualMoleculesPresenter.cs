using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Presenters.ContextMenus;
using PKSim.Presentation.Services;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualMoleculesPresenter : IMoleculesPresenter, IIndividualItemPresenter
   {
   }

   public class IndividualMoleculesPresenter : MoleculesPresenter<Individual>, IIndividualMoleculesPresenter
   {
      public IndividualMoleculesPresenter(IMoleculesView view, IEditMoleculeTask<Individual> editMoleculeTask, ITreeNodeFactory treeNodeFactory,
         ITreeNodeContextMenuFactory contextMenuFactory, IDialogCreator dialogCreator, IEntityTask entityTask,
         IRootNodeToIndividualExpressionsPresenterMapper<Individual> expressionsPresenterMapper,
         INoItemInSelectionPresenter noItemInSelectionPresenter)
         : base(view, editMoleculeTask, treeNodeFactory, contextMenuFactory, dialogCreator, entityTask, expressionsPresenterMapper,
            noItemInSelectionPresenter)
      {
      }

      public void EditIndividual(Individual individual)
      {
         Edit(individual);
      }
   }
}