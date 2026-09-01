using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_RepresentationInfoUpdater : ContextSpecification<IRepresentationInfoUpdater>
   {
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IContainer _organism;
      protected IContainer _liver;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new RepresentationInfoUpdater(_representationInfoRepository);

         _organism = new Container().WithName("Organism");
         _liver = new Container().WithName("Liver");
         _organism.Add(_liver);

         A.CallTo(() => _representationInfoRepository.ContainsInfoFor(A<IObjectBase>._)).Returns(true);
         A.CallTo(() => _representationInfoRepository.InfoFor(_organism)).Returns(new RepresentationInfo {IconName = "Organism", Description = "The organism"});
         A.CallTo(() => _representationInfoRepository.InfoFor(_liver)).Returns(new RepresentationInfo {IconName = "Liver", Description = "The liver"});
      }
   }

   public class When_updating_the_representation_info_in_a_container_structure : concern_for_RepresentationInfoUpdater
   {
      protected override void Because()
      {
         sut.UpdateRepresentationInfoIn(_organism);
      }

      [Observation]
      public void should_start_the_representation_info_repository_so_that_the_lookup_cache_is_filled()
      {
         A.CallTo(() => _representationInfoRepository.Start()).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_icon_of_the_visited_container()
      {
         _organism.Icon.ShouldBeEqualTo("Organism");
      }

      [Observation]
      public void should_set_the_icon_of_the_nested_containers()
      {
         _liver.Icon.ShouldBeEqualTo("Liver");
      }

      [Observation]
      public void should_set_the_description_of_the_visited_containers()
      {
         _liver.Description.ShouldBeEqualTo("The liver");
      }
   }

   public class When_updating_the_representation_info_of_a_container_that_already_has_a_description : concern_for_RepresentationInfoUpdater
   {
      protected override void Context()
      {
         base.Context();
         _liver.Description = "My own description";
      }

      protected override void Because()
      {
         sut.UpdateRepresentationInfoIn(_organism);
      }

      [Observation]
      public void should_keep_the_existing_description()
      {
         _liver.Description.ShouldBeEqualTo("My own description");
      }

      [Observation]
      public void should_still_set_the_icon()
      {
         _liver.Icon.ShouldBeEqualTo("Liver");
      }
   }

   public class When_updating_the_representation_info_of_an_object_that_is_not_known_by_the_repository : concern_for_RepresentationInfoUpdater
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _representationInfoRepository.ContainsInfoFor(_liver)).Returns(false);
      }

      protected override void Because()
      {
         sut.UpdateRepresentationInfoIn(_organism);
      }

      [Observation]
      public void should_leave_the_icon_untouched()
      {
         _liver.Icon.ShouldBeEmpty();
      }
   }
}
