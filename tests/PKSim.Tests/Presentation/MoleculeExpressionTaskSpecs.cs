using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ProteinExpression;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_MoleculeExpressionTask : ContextSpecification<IMoleculeExpressionTask<Individual>>
   {
      protected Individual _individual;
      protected IExecutionContext _executionContext;
      protected IMoleculeToQueryExpressionSettingsMapper _querySettingsMapper;
      protected IApplicationController _applicationController;
      private IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      protected IContainerTask _containerTask;
      protected IProteinExpressionsDatabasePathManager _proteinExpressionDbPathManager;
      protected IProteinExpressionsPresenter _proteinExpressionPresenter;
      protected ISimpleMoleculePresenter _simpleMoleculePresenter;
      protected IndividualMolecule _molecule;
      protected MoleculeExpressionContainer _moleculeContainer1;
      protected MoleculeExpressionContainer _moleculeContainer2;
      protected IOntogenyRepository _ontogenyRepository;
      protected Ontogeny _ontogeny;
      private ITransportContainerUpdater _transportContainerUpdater;
      protected IMoleculeParameterRepository _moleculeParameterRepository;
      private ISimulationSubjectExpressionTask<Individual> _subjectExpressionTask;
      protected IOntogenyTask<Individual> _ontogenyTask;

      protected override void Context()
      {
         _individual = new Individual();
         _individual.OriginData = new OriginData();
         _individual.OriginData.Species = new Species().WithName("Human");
         _ontogeny = new DatabaseOntogeny {Name = "toto"};
         _molecule = new IndividualEnzyme {Name = "CYP3A4"};
         _executionContext = A.Fake<IExecutionContext>();
         _proteinExpressionPresenter = A.Fake<IProteinExpressionsPresenter>();
         _simpleMoleculePresenter = A.Fake<ISimpleMoleculePresenter>();
         _querySettingsMapper = A.Fake<IMoleculeToQueryExpressionSettingsMapper>();
         _applicationController = A.Fake<IApplicationController>();
         _individualMoleculeFactoryResolver = A.Fake<IIndividualMoleculeFactoryResolver>();
         _transportContainerUpdater = A.Fake<ITransportContainerUpdater>();
         _containerTask = A.Fake<IContainerTask>();
         _moleculeParameterRepository = A.Fake<IMoleculeParameterRepository>();
         _proteinExpressionDbPathManager = A.Fake<IProteinExpressionsDatabasePathManager>();
         A.CallTo(() => _applicationController.Start<IProteinExpressionsPresenter>()).Returns(_proteinExpressionPresenter);
         A.CallTo(() => _applicationController.Start<ISimpleMoleculePresenter>()).Returns(_simpleMoleculePresenter);
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         var proteinFactory = A.Fake<IIndividualMoleculeFactory>();
         _moleculeContainer1 = new MoleculeExpressionContainer().WithName("C1");
         _moleculeContainer1.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameter.REL_EXP));
         _moleculeContainer1.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.REL_EXP_NORM));
         _moleculeContainer2 = new MoleculeExpressionContainer().WithName("C2");
         _moleculeContainer2.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameter.REL_EXP));
         _moleculeContainer2.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.REL_EXP_NORM));
         A.CallTo(() => _individualMoleculeFactoryResolver.FactoryFor<IndividualProtein>()).Returns(proteinFactory);
         A.CallTo(() => proteinFactory.CreateFor(_individual)).Returns(_molecule);
         _molecule.Add(_moleculeContainer1);
         _molecule.Add(_moleculeContainer2);
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameter.REFERENCE_CONCENTRATION));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(20).WithName(CoreConstants.Parameter.HALF_LIFE_LIVER));
         _molecule.Add(DomainHelperForSpecs.ConstantParameterWithValue(30).WithName(CoreConstants.Parameter.HALF_LIFE_INTESTINE));

         A.CallTo(() => _ontogenyRepository.AllFor(_individual.Species.Name)).Returns(new[] {_ontogeny, new DatabaseOntogeny {Name = "tralala"},});
         A.CallTo(() => _executionContext.Resolve<IOntogenyRepository>()).Returns(_ontogenyRepository);

         _subjectExpressionTask = new IndividualExpressionTask(_executionContext);

         _ontogenyTask = A.Fake<IOntogenyTask<Individual>>();
         sut = new MoleculeExpressionTask<Individual>(_applicationController, _executionContext,
            _individualMoleculeFactoryResolver, _querySettingsMapper,
            _containerTask, _proteinExpressionDbPathManager,
            _ontogenyRepository, _transportContainerUpdater, _moleculeParameterRepository,_subjectExpressionTask,_ontogenyTask);
      }
   }

   public class When_asked_to_add_a_protein_to_an_individual_for_which_an_expression_database_has_been_defined : concern_for_MoleculeExpressionTask
   {
      private ICommand _resultCommand;
      private QueryExpressionResults _queryResults;
      private QueryExpressionSettings _querySettings;
      private const string _queryConfiguration = "blalal";
      private const string _moleculeName = "toto";
      private const string _renamedName = "aaa";


      protected override void Context()
      {
         base.Context();
         _querySettings = A.Fake<QueryExpressionSettings>();
         _queryResults = new QueryExpressionResults(new List<ExpressionResult>());
         _queryResults.ProteinName = _moleculeName;
         A.CallTo(() => _querySettingsMapper.MapFrom(_molecule)).Returns(_querySettings);
         A.CallTo(() => _proteinExpressionDbPathManager.HasDatabaseFor(_individual.Species)).Returns(true);
         A.CallTo(() => _proteinExpressionPresenter.Start()).Returns(true);
         A.CallTo(() => _proteinExpressionPresenter.GetQueryResults()).Returns(_queryResults);
         _queryResults.QueryConfiguration = _queryConfiguration;
         A.CallTo(() => _containerTask.CreateUniqueName(_individual, _queryResults.ProteinName, true)).Returns(_renamedName);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_molecule)).Returns(_individual);
      }

      protected override void Because()
      {
         _resultCommand = sut.AddMoleculeTo<IndividualProtein>(_individual);
      }

      [Observation]
      public void should_initialize_the_query_expression_presenter_with_the_default_query_settings_for_the_protein()
      {
         A.CallTo(() => _proteinExpressionPresenter.InitializeSettings(_querySettings)).MustHaveHappened();
      }

      [Observation]
      public void should_start_the_query_expression_presenter()
      {
         A.CallTo(() => _proteinExpressionPresenter.Start()).MustHaveHappened();
      }

      [Observation]
      public void should_save_the_query_string_into_the_protein()
      {
         _molecule.QueryConfiguration.ShouldBeEqualTo(_queryConfiguration);
      }

      [Observation]
      public void should_have_created_a_unique_name_in_the_individual_for_the_protein_based_on_the_seleted_protein()
      {
         _molecule.Name.ShouldBeEqualTo(_renamedName);
      }

      [Observation]
      public void should_add_the_protein_to_the_individual()
      {
         _individual.AllMolecules().ShouldContain(_molecule);
      }

      [Observation]
      public void should_set_the_default_ontogeny_matching_the_name_of_the_molecule_if_exists()
      {
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_molecule, _ontogeny, _individual)).MustHaveHappened();
      }

      [Observation]
      public void the_resulting_command_should_be_an_instance_of_add_protein_from_query_to_individual_command()
      {
         _resultCommand.ShouldBeAnInstanceOf<AddMoleculeExpressionsFromQueryToIndividualCommand>();
      }
   }

   public class When_asked_to_add_a_default_protein_to_an_individual : concern_for_MoleculeExpressionTask
   {
      private ICommand _resultCommand;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _simpleMoleculePresenter.CreateMoleculeFor<IndividualProtein>(_individual)).Returns(true);
         A.CallTo(() => _simpleMoleculePresenter.MoleculeName).Returns("MOLECULE");
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor("MOLECULE", CoreConstants.Parameter.REFERENCE_CONCENTRATION, A<double?>._, null)).Returns(10);
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor("MOLECULE", CoreConstants.Parameter.HALF_LIFE_LIVER, A<double?>._, null)).Returns(20);
         A.CallTo(() => _moleculeParameterRepository.ParameterValueFor("MOLECULE", CoreConstants.Parameter.HALF_LIFE_INTESTINE, A<double?>._, null)).Returns(40);
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
      public void the_resulting_command_should_be_an_instance_of_add_protein_to_individual_command()
      {
         _resultCommand.ShouldBeAnInstanceOf<AddMoleculeToIndividualCommand>();
      }

      [Observation]
      public void should_add_the_protein_to_the_individual()
      {
         _individual.AllMolecules().ShouldContain(_molecule);
      }

      [Observation]
      public void should_update_the_default_value_for_the_reference_concentration_parameter()
      {
         _molecule.ReferenceConcentration.Value.ShouldBeEqualTo(10);
      }

      [Observation]
      public void should_update_the_default_value_for_the_half_life_parameters()
      {
         _molecule.HalfLifeLiver.Value.ShouldBeEqualTo(20);
         _molecule.HalfLifeIntestine.Value.ShouldBeEqualTo(40);
      }
   }

   public class When_asked_to_add_a_protein_to_an_individual_for_which_no_database_has_been_defined : concern_for_MoleculeExpressionTask
   {
      private ICommand _resultCommand;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _proteinExpressionDbPathManager.HasDatabaseFor(_individual.Species)).Returns(false);
         A.CallTo(() => _simpleMoleculePresenter.CreateMoleculeFor<IndividualProtein>(_individual)).Returns(true);
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
      public void the_resulting_command_should_be_an_instance_of_add_protein_to_individual_command()
      {
         _resultCommand.ShouldBeAnInstanceOf<AddMoleculeToIndividualCommand>();
      }

      [Observation]
      public void should_add_the_protein_to_the_individual()
      {
         _individual.AllMolecules().ShouldContain(_molecule);
      }
   }

   public class When_asked_to_set_a_relative_expression_value : concern_for_MoleculeExpressionTask
   {
      private ICommand _result;
      private double _relativeVMaxValue;
      private string _containerName;

      protected override void Context()
      {
         base.Context();
         _relativeVMaxValue = 25;
         _containerName = "C1";
         var relExp = new PKSimParameter();
         relExp.Formula = new ConstantFormula();
         A.CallTo(() => _executionContext.BuildingBlockContaining(relExp)).Returns(_individual);
      }

      protected override void Because()
      {
         _result = sut.SetRelativeExpressionFor(_molecule, _containerName, _relativeVMaxValue);
      }

      [Observation]
      public void should_return_the_underlying_command_used_to_set_the_relative_expression_value()
      {
         _result.ShouldBeAnInstanceOf<SetRelativeExpressionAndNormalizeCommand>();
      }
   }

   public class When_asked_to_remove_a_protein_from_an_individual : concern_for_MoleculeExpressionTask
   {
      private IndividualMolecule _proteinToRemove;
      private ICommand _resultCommand;

      protected override void Context()
      {
         base.Context();
         _proteinToRemove = A.Fake<IndividualMolecule>();
         _individual.AddMolecule(_proteinToRemove);
      }

      protected override void Because()
      {
         _resultCommand = sut.RemoveMoleculeFrom(_proteinToRemove, _individual);
      }

      [Observation]
      public void should_remove_the_protein_from_the_individual()
      {
         _individual.AllMolecules().Contains(_proteinToRemove).ShouldBeFalse();
      }

      [Observation]
      public void the_resulting_command_should_be_an_instance_of_remove_protein_from_individual_command()
      {
         _resultCommand.ShouldBeAnInstanceOf<RemoveMoleculeFromIndividualCommand>();
      }
   }

   public class When_asked_to_edit_a_protein : concern_for_MoleculeExpressionTask
   {
      private QueryExpressionSettings _querySettings;
      private ICommand _resultCommand;
      private QueryExpressionResults _queryResults;
      private IndividualMolecule _clonedProtein;

      protected override void Context()
      {
         base.Context();
         _clonedProtein = A.Fake<IndividualMolecule>();
         A.CallTo(() => _clonedProtein.AllExpressionsContainers()).Returns(_molecule.AllExpressionsContainers());
         _individual.AddMolecule(_molecule);
         _querySettings = A.Fake<QueryExpressionSettings>();
         _queryResults = A.Fake<QueryExpressionResults>();
         A.CallTo(() => _executionContext.Clone(_molecule)).Returns(_clonedProtein);
         A.CallTo(() => _querySettingsMapper.MapFrom(_molecule)).Returns(_querySettings);
         A.CallTo(() => _proteinExpressionPresenter.Start()).Returns(true);
         A.CallTo(() => _proteinExpressionPresenter.GetQueryResults()).Returns(_queryResults);
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
      public void the_resulting_command_should_be_an_instance_of_edit_individual_protein_from_query()
      {
         _resultCommand.ShouldBeAnInstanceOf<EditIndividualMoleculeExpressionInIndividualFromQueryCommand>();
      }
   }
}