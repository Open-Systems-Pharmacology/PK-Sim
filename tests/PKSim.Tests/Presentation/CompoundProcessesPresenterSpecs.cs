using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Views;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundProcessesPresenter : ContextSpecification<ICompoundProcessesPresenter>
   {
      protected ICompoundProcessesView _view;
      protected Compound _compound;
      protected ICompoundProcessTask _compoundProcessTask;
      protected IPartialProcessToTreeNodeMapper _partialProcessNodeMapper;
      protected ITreeNodeFactory _treeNodeFactory;
      protected ITreeNodeContextMenuFactory _contextMenuFactory;
      protected RootNode _partialProcessesNode;
      protected IEntityTask _entityTask;
      protected ICommandCollector _commandRegister;
      protected ICompoundProcessPresenter _compoundProcessPresenter;
      protected IDialogCreator _dialogCreator;
      protected INoItemInSelectionPresenter _noItemInSelectionPresenter;
      protected ICompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper _compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper;
      protected ICompoundParameterGroupPresenter _presenter;
      protected IEnzymaticCompoundProcessPresenter _compoundEnzymaticProcessPresenter;
      protected ICompoundProcessPresentationTask _compoundProcessPresentationTask;

      protected override void Context()
      {
         _compound = new Compound();
         _view = A.Fake<ICompoundProcessesView>();
         _commandRegister = A.Fake<ICommandCollector>();
         A.CallTo(() => _view.TreeView).Returns(A.Fake<IUxTreeView>());
         _compoundProcessTask = A.Fake<ICompoundProcessTask>();
         _compoundProcessPresentationTask = A.Fake<ICompoundProcessPresentationTask>();
         _partialProcessNodeMapper = A.Fake<IPartialProcessToTreeNodeMapper>();
         _compoundProcessPresenter = A.Fake<ICompoundProcessPresenter>();
         _compoundEnzymaticProcessPresenter = A.Fake<IEnzymaticCompoundProcessPresenter>();
         _treeNodeFactory = new TreeNodeFactoryForSpecs();
         _contextMenuFactory = A.Fake<ITreeNodeContextMenuFactory>();
         _entityTask = A.Fake<IEntityTask>();
         _partialProcessesNode = new RootNode(PKSimRootNodeTypes.CompoundMetabolizingEnzymes);
         A.Fake<IRepresentationInfoRepository>();
         _compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper = A.Fake<ICompoundParameterNodeTypeToCompoundParameterGroupPresenterMapper>();

         _dialogCreator = A.Fake<IDialogCreator>();
         _noItemInSelectionPresenter = A.Fake<INoItemInSelectionPresenter>();

         sut = new CompoundProcessesPresenter(_view, _compoundProcessTask, _compoundProcessPresentationTask, _partialProcessNodeMapper, _treeNodeFactory,
            _contextMenuFactory, _compoundProcessPresenter, _entityTask, _dialogCreator, _noItemInSelectionPresenter,
            _compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper, _compoundEnzymaticProcessPresenter,
            new PartialProcessToRootNodeTypeMapper(), new SystemicProcessToRootNodeTypeMapper());
         sut.InitializeWith(_commandRegister);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.CompoundMetabolizingEnzymes.Id)).Returns(_partialProcessesNode);
      }
   }

   public class When_the_compound_metabolism_presenter_is_editing_a_compound : concern_for_CompoundProcessesPresenter
   {
      private ITreeNode _node1;
      private ITreeNode _node2;
      private SystemicProcess _systemicProcess;
      private ITreeNode _hepaticClearanceNode;

      protected override void Context()
      {
         base.Context();
         var proc1 = new EnzymaticProcess().WithName("proc1");
         var proc2 = new EnzymaticProcess().WithName("proc2");
         _node1 = A.Fake<ITreeNode>();
         A.CallTo(() => _node1.Id).Returns("1");
         _node2 = A.Fake<ITreeNode>();
         A.CallTo(() => _node2.Id).Returns("2");
         _compound.Add(proc1);
         _compound.Add(proc2);
         _systemicProcess = new SystemicProcess { SystemicProcessType = SystemicProcessTypes.Hepatic };
         _compound.AddProcess(_systemicProcess);
         A.CallTo(() => _partialProcessNodeMapper.MapFrom(proc1)).Returns(_node1);
         A.CallTo(() => _partialProcessNodeMapper.MapFrom(proc2)).Returns(_node2);
         //nodes not added yet to the tree
         A.CallTo(() => _view.TreeView.NodeById(_node1.Id)).Returns(null);
         A.CallTo(() => _view.TreeView.NodeById(_node2.Id)).Returns(null);

         _hepaticClearanceNode = _treeNodeFactory.CreateFor(SystemicProcessNodeType.HepaticClearance);
         A.CallTo(() => _view.TreeView.NodeById(_hepaticClearanceNode.Id)).Returns(_hepaticClearanceNode);
      }

      protected override void Because()
      {
         sut.EditCompound(_compound);
      }

      [Observation]
      public void should_not_add_nodes_to_view_whose_parent_nodes_are_not_already_viewed()
      {
         A.CallTo(() => _view.AddNode(_node1)).MustNotHaveHappened();
      }

      [Observation]
      public void should_add_nodes_to_view_whose_parent_nodes_are_already_viewed()
      {
         A.CallTo(() => _view.AddNode(A<ITreeNode>._)).MustHaveHappened();
      }

      [Observation]
      public void should_add_node_for_absorption_process()
      {
         A.CallTo(() => _view.AddNode(new RootNode(PKSimRootNodeTypes.Absorption))).MustHaveHappened();
      }

      [Observation]
      public void should_add_node_for_inhibition_process()
      {
         A.CallTo(() => _view.AddNode(new RootNode(PKSimRootNodeTypes.InhibitionProcess))).MustHaveHappened();
      }

      [Observation]
      public void should_add_node_for_induction_process()
      {
         A.CallTo(() => _view.AddNode(new RootNode(PKSimRootNodeTypes.InductionProcess))).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_node_for_the_metabolism_processes()
      {
         A.CallTo(() => _view.AddNode(new RootNode(PKSimRootNodeTypes.MetabolicProcesses))).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_node_for_the_transport_and_excretion_processes()
      {
         A.CallTo(() => _view.AddNode(new RootNode(PKSimRootNodeTypes.TransportAndExcretionProcesses))).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_node_for_the_distribution()
      {
         A.CallTo(() => _view.AddNode(new RootNode(PKSimRootNodeTypes.Distribution))).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_node_for_partial_process_defined_in_the_compound()
      {
         _partialProcessesNode.Children.ShouldOnlyContain(_node1, _node2);
      }

      [Observation]
      public void should_add_one_node_for_the_partial_processes()
      {
         A.CallTo(() => _view.AddNode(_partialProcessesNode)).MustHaveHappened();
      }
   }

   public class when_the_compound_presenter_is_told_to_activate_a_node_representing_normal_process : concern_for_CompoundProcessesPresenter
   {
      private ITreeNode<CompoundProcess> _processNode;
      private CompoundProcess _process;

      protected override void Context()
      {
         base.Context();
         _process = new SpecificBindingPartialProcess();
         _processNode = new ObjectWithIdAndNameNode<CompoundProcess>(_process);
      }

      protected override void Because()
      {
         sut.ActivateNode(_processNode);
      }

      [Observation]
      public void should_display_the_parameters_define_in_the_process()
      {
         A.CallTo(() => _compoundProcessPresenter.Edit(_process)).MustHaveHappened();
      }
   }

   public class When_the_compound_metabolism_presenter_is_told_to_active_a_node_representing_an_enzymatic_process : concern_for_CompoundProcessesPresenter
   {
      private ITreeNode<EnzymaticProcess> _processNode;
      private EnzymaticProcess _process;

      protected override void Context()
      {
         base.Context();
         _process = new EnzymaticProcess();
         _processNode = new ObjectWithIdAndNameNode<EnzymaticProcess>(_process);
      }

      protected override void Because()
      {
         sut.ActivateNode(_processNode);
      }

      [Observation]
      public void should_display_the_parameters_define_in_the_process()
      {
         A.CallTo(() => _compoundEnzymaticProcessPresenter.Edit(_process)).MustHaveHappened();
      }
   }

   public class when_asked_to_activate_node_that_represents_intestinal_permeability_parameter : concern_for_CompoundProcessesPresenter
   {
      private ITreeNode _treeNode;
      private CompoundParameterNodeType _compoundParameterNodeType;

      protected override void Context()
      {
         base.Context();
         _treeNode = A.Fake<ITreeNode>();
         _compoundParameterNodeType = CompoundParameterNodeType.SpecificIntestinalPermeability;

         A.CallTo(() => _treeNode.TagAsObject).Returns(_compoundParameterNodeType);

         _presenter = A.Fake<ICompoundParameterGroupPresenter>();
         A.CallTo(() => _compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper.MapFrom(_compoundParameterNodeType)).Returns(_presenter);

         sut.EditCompound(_compound);
      }

      protected override void Because()
      {
         sut.ActivateNode(_treeNode);
      }

      [Observation]
      public void should_activate_view_for_intestinal_permeability()
      {
         A.CallTo(() => _view.ActivateView(_presenter.BaseView)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_compound()
      {
         A.CallTo(() => _presenter.EditCompound(_compound)).MustHaveHappened();
      }
   }

   public class when_resolving_presenters_for_a_node_type : concern_for_CompoundProcessesPresenter
   {
      private ITreeNode _treeNode;
      private CompoundParameterNodeType _compoundParameterNodeType;

      protected override void Context()
      {
         base.Context();
         _treeNode = A.Fake<ITreeNode>();
         _compoundParameterNodeType = CompoundParameterNodeType.SpecificIntestinalPermeability;

         A.CallTo(() => _treeNode.TagAsObject).Returns(_compoundParameterNodeType);
         sut.EditCompound(_compound);
      }

      protected override void Because()
      {
         sut.ActivateNode(_treeNode);
         sut.ActivateNode(_treeNode);
      }

      [Observation]
      public void should_cache_and_use_the_first_instance_of_presenter_resolved_for_node_type()
      {
         A.CallTo(() => _compoundParameterNodeTypeToCompoundParameterGroupPresenterMapper.MapFrom(_compoundParameterNodeType)).MustHaveHappenedOnceExactly();
      }
   }

   public class When_the_compound_metabolism_presenter_is_told_to_active_a_node_that_does_not_represent_an_enzymaic_process : concern_for_CompoundProcessesPresenter
   {
      protected override void Because()
      {
         sut.ActivateNode(A.Fake<ITreeNode>());
      }

      [Observation]
      public void should_clear_the_list_of_displayed_parameter()
      {
         A.CallTo(() => _view.ActivateView(_noItemInSelectionPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_the_compound_process_presenter_is_told_to_rename_a_process : concern_for_CompoundProcessesPresenter
   {
      private SystemicProcess _process;
      private IPKSimCommand _renameCommand;

      protected override void Context()
      {
         base.Context();
         _process = A.Fake<SystemicProcess>();
         _renameCommand = A.Fake<IPKSimCommand>();
         A.CallTo(() => _compoundProcessPresentationTask.RenameDataSource(_process)).Returns(_renameCommand);
      }

      protected override void Because()
      {
         sut.RenameDataSourceInProcess(_process);
      }

      [Observation]
      public void should_rename_the_process_and_add_the_command_to_the_history()
      {
         A.CallTo(() => _commandRegister.AddCommand(_renameCommand)).MustHaveHappened();
      }
   }

   public abstract class When_adding_processes_to_the_compound_for_a_molecule_that_already_exists<TFirstProcess, TSecondProcess> : concern_for_CompoundProcessesPresenter
      where TFirstProcess : PartialProcess, new()
      where TSecondProcess : PartialProcess, new()
   {
      private PartialProcess _newProcess;
      private ITreeNode _moleculeNode1;
      private ITreeNode _compoundProcessNode1;
      private ITreeNode _compoundProcessNode2;

      protected override void Context()
      {
         base.Context();
         var existingProcess = new TFirstProcess { MoleculeName = "CYP", DataSource = "Lab", Id = "1" };
         _newProcess = new TFirstProcess { MoleculeName = "CYP", DataSource = "Lab2", Id = "2" };
         _compoundProcessNode1 = new CompoundProcessNode(existingProcess);
         _compoundProcessNode2 = new CompoundProcessNode(_newProcess);

         _moleculeNode1 = new PartialProcessMoleculeNode("CYP", new TFirstProcess());
         _moleculeNode1.AddChild(_compoundProcessNode1);

         var moleculeNode2 = new PartialProcessMoleculeNode("CYP", new TSecondProcess());
         moleculeNode2.AddChild(_compoundProcessNode2);

         A.CallTo(() => _partialProcessNodeMapper.MapFrom(existingProcess)).Returns(_moleculeNode1);
         A.CallTo(() => _partialProcessNodeMapper.MapFrom(_newProcess)).Returns(moleculeNode2);

         A.CallTo(() => _view.TreeView.NodeById(_moleculeNode1.Id)).Returns(_moleculeNode1);

         sut.Handle(new AddCompoundProcessEvent { Entity = existingProcess });
      }

      protected override void Because()
      {
         sut.Handle(new AddCompoundProcessEvent { Entity = _newProcess });
      }

      [Observation]
      public void should_add_the_created_process_to_the_existing_molecule_instead_of_creating_a_new_one()
      {
         _moleculeNode1.Children.ShouldContain(_compoundProcessNode1);
         _moleculeNode1.Children.ShouldContain(_compoundProcessNode2);
      }

      [Observation]
      public void must_add_node_to_correct_parent()
      {
         A.CallTo(() => _view.TreeView.NodeById(GetRootNodeId())).MustHaveHappenedTwiceExactly();
      }

      protected abstract string GetRootNodeId();
   }

   public class When_adding_an_inhibition_process_to_the_compound_for_a_molecule_that_already_exists :
      When_adding_processes_to_the_compound_for_a_molecule_that_already_exists<InhibitionProcess, InhibitionProcess>
   {
      protected override string GetRootNodeId()
      {
         return PKSimRootNodeTypes.InhibitionProcess.Id;
      }
   }

   public class When_adding_an_specific_binding_process_to_the_compound_for_a_molecule_that_already_exists :
      When_adding_processes_to_the_compound_for_a_molecule_that_already_exists<SpecificBindingPartialProcess, SpecificBindingPartialProcess>
   {
      protected override string GetRootNodeId()
      {
         return PKSimRootNodeTypes.CompoundProteinBindingPartners.Id;
      }
   }

   public class When_adding_an_transport_process_to_the_compound_for_a_molecule_that_already_exists :
      When_adding_processes_to_the_compound_for_a_molecule_that_already_exists<TransportPartialProcess, TransportPartialProcessWithSpecies>
   {
      protected override string GetRootNodeId()
      {
         return PKSimRootNodeTypes.CompoundTransportProteins.Id;
      }
   }

   public class When_adding_an_enzymatic_process_to_the_compound_for_a_molecule_that_already_exists :
      When_adding_processes_to_the_compound_for_a_molecule_that_already_exists<EnzymaticProcess, EnzymaticProcessWithSpecies>
   {
      protected override string GetRootNodeId()
      {
         return PKSimRootNodeTypes.CompoundMetabolizingEnzymes.Id;
      }
   }

   public class EnzymaticProcessEqualityComparer : GenericEqualityComparer<EnzymaticProcess>
   {

   }
}