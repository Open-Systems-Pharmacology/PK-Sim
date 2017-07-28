using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Queries;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ModelContainerQuery : ContextSpecification<IModelContainerQuery>
   {
      private IFlatModelContainerRepository _modelContainerRepo;
      private IFlatPopulationContainerRepository _populationContainerRepo;
      private IFlatContainerRepository _flatContainerRepo;
      protected IEntityPathResolver _entityPathResolver;
      protected IFlatContainerIdToContainerMapper _flatContainerIdToContainerMapper;
      protected IContainer _compartment;
      private FlatModelContainer _flatModelCompartment;
      protected string _populationName;

      protected override void Context()
      {
         _entityPathResolver = new EntityPathResolver(new ObjectPathFactoryForSpecs());
         _flatContainerIdToContainerMapper = A.Fake<IFlatContainerIdToContainerMapper>();
         _modelContainerRepo = A.Fake<IFlatModelContainerRepository>();
         _compartment = A.Fake<IContainer>();
         _populationName = "HUMAN_POP";

         A.CallTo(() => _modelContainerRepo.All()).Returns(
            new List<FlatModelContainer>
            {
               NewFMC(1, "C1", "ORGAN", "M1"),
               NewFMC(2, "C2", "COMPARTMENT", "M1"),
               NewFMC(3, Constants.ROOT, "SIMULATION", "M1")
            });

         _populationContainerRepo = A.Fake<IFlatPopulationContainerRepository>();
         A.CallTo(() => _populationContainerRepo.AllSubContainerFor(_populationName, 3)).Returns(
            new List<FlatPopulationContainer>
            {
               CreateFlatPopulationContainer(1, "C1", "ORGAN", _populationName),
            });

         A.CallTo(() => _populationContainerRepo.AllSubContainerFor(_populationName, 1)).Returns(
            new List<FlatPopulationContainer>
            {
               CreateFlatPopulationContainer(2, "C2", "COMPARTMENT", _populationName)
            });


         _flatContainerRepo = A.Fake<IFlatContainerRepository>();
         var simulationFlatContainer = NewFC(3, Constants.ROOT, "SIMULATION", null, "", "");
         var organFlatContainer = NewFC(1, "C1", "ORGAN", 3, "SIMULATION", Constants.ROOT);
         var compartmentFlatContainer = NewFC(2, "C2", "COMPARTMENT", 1, "C1", "ORGAN");
         A.CallTo(() => _flatContainerRepo.All()).Returns(
            new List<FlatContainer>
            {
               organFlatContainer,
               compartmentFlatContainer,
               simulationFlatContainer
            });

         A.CallTo(() => _flatContainerRepo.ParentContainerFrom(organFlatContainer.Id)).Returns(simulationFlatContainer);
         A.CallTo(() => _flatContainerRepo.ParentContainerFrom(compartmentFlatContainer.Id)).Returns(organFlatContainer);
         A.CallTo(() => _flatContainerRepo.ParentContainerFrom(simulationFlatContainer.Id)).Returns(null);

         A.CallTo(() => _flatContainerRepo.ContainerFrom(organFlatContainer.Id)).Returns(organFlatContainer);
         A.CallTo(() => _flatContainerRepo.ContainerFrom(compartmentFlatContainer.Id)).Returns(compartmentFlatContainer);
         A.CallTo(() => _flatContainerRepo.ContainerFrom(simulationFlatContainer.Id)).Returns(simulationFlatContainer);

         _flatModelCompartment = new FlatModelContainer {Id = compartmentFlatContainer.Id};
         A.CallTo(() => _flatContainerRepo.ContainerFrom("ROOT")).Returns(organFlatContainer);
         A.CallTo(() => _flatContainerRepo.ContainerFrom("C1")).Returns(organFlatContainer);

         A.CallTo(() => _modelContainerRepo.AllSubContainerFor("M1",1)).Returns(new[] {_flatModelCompartment});
         A.CallTo(() => _flatContainerIdToContainerMapper.MapFrom(_flatModelCompartment)).Returns(_compartment);
         sut = new ModelContainerQuery(_modelContainerRepo, _populationContainerRepo,
            _flatContainerRepo, _flatContainerIdToContainerMapper, _entityPathResolver);
      }

      private FlatModelContainer NewFMC(int id, string name, string type, string model)
      {
         return new FlatModelContainer {Id = id, Name = name, Type = type, Model = model, UsageInIndividual = "REQUIRED"};
      }

      private FlatPopulationContainer CreateFlatPopulationContainer(int id, string name, string type, string species)
      {
         return new FlatPopulationContainer {Id = id, Name = name, Type = type, Population = species};
      }

      private FlatContainer NewFC(int id, string name, string type, int? parentId, string parentName, string parentType)
      {
         return new FlatContainer {Id = id, Name = name, Type = type, ParentId = parentId, ParentName = parentName, ParentType = parentType};
      }
   }

   public class When_getting_subcontainers_defined_for_a_given_organ : concern_for_ModelContainerQuery
   {
      private IEnumerable<IContainer> _subcontainers;
      private Organ _organ;
      private ModelConfiguration _modelConfiguration;
      private SpeciesPopulation _population;

      protected override void Context()
      {
         base.Context();

         _modelConfiguration = A.Fake<ModelConfiguration>();
         _modelConfiguration.ModelName = "M1";
         _population = new SpeciesPopulation().WithName(_populationName);
         _organ = new Organ().WithName("C1");
         _organ.ParentContainer = A.Fake<Simulation>().WithName(Constants.ROOT);
      }

      protected override void Because()
      {
         _subcontainers = sut.SubContainersFor(_population, _modelConfiguration, _organ);
      }

      [Observation]
      public void should_return_the_defined_compartments()
      {
         _subcontainers.Contains(_compartment).ShouldBeTrue();
      }
   }
}