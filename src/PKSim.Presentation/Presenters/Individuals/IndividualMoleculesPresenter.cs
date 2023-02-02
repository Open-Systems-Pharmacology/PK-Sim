using System;
using System.Collections.Generic;
using System.Drawing;
using OSPSuite.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
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

   public class NullIndividualMoleculesPresenter : IIndividualMoleculesPresenter
   {
      public void EditIndividual(Individual individualToEdit)
      {
         throw new NotSupportedException();
      }

      public void ShowContextMenu(ITreeNode objectRequestingPopup, Point popupLocation)
      {
         throw new NotSupportedException();
      }

      public void InitializeWith(ICommandCollector initializer)
      {
         throw new NotSupportedException();
      }

      public void ReleaseFrom(IEventPublisher eventPublisher)
      {
         throw new NotSupportedException();
      }

      public void ViewChanged()
      {
         throw new NotSupportedException();
      }

      public void Initialize()
      {
         throw new NotSupportedException();
      }

      public bool CanClose { get; }
      public IView BaseView { get; }
      public event EventHandler StatusChanged;
      public void RemoveMolecule(IndividualMolecule molecule)
      {
         throw new NotSupportedException();
      }

      public void AddMolecule<TMolecule>() where TMolecule : IndividualMolecule
      {
         throw new NotSupportedException();
      }

      public void ActivateNode(ITreeNode node)
      {
         throw new NotSupportedException();
      }

      public void NodeDoubleClicked(ITreeNode node)
      {
         throw new NotSupportedException();
      }

      public void AddCommand(ICommand command)
      {
         throw new NotSupportedException();
      }

      public IEnumerable<ICommand> All()
      {
         throw new NotImplementedException();
      }

      public ICommandCollector CommandCollector { get; }
      public ApplicationIcon Icon { get; }
   }
}