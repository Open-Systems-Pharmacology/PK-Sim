using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Individuals;

using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Nodes;
using OSPSuite.Presentation.Views;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualMoleculesPresenter : ContextSpecification<IIndividualMoleculesPresenter>
   {
      protected IMoleculesView _view;
      protected IEditMoleculeTask<Individual> _editMoleculeTask;
      protected ITreeNodeFactory _treeNodeFactory;
      protected ITreeNodeContextMenuFactory _contextMenyFactory;
      protected IDialogCreator _dialogCreator;
      protected IEntityTask _entityTask;
      protected List<IndividualEnzyme> _enzymeList;
      protected List<IndividualMolecule> _moleculeList;
      protected List<IndividualOtherProtein> _otherProteinList;
      protected List<IndividualTransporter> _transporterList;
      protected Individual _individual;
      protected ICommandCollector _commandRegister;
      private IRootNodeToIndividualExpressionsPresenterMapper<Individual> _expressionsPresenterMapper;
      protected ITreeNode<RootNodeType> _transporterFolderNode;
      protected ITreeNode<RootNodeType> _enzymeFolderNode;
      protected ITreeNode<RootNodeType> _otherProteinsFolderNode;
      private INoItemInSelectionPresenter _noitemInSelectionPresenter;

      protected override void Context()
      {
         _view = A.Fake<IMoleculesView>();
         A.CallTo(() => _view.TreeView).Returns(A.Fake<IUxTreeView>());
         _editMoleculeTask = A.Fake<IEditMoleculeTask<Individual>>();
         _treeNodeFactory = A.Fake<ITreeNodeFactory>();
         _contextMenyFactory = A.Fake<ITreeNodeContextMenuFactory>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _entityTask = A.Fake<IEntityTask>();
         _commandRegister = A.Fake<ICommandCollector>();
         _expressionsPresenterMapper = A.Fake<IRootNodeToIndividualExpressionsPresenterMapper<Individual>>();
         _noitemInSelectionPresenter = A.Fake<INoItemInSelectionPresenter>();
         _enzymeList = new List<IndividualEnzyme>();
         _moleculeList = new List<IndividualMolecule>();
         _otherProteinList = new List<IndividualOtherProtein>();
         _transporterList = new List<IndividualTransporter>();
         _individual = A.Fake<Individual>();
         _enzymeFolderNode = new RootNode(PKSimRootNodeTypes.IndividualMetabolizingEnzymes);
         _otherProteinsFolderNode = new RootNode(PKSimRootNodeTypes.IndividualProteinBindingPartners);
         _transporterFolderNode = new RootNode(PKSimRootNodeTypes.IndividualTransportProteins);
         A.CallTo(() => _individual.AllMolecules<IndividualEnzyme>()).Returns(_enzymeList);
         A.CallTo(() => _individual.AllMolecules<IndividualOtherProtein>()).Returns(_otherProteinList);
         A.CallTo(() => _individual.AllMolecules<IndividualTransporter>()).Returns(_transporterList);
         A.CallTo(() => _individual.AllMolecules()).Returns(_moleculeList);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualMetabolizingEnzymes)).Returns(_enzymeFolderNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualProteinBindingPartners)).Returns(_otherProteinsFolderNode);
         A.CallTo(() => _treeNodeFactory.CreateFor(PKSimRootNodeTypes.IndividualTransportProteins)).Returns(_transporterFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.IndividualMetabolizingEnzymes.Id)).Returns(_enzymeFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.IndividualProteinBindingPartners.Id)).Returns(_otherProteinsFolderNode);
         A.CallTo(() => _view.TreeView.NodeById(PKSimRootNodeTypes.IndividualTransportProteins.Id)).Returns(_transporterFolderNode);
         sut = new IndividualMoleculesPresenter(_view, _editMoleculeTask, _treeNodeFactory, _contextMenyFactory, _dialogCreator, _entityTask, _expressionsPresenterMapper, _noitemInSelectionPresenter);
         sut.InitializeWith(_commandRegister);
      }
   }

   public class When_the_individual_expressions_presenter_is_editing_an_individual : concern_for_IndividualMoleculesPresenter
   {
      private IndividualProtein _protein;
      private IndividualEnzyme _enzyme;
      private ITreeNode<IndividualEnzyme> _enzymeNode;

      protected override void Context()
      {
         base.Context();
         _protein = A.Fake<IndividualProtein>();
         _enzyme = A.Fake<IndividualEnzyme>();
         _moleculeList.AddRange(new[] {_protein, _enzyme});
         _enzymeList.Add(_enzyme);
         _enzymeNode = A.Fake<ITreeNode<IndividualEnzyme>>();
         A.CallTo(() => _treeNodeFactory.CreateFor(_enzyme)).Returns(_enzymeNode);
      }

      protected override void Because()
      {
         sut.EditIndividual(_individual);
      }

      [Observation]
      public void should_add_one_node_for_each_protein_type_available_in_pksim()
      {
         A.CallTo(() => _view.AddNode(_transporterFolderNode)).MustHaveHappened();
         A.CallTo(() => _view.AddNode(_enzymeFolderNode)).MustHaveHappened();
         A.CallTo(() => _view.AddNode(_otherProteinsFolderNode)).MustHaveHappened();
      }

      [Observation]
      public void should_add_one_node_for_each_enzyme_defined_in_the_individual()
      {
         A.CallTo(() => _view.AddNode(_enzymeNode)).MustHaveHappened();
      }
   }

   public class When_adding_an_enzyme_expression_to_an_individual : concern_for_IndividualMoleculesPresenter
   {
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<IPKSimCommand>();
         A.CallTo(() => _editMoleculeTask.AddMoleculeTo<IndividualEnzyme>(_individual)).Returns(_command);
         sut.EditIndividual(_individual);
      }

      protected override void Because()
      {
         sut.AddMolecule<IndividualEnzyme>();
      }

      [Observation]
      public void should_leverage_the_protein_expression_task_to_add_a_protein_for_the_individual()
      {
         A.CallTo(() => _editMoleculeTask.AddMoleculeTo<IndividualEnzyme>(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_resulting_command_to_the_individual_presenter()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustHaveHappened();
      }
   }

   public class When_adding_a_molecule_to_an_individual : concern_for_IndividualMoleculesPresenter
   {
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<IPKSimCommand>();
         A.CallTo(() => _editMoleculeTask.AddMoleculeTo<IndividualEnzyme>(_individual)).Returns(_command);
         sut.EditIndividual(_individual);
      }

      protected override void Because()
      {
         sut.AddMolecule<IndividualEnzyme>();
      }

      [Observation]
      public void should_leverage_the_protein_expression_task_to_add_a_default_protein_for_the_individual()
      {
         A.CallTo(() => _editMoleculeTask.AddMoleculeTo<IndividualEnzyme>(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_resulting_command_to_the_individual_presenter()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustHaveHappened();
      }
   }

   public class When_removing_an_expression_from_an_individual : concern_for_IndividualMoleculesPresenter
   {
      private IPKSimCommand _command;
      private IndividualProtein _proteinToRemove;
      private string _proteinType;

      protected override void Context()
      {
         base.Context();
         _command = A.Fake<IPKSimCommand>();
         _proteinToRemove = A.Fake<IndividualEnzyme>().WithName("Trlala");
         _proteinType = "toto";
         A.CallTo(() => _editMoleculeTask.RemoveMoleculeFrom(_proteinToRemove, _individual)).Returns(_command);
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>.Ignored, ViewResult.Yes)).Returns(ViewResult.Yes);
         A.CallTo(() => _entityTask.TypeFor(_proteinToRemove)).Returns(_proteinType);
         sut.EditIndividual(_individual);
      }

      protected override void Because()
      {
         sut.RemoveMolecule(_proteinToRemove);
      }

      [Observation]
      public void should_ask_the_user_if_he_really_wants_to_delete_the_protein()
      {
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteProtein(_proteinType, _proteinToRemove.Name), ViewResult.Yes)).MustHaveHappened();
      }

      [Observation]
      public void should_leverage_the_protein_expression_task_to_remove_the_protein_from_the_individual()
      {
         A.CallTo(() => _editMoleculeTask.RemoveMoleculeFrom(_proteinToRemove, _individual)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_resulting_command_to_the_individual_presenter()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustHaveHappened();
      }
   }

   public class When_removing_an_expression_from_an_individual_and_the_user_cancels_the_action : concern_for_IndividualMoleculesPresenter
   {
      private IndividualProtein _proteinToRemove;

      protected override void Context()
      {
         base.Context();
         _proteinToRemove = A.Fake<IndividualEnzyme>().WithName("Trlala");
         sut.EditIndividual(_individual);
         A.CallTo(() => _dialogCreator.MessageBoxYesNo(A<string>.Ignored, ViewResult.Yes)).Returns(ViewResult.No);
      }

      protected override void Because()
      {
         sut.RemoveMolecule(_proteinToRemove);
      }

      [Observation]
      public void should_not_delete_the_protein()
      {
         A.CallTo(() => _editMoleculeTask.RemoveMoleculeFrom(_proteinToRemove, _individual)).MustNotHaveHappened();
      }
   }
}