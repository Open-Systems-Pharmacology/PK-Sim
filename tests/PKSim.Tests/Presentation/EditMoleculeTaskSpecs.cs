using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditMoleculeTask : ContextSpecification<IEditMoleculeTask<Individual>>
   {
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      protected IMoleculeToQueryExpressionSettingsMapper _querySettingsMapper;
      protected IExecutionContext _executionContext;
      protected IGeneExpressionsDatabasePathManager _geneExpressionsDatabasePathManager;
      protected IApplicationController _applicationController;
      protected IIndividualMoleculeFactoryResolver _moleculeFactoryResolver;
      protected IProteinExpressionsPresenter _proteinExpressionPresenter;
      protected ISimpleMoleculePresenter _simpleMoleculePresenter;
      protected MoleculeExpressionContainer _moleculeContainer1;
      protected MoleculeExpressionContainer _moleculeContainer2;
      protected IndividualMolecule _molecule;
      protected Individual _individual;

      protected override void Context()
      {
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         _querySettingsMapper = A.Fake<IMoleculeToQueryExpressionSettingsMapper>();
         _executionContext = A.Fake<IExecutionContext>();
         _geneExpressionsDatabasePathManager = A.Fake<IGeneExpressionsDatabasePathManager>();
         _applicationController = A.Fake<IApplicationController>();
         _moleculeFactoryResolver = A.Fake<IIndividualMoleculeFactoryResolver>();

         sut = new EditMoleculeTask<Individual>(_moleculeExpressionTask, _querySettingsMapper, _executionContext,
            _geneExpressionsDatabasePathManager, _applicationController, _moleculeFactoryResolver);

         _proteinExpressionPresenter = A.Fake<IProteinExpressionsPresenter>();
         _simpleMoleculePresenter = A.Fake<ISimpleMoleculePresenter>();

         A.CallTo(() => _applicationController.Start<IProteinExpressionsPresenter>()).Returns(_proteinExpressionPresenter);
         A.CallTo(() => _applicationController.Start<ISimpleMoleculePresenter>()).Returns(_simpleMoleculePresenter);

         _moleculeContainer1 = new MoleculeExpressionContainer().WithName("C1");
         _moleculeContainer1.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameters.REL_EXP));
         _moleculeContainer2 = new MoleculeExpressionContainer().WithName("C2");
         _moleculeContainer2.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameters.REL_EXP));

         _individual = new Individual {OriginData = new OriginData {Species = new Species().WithName("Human")}};


         _molecule = new IndividualEnzyme {Name = "CYP3A4"};
         _molecule.Add(_moleculeContainer1);
         _molecule.Add(_moleculeContainer2);
      }
   }

   public class When_asked_to_add_a_protein_to_an_individual_for_which_an_expression_database_has_been_defined : concern_for_EditMoleculeTask
   {
      private ICommand _resultCommand;
      private QueryExpressionResults _queryResults;
      private QueryExpressionSettings _querySettings;
      private IIndividualMoleculeFactory _proteinFactory;
      private IndividualMolecule _tempProtein;
      private ICommand _addCommand;
      private const string _queryConfiguration = "blalal";
      private const string _moleculeName = "toto";
      private const string _renamedName = "aaa";

      protected override void Context()
      {
         base.Context();
         _addCommand = A.Fake<ICommand>();
         _querySettings = A.Fake<QueryExpressionSettings>();
         _queryResults = new QueryExpressionResults(new List<ExpressionResult>()) {ProteinName = _moleculeName};
         A.CallTo(() => _geneExpressionsDatabasePathManager.HasDatabaseFor(_individual.Species)).Returns(true);
         A.CallTo(() => _proteinExpressionPresenter.Start()).Returns(true);
         A.CallTo(() => _proteinExpressionPresenter.GetQueryResults()).Returns(_queryResults);
         _queryResults.QueryConfiguration = _queryConfiguration;
         A.CallTo(() => _executionContext.BuildingBlockContaining(_molecule)).Returns(_individual);
         _proteinFactory = A.Fake<IIndividualMoleculeFactory>();
         A.CallTo(() => _moleculeFactoryResolver.FactoryFor<IndividualProtein>()).Returns(_proteinFactory);
         _tempProtein = new IndividualEnzyme();
         A.CallTo(() => _querySettingsMapper.MapFrom(_tempProtein, _individual, _tempProtein.Name)).Returns(_querySettings);
         A.CallTo(() => _proteinFactory.AddMoleculeTo(_individual, "%TEMP%")).Returns(_tempProtein);
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo(_individual, _tempProtein, _queryResults)).Returns(_addCommand);
      }

      protected override void Because()
      {
         _resultCommand = sut.AddMoleculeTo<IndividualProtein>(_individual);
      }

      [Observation]
      public void should_initialize_the_query_expression_presenter_with_the_default_query_settings_for_the_temp_protein_created_for_the_edit()
      {
         A.CallTo(() => _proteinExpressionPresenter.InitializeSettings(_querySettings)).MustHaveHappened();
      }

      [Observation]
      public void should_start_the_query_expression_presenter()
      {
         A.CallTo(() => _proteinExpressionPresenter.Start()).MustHaveHappened();
      }

      [Observation]
      public void the_resulting_command_should_be_an_instance_of_add_protein_from_query_to_individual_command()
      {
         _resultCommand.ShouldBeEqualTo(_addCommand);
      }
   }

   public class When_asked_to_add_a_default_protein_to_an_individual : concern_for_EditMoleculeTask
   {
      private ICommand _resultCommand;
      private ICommand _addCommand;

      protected override void Context()
      {
         base.Context();
         _addCommand = A.Fake<ICommand>();
         A.CallTo(() => _simpleMoleculePresenter.CreateMoleculeFor<IndividualProtein>(_individual)).Returns(true);
         A.CallTo(() => _simpleMoleculePresenter.MoleculeName).Returns("MOLECULE");
         _molecule.Name = _simpleMoleculePresenter.MoleculeName;
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualProtein>(_individual, "MOLECULE")).Returns(_addCommand);
      }

      protected override void Because()
      {
         _resultCommand = sut.AddDefaultMolecule<IndividualProtein>(_individual);
      }

      [Observation]
      public void should_generate_a_default_protein_bypassing_the_expression_database()
      {
         A.CallTo(() => _simpleMoleculePresenter.CreateMoleculeFor<IndividualProtein>(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_default_parameters_in_the_newly_added_molecule()
      {
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualProtein>(_individual, "MOLECULE")).MustHaveHappened();
      }

      [Observation]
      public void the_resulting_command_should_be_an_instance_of_add_protein_to_individual_command()
      {
         _resultCommand.ShouldBeEqualTo(_addCommand);
      }
   }

   public class When_asked_to_add_a_protein_to_an_individual_for_which_no_database_has_been_defined : concern_for_EditMoleculeTask
   {
      private ICommand _resultCommand;
      private ICommand _addCommand;

      protected override void Context()
      {
         base.Context();
         _addCommand = A.Fake<ICommand>();
         A.CallTo(() => _geneExpressionsDatabasePathManager.HasDatabaseFor(_individual.Species)).Returns(false);
         A.CallTo(() => _simpleMoleculePresenter.CreateMoleculeFor<IndividualProtein>(_individual)).Returns(true);
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualProtein>(_individual, _simpleMoleculePresenter.MoleculeName))
            .Returns(_addCommand);
      }

      protected override void Because()
      {
         _resultCommand = sut.AddMoleculeTo<IndividualProtein>(_individual);
      }

      [Observation]
      public void should_start_the_simple_protein_presenter_to_retrieve_the_minimal_info_required_to_create_a_protein()
      {
         A.CallTo(() => _simpleMoleculePresenter.CreateMoleculeFor<IndividualProtein>(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_leverage_the_molecule_task_to_add_the_molecule_to_the_individual()
      {
         _resultCommand.ShouldBeEqualTo(_addCommand);
      }
   }

   public class When_asked_to_edit_a_protein : concern_for_EditMoleculeTask
   {
      private QueryExpressionSettings _querySettings;
      private ICommand _resultCommand;
      private QueryExpressionResults _queryResults;
      private IndividualMolecule _clonedProtein;
      private ICommand _editCommand;

      protected override void Context()
      {
         base.Context();
         _clonedProtein = A.Fake<IndividualMolecule>();
         _individual.AddMolecule(_molecule);
         _querySettings = A.Fake<QueryExpressionSettings>();
         _queryResults = A.Fake<QueryExpressionResults>();
         A.CallTo(() => _executionContext.Clone(_molecule)).Returns(_clonedProtein);
         A.CallTo(() => _querySettingsMapper.MapFrom(_molecule, _individual, _molecule.Name)).Returns(_querySettings);
         A.CallTo(() => _proteinExpressionPresenter.Start()).Returns(true);
         A.CallTo(() => _proteinExpressionPresenter.GetQueryResults()).Returns(_queryResults);
         _editCommand = A.Fake<ICommand>();
         A.CallTo(() => _moleculeExpressionTask.EditMolecule(_molecule, _queryResults, _individual)).Returns(_editCommand);
      }

      protected override void Because()
      {
         _resultCommand = sut.EditMolecule(_molecule, _individual);
      }

      [Observation]
      public void should_leverage_query_expression_presenter_to_edit_the_protein_with_the_previous_configuration()
      {
         A.CallTo(() => _proteinExpressionPresenter.InitializeSettings(_querySettings)).MustHaveHappened();
      }

      [Observation]
      public void should_create_a_clone_of_the_input_protein_and_perform_an_update_value_by_value_of_the_edited_protein()
      {
         A.CallTo(() => _proteinExpressionPresenter.InitializeSettings(_querySettings)).MustHaveHappened();
      }

      [Observation]
      public void should_leverage_the_molecule_expression_task_to_edit_the_molecule()
      {
         _resultCommand.ShouldBeEqualTo(_editCommand);
      }
   }
}