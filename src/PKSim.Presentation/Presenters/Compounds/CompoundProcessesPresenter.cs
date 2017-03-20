using System;
using System.Collections.Generic;
using System.Drawing;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Views;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundProcessesPresenter : IPresenter<ICompoundProcessesView>,
      IListener<AddCompoundProcessEvent>,
      IListener<RemoveCompoundProcessEvent>,
      IListener<MoleculeRenamedInCompound>,
      ICompoundItemPresenter, IPresenterWithContextMenu<ITreeNode>
   {
      /// <summary>
      ///    Add an enzymatic partial process
      /// </summary>
      void AddEnzymaticPartialProcess();

      /// <summary>
      ///    Active the given node (e.g. as results of a user click)
      /// </summary>
      void ActivateNode(ITreeNode node);

      /// <summary>
      ///    Rename the datasource in the given partial process
      /// </summary>
      void RenameDataSourceInProcess(CompoundProcess partialProcess);

      /// <summary>
      ///    Delete the plasma clearance process
      /// </summary>
      void RemoveProcess(CompoundProcess compoundProcess);

      /// <summary>
      ///    Add a systemic process for the given systemic  process type
      /// </summary>
      /// <param name="systemicProcessType">Systemic process type to add</param>
      void AddSystemicProcess(IEnumerable<SystemicProcessType> systemicProcessType);

      /// <summary>
      ///    Add a specific binding process
      /// </summary>
      void AddSpecificBinding();

      /// <summary>
      ///    Add a transport process
      /// </summary>
      void AddTransport();

      /// <summary>
      ///    This method is called whenever a node is beind double cliked
      /// </summary>
      void NodeDoubleClicked(ITreeNode node);

      /// <summary>
      ///    Rename the molecule associated with processes of the given type
      /// </summary>
      void RenameMoleculeForPartialProcesses(string moleculeName, Type partialProcessType);

      /// <summary>
      ///    Add a partial process for the given molecule and process type
      /// </summary>
      void AddPartialProcessesForMolecule(string moleculeName, Type partialProcessType);

      /// <summary>
      ///    Adds a new inhibition process
      /// </summary>
      void AddInhibitionProcess();

      /// <summary>
      ///    Adds a new induction process
      /// </summary>
      void AddInductionProcess();
   }

   public class CompoundProcessesPresenter : AbstractSubPresenter<ICompoundProcessesView, ICompoundProcessesPresenter>, ICompoundProcessesPresenter
   {
      private readonly ICompoundProcessTask _compoundProcessTask;
      private readonly IPartialProcessToTreeNodeMapper _partialProcessNodeMapper;
      private readonly ITreeNodeContextMenuFactory _contextMenuFactory;
      private readonly ICompoundProcessPresenter _compoundProcessPresenter;
      private readonly IEnzymaticCompoundProcessPresenter _compoundEnzymaticProcessPresenter;
      private readonly IEntityTask _entityTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly INoItemInSelectionPresenter _noItemInSelectionPresenter;
      private readonly ITreeNodeFactory _treeNodeFactory;
      private Compound _compound;
      private readonly ICompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper _compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper;
      private readonly ICache<CompoundParameterNodeType, ICompoundParameterGroupPresenter> _parameterPresenterCache;
      private readonly IPartialProcessToRootNodeTypeMapper _partialProcessToRootNodeTypeMapper;
      private readonly ISystemicProcessToRootNodeTypeMapper _systemicProcessToRootNodeTypeMapper;

      public CompoundProcessesPresenter(ICompoundProcessesView view,
         ICompoundProcessTask compoundProcessTask,
         IPartialProcessToTreeNodeMapper partialProcessNodeMapper,
         ITreeNodeFactory treeNodeFactory,
         ITreeNodeContextMenuFactory contextMenuFactory,
         ICompoundProcessPresenter compoundProcessPresenter,
         IEntityTask entityTask, IDialogCreator dialogCreator, INoItemInSelectionPresenter noItemInSelectionPresenter,
         ICompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper,
         IEnzymaticCompoundProcessPresenter compoundEnzymaticProcessPresenter,
         IPartialProcessToRootNodeTypeMapper partialProcessToRootNodeTypeMapper,
         ISystemicProcessToRootNodeTypeMapper systemicProcessToRootNodeTypeMapper)
         : base(view)
      {
         _compoundProcessTask = compoundProcessTask;
         _partialProcessNodeMapper = partialProcessNodeMapper;
         _treeNodeFactory = treeNodeFactory;
         _contextMenuFactory = contextMenuFactory;
         _compoundProcessPresenter = compoundProcessPresenter;
         _entityTask = entityTask;
         _dialogCreator = dialogCreator;
         _noItemInSelectionPresenter = noItemInSelectionPresenter;
         _noItemInSelectionPresenter.Description = PKSimConstants.Information.CompoundProcessesInfo;
         _compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper = compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper;
         _compoundEnzymaticProcessPresenter = compoundEnzymaticProcessPresenter;
         _parameterPresenterCache = new Cache<CompoundParameterNodeType, ICompoundParameterGroupPresenter>();
         _partialProcessToRootNodeTypeMapper = partialProcessToRootNodeTypeMapper;
         _systemicProcessToRootNodeTypeMapper = systemicProcessToRootNodeTypeMapper;
         AddSubPresenters(_compoundProcessPresenter, _compoundEnzymaticProcessPresenter, _noItemInSelectionPresenter);
      }

      private void displayAllProcesses()
      {
         _view.Clear();
         _view.DestroyNodes();

         var metabolismNode = createMetabolismSubtree();
         var transportAndExcretionNode = createTransportAndExcretionSubtree();
         var distributionNode = createDistributionSubtree();
         var specificBindingNode = createSpecificBindingSubtree().Under(distributionNode);
         var absorptionNode = createAbsorptionSubtree();
         var inhibitionNode = createInhibitionSubtree();
         var inductionNode = createInductionSubtree();

         _view.AddNode(absorptionNode);
         _view.AddNode(distributionNode);
         _view.AddNode(metabolismNode);
         _view.AddNode(transportAndExcretionNode);
         _view.AddNode(inhibitionNode);
         _view.AddNode(inductionNode);

         addPartialProcessesToView<EnzymaticProcess>(PKSimRootNodeTypes.CompoundMetabolizingEnzymes);
         addPartialProcessesToView<SpecificBindingPartialProcess>(PKSimRootNodeTypes.CompoundProteinBindingPartners);
         addPartialProcessesToView<TransportPartialProcess>(PKSimRootNodeTypes.CompoundTransportProteins);
         addPartialProcessesToView<InhibitionProcess>(PKSimRootNodeTypes.InhibitionProcess);
         addPartialProcessesToView<InductionProcess>(PKSimRootNodeTypes.InductionProcess);

         _view.ExpandNode(metabolismNode);
         _view.ExpandNode(transportAndExcretionNode);
         _view.ExpandNode(distributionNode);
         _view.ExpandNode(absorptionNode);
         _view.ExpandNode(specificBindingNode);
      }

      private ITreeNode<RootNodeType> createInhibitionSubtree()
      {
         return _treeNodeFactory.CreateFor(PKSimRootNodeTypes.InhibitionProcess);
      }

      private ITreeNode<RootNodeType> createInductionSubtree()
      {
         return _treeNodeFactory.CreateFor(PKSimRootNodeTypes.InductionProcess);
      }

      private ITreeNode createAbsorptionSubtree()
      {
         var absorptionNode = _treeNodeFactory.CreateFor(PKSimRootNodeTypes.Absorption);
         _treeNodeFactory.CreateFor(CompoundParameterNodeType.SpecificIntestinalPermeability)
            .Under(absorptionNode);
         return absorptionNode;
      }

      private ITreeNode createSpecificBindingSubtree()
      {
         var specificBindingNode = _treeNodeFactory.CreateFor(PKSimRootNodeTypes.SpecificBindingProcesses);
         _treeNodeFactory.CreateFor(PKSimRootNodeTypes.CompoundProteinBindingPartners)
            .Under(specificBindingNode);
         return specificBindingNode;
      }

      private ITreeNode createDistributionSubtree()
      {
         var distributionNode = _treeNodeFactory.CreateFor(PKSimRootNodeTypes.Distribution);
         _treeNodeFactory.CreateFor(CompoundParameterNodeType.DistributionCalculation).Under(distributionNode);
         return distributionNode;
      }

      private ITreeNode createTransportAndExcretionSubtree()
      {
         var transportAndExcretionNode = _treeNodeFactory.CreateFor(PKSimRootNodeTypes.TransportAndExcretionProcesses);

         _treeNodeFactory.CreateFor(PKSimRootNodeTypes.CompoundTransportProteins)
            .Under(transportAndExcretionNode);

         var renalClearanceNode = _treeNodeFactory.CreateFor(SystemicProcessNodeType.RenalClearance)
            .Under(transportAndExcretionNode);

         var biliaryClearanceNode = _treeNodeFactory.CreateFor(SystemicProcessNodeType.BiliaryClearance)
            .Under(transportAndExcretionNode);

         addSystemicProcessesToTree(SystemicProcessTypes.Renal, renalClearanceNode);
         addSystemicProcessesToTree(SystemicProcessTypes.GFR, renalClearanceNode);
         addSystemicProcessesToTree(SystemicProcessTypes.Biliary, biliaryClearanceNode);
         return transportAndExcretionNode;
      }

      private ITreeNode createMetabolismSubtree()
      {
         var metabolismNode = _treeNodeFactory.CreateFor(PKSimRootNodeTypes.MetabolicProcesses);

         _treeNodeFactory.CreateFor(PKSimRootNodeTypes.CompoundMetabolizingEnzymes)
            .Under(metabolismNode);

         var hepaticClearanceNode = _treeNodeFactory.CreateFor(SystemicProcessNodeType.HepaticClearance)
            .Under(metabolismNode);
         addSystemicProcessesToTree(SystemicProcessTypes.Hepatic, hepaticClearanceNode);
         return metabolismNode;
      }

      private void addSystemicProcessesToTree(SystemicProcessType systemicProcessType, ITreeNode systemicProcessContainerNode)
      {
         foreach (var process in _compound.AllSystemicProcessesOfType(systemicProcessType))
         {
            addSystemicProcessToTree(process, systemicProcessContainerNode);
         }
      }

      private ITreeNode addSystemicProcessToTree(SystemicProcess systemicProcess, ITreeNode systemicProcessContainerNode)
      {
         return _treeNodeFactory.CreateFor(systemicProcess).Under(systemicProcessContainerNode);
      }

      private void addSystemicProcessToView(SystemicProcess systemicProcess)
      {
         var node = addSystemicProcessToTree(systemicProcess, _view.NodeByType(_systemicProcessToRootNodeTypeMapper.MapFrom(systemicProcess.SystemicProcessType)));
         _view.AddNode(node);
      }

      private void addPartialProcessesToView<TPartialProcess>(RootNodeType parentNodeType) where TPartialProcess : PartialProcess
      {
         _compound.AllProcesses<TPartialProcess>().Each(p => addPartialProcessToView(p, parentNodeType));
      }

      private void addPartialProcessToView(PartialProcess partialProcess, RootNodeType parentNodeType)
      {
         var partialProcessesNode = _view.NodeByType(parentNodeType);
         var moleculeNode = _partialProcessNodeMapper.MapFrom(partialProcess);
         var existingMoleculeNode = _view.NodeById(moleculeNode.Id);
         //a node for the molecule was already added, promote the children
         if (existingMoleculeNode != null)
            moleculeNode.Children.Each(existingMoleculeNode.AddChild);
         else
            partialProcessesNode.AddChild(moleculeNode);

         //this refreshed whole process node
         _view.AddNode(partialProcessesNode);
      }

      private void addPartialProcessToView(PartialProcess partialProcess)
      {
         addPartialProcessToView(partialProcess, _partialProcessToRootNodeTypeMapper.MapFrom(partialProcess));
      }

      public void AddEnzymaticPartialProcess()
      {
         AddCommand(_compoundProcessTask.CreateEnzymaticProcessFor(_compound));
      }

      public void EditCompound(Compound compound)
      {
         _compound = compound;
         displayAllProcesses();
         _view.SelectFocusedNodeOrFirst();
      }

      public void NodeDoubleClicked(ITreeNode node)
      {
         View.ToggleExpandState(node);
      }

      public void RenameMoleculeForPartialProcesses(string moleculeName, Type partialProcessType)
      {
         AddCommand(_compoundProcessTask.RenameMoleculeForPartialProcessesIn(_compound, moleculeName, partialProcessType));
      }

      public void AddPartialProcessesForMolecule(string moleculeName, Type partialProcessType)
      {
         AddCommand(_compoundProcessTask.AddPartialProcessesForMolecule(_compound, moleculeName, partialProcessType));
      }

      public void AddInhibitionProcess()
      {
         AddCommand(_compoundProcessTask.CreateInhibitionProcessFor(_compound));
      }

      public void AddInductionProcess()
      {
         AddCommand(_compoundProcessTask.CreateInductionProcessFor(_compound));
      }

      public void ActivateNode(ITreeNode node)
      {
         if (node == null) return;
         _view.GroupCaption = node.FullPath(PKSimConstants.UI.DisplayPathSeparator);

         var enzymaticProcess = node.TagAsObject as EnzymaticProcess;
         if (enzymaticProcess != null)
         {
            _view.ActivateView(_compoundEnzymaticProcessPresenter.BaseView);
            _compoundEnzymaticProcessPresenter.Edit(enzymaticProcess);
            return;
         }

         var process = node.TagAsObject as CompoundProcess;
         if (process != null)
         {
            _view.ActivateView(_compoundProcessPresenter.BaseView);
            _compoundProcessPresenter.Edit(process);
            return;
         }

         var definition = node.TagAsObject as CompoundParameterNodeType;
         var presenter = compoundParameterGroupPresenterFor(definition);
         if (canEditCompoundParameter(definition, presenter))
         {
            _view.ActivateView(presenter.BaseView);
            presenter.EditCompound(_compound);
            return;
         }

         //nothing to display: Reset view
         _view.ActivateView(_noItemInSelectionPresenter.BaseView);
      }

      private static bool canEditCompoundParameter(CompoundParameterNodeType definition, ICompoundParameterGroupPresenter presenter)
      {
         return definition != null && presenter != null;
      }

      private ICompoundParameterGroupPresenter compoundParameterGroupPresenterFor(CompoundParameterNodeType compoundParameterNodeType)
      {
         if (compoundParameterNodeType == null)
            return null;
         if (_parameterPresenterCache.Contains(compoundParameterNodeType))
            return _parameterPresenterCache[compoundParameterNodeType];

         var presenter = _compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper.MapFrom(compoundParameterNodeType);
         if (presenter == null)
            return null;

         _parameterPresenterCache.Add(compoundParameterNodeType, presenter);
         presenter.InitializeWith(CommandCollector);
         return presenter;
      }

      public void RenameDataSourceInProcess(CompoundProcess compoundProcess)
      {
         AddCommand(_compoundProcessTask.RenameDataSource(compoundProcess));
      }

      public void RemoveProcess(CompoundProcess compoundProcess)
      {
         var question = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteProcess(_entityTask.TypeFor(compoundProcess), compoundProcess.Name));
         if (question == ViewResult.No) return;
         AddCommand(_compoundProcessTask.RemoveProcess(_compound, compoundProcess));
      }

      public void AddSystemicProcess(IEnumerable<SystemicProcessType> systemicProcessType)
      {
         AddCommand(_compoundProcessTask.CreateSystemicProcessFor(_compound, systemicProcessType));
      }

      public void AddSpecificBinding()
      {
         AddCommand(_compoundProcessTask.CreateSpecificBindingFor(_compound));
      }

      public void AddTransport()
      {
         AddCommand(_compoundProcessTask.CreateTransportFor(_compound));
      }

      public override void ReleaseFrom(IEventPublisher eventPublisher)
      {
         base.ReleaseFrom(eventPublisher);
         _parameterPresenterCache.Each(presenter => presenter.ReleaseFrom(eventPublisher));
         _parameterPresenterCache.Clear();
         _view.DestroyNodes();
      }

      public void ShowContextMenu(ITreeNode treeNode, Point popupLocation)
      {
         var contextMenu = _contextMenuFactory.CreateFor(treeNode, this);
         contextMenu.Show(_view, popupLocation);
      }

      public void Handle(AddCompoundProcessEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         var systemicProcess = eventToHandle.Entity as SystemicProcess;
         if (systemicProcess != null)
            addSystemicProcessToView(systemicProcess);
         else
            addPartialProcessToView(eventToHandle.Entity.DowncastTo<PartialProcess>());

         //once a process has been added, focus the node corresponding to the process
         _view.SelectNode(_view.NodeById(eventToHandle.Entity.Id));
      }

      public void Handle(RemoveCompoundProcessEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         displayAllProcesses();
      }

      private bool canHandle(ICompoundEvent compoundEvent)
      {
         return Equals(compoundEvent.Compound, _compound);
      }

      public void Handle(MoleculeRenamedInCompound eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         displayAllProcesses();
      }
   }
}