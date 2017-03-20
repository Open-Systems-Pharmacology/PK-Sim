using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;
using PKSim.Infrastructure.Services;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_NeighborhoodFinalizer : ContextSpecification<INeighborhoodFinalizer>
   {
      protected IFlatNeighborhoodRepository _flatNeighborhoodRepository;
      protected IFlatContainerRepository _flatContainerRepository;

      protected override void Context()
      {
         _flatNeighborhoodRepository =A.Fake<IFlatNeighborhoodRepository>();
         _flatContainerRepository=A.Fake<IFlatContainerRepository>();
         sut = new NeighborhoodFinalizer(_flatNeighborhoodRepository,_flatContainerRepository);
      }
   }

   
   public class When_setting_the_first_and_second_neighboors_of_the_neighbhorhood_defined_in_an_individual : concern_for_NeighborhoodFinalizer
   {
      private PKSim.Core.Model.Organism _organism;
      private List<INeighborhoodBuilder> _neighborhoods;
      private IContainer _firstNeighbor;
      private IContainer _secondNeighbor;
      private INeighborhoodBuilder _neighborhood;

      protected override void Context()
      {
         base.Context();
         _neighborhood = A.Fake<INeighborhoodBuilder>().WithName("toto");
         _organism = A.Fake<PKSim.Core.Model.Organism>();
         _firstNeighbor = A.Fake<IContainer>();
         _secondNeighbor = A.Fake<IContainer>();
         _neighborhoods = new List<INeighborhoodBuilder> { _neighborhood};
         var flatNeighborhood = new FlatNeighborhood {FirstNeighborId = 1, SecondNeighborId = 2};
         A.CallTo(() => _flatNeighborhoodRepository.NeighborhoodFrom(_neighborhood.Name)).Returns(flatNeighborhood);
         var containerPath1 = A.Fake<IObjectPath>();
         var containerPath2 = A.Fake<IObjectPath>();
         A.CallTo(() => _flatContainerRepository.ContainerPathFrom(flatNeighborhood.FirstNeighborId)).Returns(containerPath1);
         A.CallTo(() => _flatContainerRepository.ContainerPathFrom(flatNeighborhood.SecondNeighborId)).Returns(containerPath2);
         A.CallTo(() => containerPath1.Resolve<IContainer>(_organism)).Returns(_firstNeighbor);
         A.CallTo(() => containerPath2.Resolve<IContainer>(_organism)).Returns(_secondNeighbor);
      }

      protected override void Because()
      {
         sut.SetNeighborsIn(_organism,_neighborhoods);
      }

      [Observation]
      public void should_have_set_a_reference_to_the_first_and_second_neighbors_in_each_defined_neighbhorhood()
      {
         _neighborhood.FirstNeighbor.ShouldBeEqualTo(_firstNeighbor);
         _neighborhood.SecondNeighbor.ShouldBeEqualTo(_secondNeighbor);
      }
   }
}	