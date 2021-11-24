using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_MoleculeExpressionTask : ContextSpecification<IMoleculeExpressionTask<Individual>>
   {
      protected Individual _individual;
      protected IExecutionContext _executionContext;
      private IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      protected IndividualMolecule _molecule;
      protected MoleculeExpressionContainer _moleculeContainer1;
      protected MoleculeExpressionContainer _moleculeContainer2;
      protected IOntogenyRepository _ontogenyRepository;
      protected Ontogeny _ontogeny;
      protected ISimulationSubjectExpressionTask<Individual> _subjectExpressionTask;
      protected IExpressionProfileUpdater _expressionProfileUpdater;

      protected override void Context()
      {
         _individual = new Individual {OriginData = new OriginData {Species = new Species().WithName("Human")}};
         _ontogeny = new DatabaseOntogeny {Name = "toto"};
         _executionContext = A.Fake<IExecutionContext>();
         _individualMoleculeFactoryResolver = A.Fake<IIndividualMoleculeFactoryResolver>();
         _ontogenyRepository = A.Fake<IOntogenyRepository>();
         var proteinFactory = A.Fake<IIndividualMoleculeFactory>();
         _expressionProfileUpdater = A.Fake<IExpressionProfileUpdater>();
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
            _expressionProfileUpdater
            );
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