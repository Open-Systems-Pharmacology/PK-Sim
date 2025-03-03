using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualModelTask : ContextSpecification<IIndividualModelTask>
   {
      protected ISpeciesContainerQuery _speciesContainerQuery;
      protected IParameterContainerTask _parameterContainerTask;
      protected Compartment _compartment;
      protected Organ _organ;
      protected Organism _organism;
      protected IBuildingBlockFinalizer _buildingBlockFinalizer;
      protected IFormulaFactory _formulaFactory;
      protected IPopulationAgeRepository _populationAgeRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _speciesContainerQuery = A.Fake<ISpeciesContainerQuery>();
         _parameterContainerTask = A.Fake<IParameterContainerTask>();
         _buildingBlockFinalizer = A.Fake<IBuildingBlockFinalizer>();
         _formulaFactory = A.Fake<IFormulaFactory>();
         _populationAgeRepository = A.Fake<IPopulationAgeRepository>();
         _compartment = new Compartment().WithName("compartment");
         _organ = new Organ().WithName("organ");
         _organ.Name = "organ";
         _organism = new Organism().WithName("organism");
         sut = new IndividualModelTask(_parameterContainerTask, _speciesContainerQuery, _buildingBlockFinalizer, _formulaFactory, _populationAgeRepository);
      }
   }

   public class When_requested_to_create_an_organism_for_the_specified_origin_data : concern_for_IndividualModelTask
   {
      private OriginData _originData;
      private Individual _individual;
      private IContainer _neighborhoods;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _individual = new Individual();
         _originData = new OriginData();
         _neighborhoods = new Container().WithName(Constants.NEIGHBORHOODS);
         _individual.OriginData = _originData;
         _individual.Add(_organism);
         _individual.Add(_neighborhoods);
         _originData.Population = new SpeciesPopulation();
         A.CallTo(() => _speciesContainerQuery.SubContainersFor(_originData.Population, _individual)).Returns(new[] { _organism });
         A.CallTo(() => _speciesContainerQuery.SubContainersFor(_originData.Population, _organism)).Returns(new[] { _organ });
         A.CallTo(() => _speciesContainerQuery.SubContainersFor(_originData.Population, _organ)).Returns(new[] { _compartment });
         A.CallTo(() => _speciesContainerQuery.SubContainersFor(_originData.Population, _compartment)).Returns(new List<IContainer>());
         A.CallTo(() => _speciesContainerQuery.SubContainersFor(_originData.Population, _neighborhoods)).Returns(new List<IContainer>());
      }

      protected override void Because()
      {
         sut.CreateModelFor(_individual);
      }

      [Observation]
      public void should_create_an_organism_containing_the_appropriate_structure()
      {
         _organism.ContainsName(_organ.Name).ShouldBeTrue();
         _organ.ContainsName(_compartment.Name).ShouldBeTrue();
      }

      [Observation]
      public void should_add_all_individual_parameters_to_organism_and_its_sub_organs_and_compartments()
      {
         A.CallTo(() => _parameterContainerTask.AddIndividualParametersTo<IContainer>(_organism, _originData)).MustHaveHappened();
         A.CallTo(() => _parameterContainerTask.AddIndividualParametersTo<IContainer>(_organ, _originData)).MustHaveHappened();
         A.CallTo(() => _parameterContainerTask.AddIndividualParametersTo<IContainer>(_compartment, _originData)).MustHaveHappened();
      }

      [Observation]
      public void should_add_all_individual_parameters_to_the_neighborhoods_of_the_individual()
      {
         A.CallTo(() => _parameterContainerTask.AddIndividualParametersTo(_neighborhoods, _originData)).MustHaveHappened();
      }

      [Observation]
      public void should_finalize_the_created_individual()
      {
         A.CallTo(() => _buildingBlockFinalizer.Finalize(_individual)).MustHaveHappened();
      }
   }

   public class OrganismEqualityComparer : GenericEqualityComparer<Organism>
   {
   }

   public class OrganEqualityComparer : GenericEqualityComparer<Organ>
   {
   }

   public class ContainerEqualityComparer : GenericEqualityComparer<Container>
   {
   }

   public class CompartmentEqualityComparer : GenericEqualityComparer<Compartment>
   {
   }
}