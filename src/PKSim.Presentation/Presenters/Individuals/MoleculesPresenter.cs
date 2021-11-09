using System.Drawing;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IMoleculesPresenter :
      IPresenterWithContextMenu<ITreeNode>
   {
      /// <summary>
      ///    rename the given protein
      /// </summary>
      void RenameMolecule(IndividualMolecule molecule);

      /// <summary>
      ///    Can we perform an edit on the protein (edit using the protein expression database)
      /// </summary>
      bool EditConfigurationEnabledFor(IndividualMolecule molecule);

      /// <summary>
      ///    Can we perform an edit on the individual (edit using the protein expression database)
      /// </summary>
      bool QueryConfigurationEnabled { get; }

      /// <summary>
      ///    Launch the protein edition
      /// </summary>
      void EditMolecule(IndividualMolecule molecule);

      /// <summary>
      ///    Remove the given protein
      /// </summary>
      void RemoveMolecule(IndividualMolecule molecule);

      /// <summary>
      ///    Add a new protein of the given type to the individual
      /// </summary>
      void AddMolecule<TMolecule>() where TMolecule : IndividualMolecule;

      /// <summary>
      ///    The node given as parameter was selected
      /// </summary>
      void ActivateNode(ITreeNode node);

      /// <summary>
      ///    The node given as parameter was double clicked
      /// </summary>
      void NodeDoubleClicked(ITreeNode node);
   }

   public abstract class MoleculesPresenter<TSimulationSubject> : AbstractSubPresenter<IMoleculesView, IMoleculesPresenter>,
      IMoleculesPresenter,
      IListener<AddMoleculeToSimulationSubjectEvent<TSimulationSubject>>,
      IListener<RemoveMoleculeFromSimulationSubjectEvent<TSimulationSubject>>,
      IListener<RefreshMoleculeInSimulationSubjectEvent<TSimulationSubject>>
      where TSimulationSubject : ISimulationSubject
   {
      private readonly IEditMoleculeTask<TSimulationSubject> _editMoleculeTask;
      private readonly ITreeNodeFactory _treeNodeFactory;
      private readonly ITreeNodeContextMenuFactory _contextMenuFactory;
      private readonly IDialogCreator _dialogCreator;
      private readonly IEntityTask _entityTask;
      private readonly IRootNodeToIndividualExpressionsPresenterMapper<TSimulationSubject> _expressionsPresenterMapper;
      private readonly INoItemInSelectionPresenter _noItemInSelectionPresenter;
      private TSimulationSubject _simulationSubject;
      private readonly ICache<RootNode, IIndividualMoleculeExpressionsPresenter> _expressionsPresenterCache;
      private IIndividualMoleculeExpressionsPresenter _activePresenter;

      protected MoleculesPresenter(IMoleculesView view,
         IEditMoleculeTask<TSimulationSubject> editMoleculeTask,
         ITreeNodeFactory treeNodeFactory,
         ITreeNodeContextMenuFactory contextMenuFactory,
         IDialogCreator dialogCreator,
         IEntityTask entityTask,
         IRootNodeToIndividualExpressionsPresenterMapper<TSimulationSubject> expressionsPresenterMapper,
         INoItemInSelectionPresenter noItemInSelectionPresenter)
         : base(view)
      {
         _editMoleculeTask = editMoleculeTask;
         _treeNodeFactory = treeNodeFactory;
         _contextMenuFactory = contextMenuFactory;
         _dialogCreator = dialogCreator;
         _entityTask = entityTask;
         _expressionsPresenterMapper = expressionsPresenterMapper;
         _noItemInSelectionPresenter = noItemInSelectionPresenter;
         _noItemInSelectionPresenter.Description = PKSimConstants.Information.IndividualExpressionInfo;
         _expressionsPresenterCache = new Cache<RootNode, IIndividualMoleculeExpressionsPresenter>();
      }

      public virtual void Edit(TSimulationSubject simulationSubject)
      {
         _simulationSubject = simulationSubject;

         _view.DestroyNodes();
         _expressionsPresenterCache.Clear();

         //add root nodes
         _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualMetabolizingEnzymes));
         _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualTransportProteins));
         _view.AddNode(_treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualProteinBindingPartners));
         addIndividualProteins<IndividualEnzyme>(PKSimRootNodeTypes.IndividualMetabolizingEnzymes, ApplicationIcons.EmptyIcon);
         addIndividualProteins<IndividualOtherProtein>(PKSimRootNodeTypes.IndividualProteinBindingPartners, ApplicationIcons.EmptyIcon);
         addIndividualProteins<IndividualTransporter>(PKSimRootNodeTypes.IndividualTransportProteins, ApplicationIcons.EmptyIcon);

         _view.ExpandAllNodes();

         //select first node
         _view.SelectNode(_view.NodeByType(PKSimRootNodeTypes.IndividualMetabolizingEnzymes));
      }

      private void addIndividualProteins<TMolecule>(RootNodeType proteinContainerType, ApplicationIcon icon) where TMolecule : IndividualMolecule
      {
         var proteinContainerNode = _view.NodeByType(proteinContainerType);
         _simulationSubject.AllMolecules<TMolecule>()
            .Each(protein => _view.AddNode(_treeNodeFactory.CreateFor(protein)
               .WithIcon(icon)
               .Under(proteinContainerNode)));
      }

      public void ShowContextMenu(ITreeNode nodeRequestingPopup, Point popupLocation)
      {
         var contextMenu = _contextMenuFactory.CreateFor(nodeRequestingPopup, this);
         contextMenu.Show(_view, popupLocation);
      }

      public bool QueryConfigurationEnabled => _editMoleculeTask.CanQueryProteinExpressionsFor(_simulationSubject);

      public bool EditConfigurationEnabledFor(IndividualMolecule molecule)
      {
         return QueryConfigurationEnabled && molecule.HasQuery();
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _expressionsPresenterCache.Each(p => p.ReleaseFrom(eventPublisher));
         _expressionsPresenterCache.Each(p => p.BaseView.Dispose());
         _expressionsPresenterCache.Clear();
         _activePresenter = null;
      }

      public void ActivateNode(ITreeNode node)
      {
         if (node == null) return;
         //one of the root has been selected
         _view.GroupCaption = node.FullPath(PKSimConstants.UI.DisplayPathSeparator);
         if (nodeRepresentsMoleculeFolder(node))
         {
            _view.ActivateView(_noItemInSelectionPresenter.BaseView);
            return;
         }

         var rootNode = node.ParentNode.DowncastTo<RootNode>();
         _activePresenter = presenterFor(rootNode);
         //needs to be done as soon as the view is available to allow proper resizing
         _view.ActivateView(_activePresenter.BaseView);
         _activePresenter.OntogenyVisible = _simulationSubject.IsAgeDependent;
         _activePresenter.MoleculeParametersVisible = _simulationSubject.IsAnImplementationOf<Individual>();
         _activePresenter.ActivateMolecule(moleculeFrom(node));
      }

      public void NodeDoubleClicked(ITreeNode node)
      {
         View.ToggleExpandState(node);
      }

      private IIndividualMoleculeExpressionsPresenter presenterFor(RootNode node)
      {
         if (!_expressionsPresenterCache.Contains(node))
         {
            var presenter = _expressionsPresenterMapper.MapFrom(node);
            presenter.SimulationSubject = _simulationSubject;
            presenter.InitializeWith(CommandCollector);
            presenter.StatusChanged += OnStatusChanged;
            _expressionsPresenterCache.Add(node, presenter);
         }

         return _expressionsPresenterCache[node];
      }

      public override bool CanClose => _activePresenter == null || _activePresenter.CanClose;

      private bool nodeRepresentsMoleculeFolder(ITreeNode moleculeNode)
      {
         return moleculeNode == null || moleculeNode.IsAnImplementationOf<RootNode>();
      }

      private IndividualMolecule moleculeFrom(ITreeNode moleculeNode)
      {
         return moleculeNode?.TagAsObject as IndividualMolecule;
      }

      public void RenameMolecule(IndividualMolecule molecule)
      {
         var newName = _entityTask.NewNameFor(molecule, _simulationSubject.AllMolecules().AllNames());
         if(string.IsNullOrEmpty(newName))
            return;
         
         AddCommand(_editMoleculeTask.RenameMolecule(molecule, newName, _simulationSubject));
         editMolecule(molecule);
      }

      public void EditMolecule(IndividualMolecule molecule)
      {
         if (!EditConfigurationEnabledFor(molecule))
            return;

         AddCommand(_editMoleculeTask.EditMolecule(molecule, _simulationSubject));
      }

      private void editMolecule(IndividualMolecule molecule)
      {
         //user factory to retrieve a node that we will use to refresh the view
         var node = _treeNodeFactory.CreateFor(molecule);
         _view.SelectNode(_view.NodeById(node.Id));
      }

      public void RemoveMolecule(IndividualMolecule molecule)
      {
         var viewResult = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteProtein(_entityTask.TypeFor(molecule), molecule.Name));
         if (viewResult == ViewResult.No) return;

         AddCommand(_editMoleculeTask.RemoveMoleculeFrom(molecule, _simulationSubject));
      }

      public virtual void AddMolecule<TMolecule>() where TMolecule : IndividualMolecule
      {
         AddCommand(_editMoleculeTask.AddMoleculeTo<TMolecule>(_simulationSubject));
      }

      public void Handle(AddMoleculeToSimulationSubjectEvent<TSimulationSubject> eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;

         Edit(_simulationSubject);

         //select node for protein
         editMolecule(eventToHandle.Entity);
      }

      public void Handle(RemoveMoleculeFromSimulationSubjectEvent<TSimulationSubject> eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         Edit(_simulationSubject);
      }

      private bool canHandle(ISimulationSubjectEvent simulationSubjectEvent)
      {
         if (_simulationSubject == null) return false;
         return Equals(simulationSubjectEvent.SimulationSubject, _simulationSubject);
      }

      public void Handle(RefreshMoleculeInSimulationSubjectEvent<TSimulationSubject> eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         editMolecule(eventToHandle.Entity);
      }
   }
}