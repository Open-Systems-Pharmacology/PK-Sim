using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FlatContainerRepository : ContextForIntegration<IFlatContainerRepository>
   {
   }

   public class when_getting_container : concern_for_FlatContainerRepository
   {
      private Simulation _sim;
      private Organism _organism;
      private Organ _organ;
      protected EntityPathResolver _entityPathResolver;

      protected override void Because()
      {
         _sim = new IndividualSimulation().WithName("Tralala");

         _organism = new Organism().WithName(Constants.ORGANISM);
         _sim.Add(_organism);

         _organ = new Organ().WithName("Liver");
         _organism.Add(_organ);

         _entityPathResolver = new EntityPathResolverForSpecs();
      }

      [Observation]
      public void should_return_organ_from_path()
      {
         var organContainer = sut.ContainerFrom(_entityPathResolver.PathFor(_organ));
         organContainer.Type.ShouldBeEqualTo(CoreConstants.ContainerType.Organ);
         organContainer.Name.ShouldBeEqualTo(_organ.Name);
      }

      [Observation]
      public void should_return_parent_for_organ()
      {
         var organContainer = sut.ContainerFrom(_entityPathResolver.PathFor(_organ));
         var organismContainer = sut.ContainerFrom(_entityPathResolver.PathFor(_organism));
         sut.ParentContainerFrom(organContainer.Id).ShouldBeEqualTo(organismContainer);
         sut.ParentContainerFrom(organContainer.Id).ShouldBeEqualTo(organismContainer);
      }
   }
}