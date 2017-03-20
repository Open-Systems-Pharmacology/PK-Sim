using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_FullPathDisplayResolver : ContextSpecification<IFullPathDisplayResolver>
   {
      protected Individual _individual;
      protected IRepresentationInfoRepository _representationInfoRep;
      protected IQuantityPathToQuantityDisplayPathMapper _quantityPathMapper;

      protected override void Context()
      {
         _individual = DomainHelperForSpecs.CreateIndividual();
         _representationInfoRep = A.Fake<IRepresentationInfoRepository>();
         _quantityPathMapper = A.Fake<IQuantityPathToQuantityDisplayPathMapper>();
         sut = new FullPathDisplayResolver(_representationInfoRep, _quantityPathMapper);
      }
   }

   public class When_resolving_a_full_path_to_display_for_a_parameter : concern_for_FullPathDisplayResolver
   {
      private IParameter _parameter;
      private string _result;

      protected override void Context()
      {
         base.Context();
         var organ = _individual.Organism.Organ("Liver");
         _parameter = organ.Parameter("PLiver");
         A.CallTo(() => _quantityPathMapper.DisplayPathAsStringFor(_parameter, false)).Returns("P in Liver");
      }

      protected override void Because()
      {
         _result = sut.FullPathFor(_parameter);
      }

      [Observation]
      public void should_return_the_path_of_the_parameter_in_the_hierarchy_using_only_the_display_information_of_its_parent_container_up_to_the_organ()
      {
         _result.ShouldBeEqualTo("P in Liver");
      }
   }
}