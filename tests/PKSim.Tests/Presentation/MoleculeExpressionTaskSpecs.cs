using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_MoleculeExpressionTask : ContextSpecification<IMoleculeExpressionTask<Individual>>
   {
      protected Individual _individual;
      protected IExecutionContext _executionContext;
      protected IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      protected IndividualMolecule _molecule;
      protected MoleculeExpressionContainer _moleculeContainer1;
      protected MoleculeExpressionContainer _moleculeContainer2;
      protected IOntogenyRepository _ontogenyRepository;
      protected Ontogeny _ontogeny;
      protected ISimulationSubjectExpressionTask<Individual> _subjectExpressionTask;
      protected IExpressionProfileUpdater _expressionProfileUpdater;
      protected IRegistrationTask _registrationTask;

      protected override void Context()
      {
         _individual = new Individual {OriginData = new OriginData {Species = new Species().WithName("Human")}};
         _ontogeny = new DatabaseOntogeny {Name = "toto"};
         _executionContext = A.Fake<IExecutionContext>();
         _individualMoleculeFactoryResolver = A.Fake<IIndividualMoleculeFactoryResolver>();
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         var proteinFactory = A.Fake<IIndividualMoleculeFactory>();
         _expressionProfileUpdater = A.Fake<IExpressionProfileUpdater>();
         _registrationTask= A.Fake<IRegistrationTask>(); 
         _moleculeContainer1 = new MoleculeExpressionContainer().WithName("C1");
         _moleculeContainer1.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameters.REL_EXP));
         _moleculeContainer2 = new MoleculeExpressionContainer().WithName("C2");
         _moleculeContainer2.Add(DomainHelperForSpecs.ConstantParameterWithValue(5).WithName(CoreConstants.Parameters.REL_EXP));
         A.CallTo(() => _individualMoleculeFactoryResolver.FactoryFor<IndividualProtein>()).Returns(proteinFactory);
         _molecule = new IndividualEnzyme {Name = "CYP3A4"};
         _molecule.Add(_moleculeContainer1);
         _molecule.Add(_moleculeContainer2);
         A.CallTo(() => proteinFactory.AddMoleculeTo(_individual, A<string>._)).Returns(_molecule);

         A.CallTo(() => _ontogenyRepository.AllFor(_individual.Species.Name)).Returns(new[] {_ontogeny, new DatabaseOntogeny {Name = "tralala"},});
         A.CallTo(() => _executionContext.Resolve<IOntogenyRepository>()).Returns(_ontogenyRepository);

         _subjectExpressionTask = new IndividualExpressionTask(_executionContext);

         sut = new MoleculeExpressionTask<Individual>(
            _executionContext,
            _individualMoleculeFactoryResolver,
            _subjectExpressionTask, 
            _expressionProfileUpdater,
            _registrationTask
            );
      }
   }

   public class When_adding_an_expression_profile_to_an_individual : concern_for_MoleculeExpressionTask
   {
      private ICommand _resultCommand;
      private ExpressionProfile _expressionProfile;
      private IOSPSuiteCommand _addCommand;
      private IIndividualMoleculeFactory _moleculeFactory;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _moleculeFactory = A.Fake<IIndividualMoleculeFactory>();
         _addCommand = new AddMoleculeToIndividualCommand(_molecule, _individual, _executionContext);
         A.CallTo(() => _individualMoleculeFactoryResolver.FactoryFor(_expressionProfile.Molecule)).Returns(_moleculeFactory);
      }

      protected override void Because()
      {
         _resultCommand = sut.AddExpressionProfile(_individual, _expressionProfile);
      }

      [Observation]
      public void should_create_a_new_molecule_base_on_the_expression_profile_type_and_add_it_to_the_individual()
      {
         A.CallTo(() => _moleculeFactory.AddMoleculeTo(_individual, _expressionProfile.MoleculeName)).MustHaveHappened();
      }

      [Observation]
      public void should_register_the_whole_individual_again_to_ensure_that_the_newly_added_parameters_and_containers_are_available()
      {
         A.CallTo(() => _registrationTask.Register(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_ensure_that_expression_profile_and_simulation_subject_are_synchronized()
      {
         A.CallTo(() => _expressionProfileUpdater.SynchroniseSimulationSubjectWithExpressionProfile(_individual, _expressionProfile)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_expression_profile_to_the_individual()
      {
         _individual.AllExpressionProfiles().ShouldContain(_expressionProfile);
      }

      [Observation]
      public void should_return_the_add_expression_profile_command()
      {
         _resultCommand.ShouldBeAnInstanceOf<AddMoleculeToIndividualCommand>();
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
         _resultCommand = sut.RemoveExpressionProfileFor(_proteinToRemove, _individual);
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
}