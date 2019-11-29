using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
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
         sut = new Services.FullPathDisplayResolver(_quantityPathMapper, _representationInfoRep);
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

   public class When_resolving_the_full_path_display_for_a_parameter_alternative_group : concern_for_FullPathDisplayResolver
   {
      private ParameterAlternativeGroup _parameterAlternativeGroup;
      private string _result;

      protected override void Context()
      {
         base.Context();
         _parameterAlternativeGroup = new ParameterAlternativeGroup();
         A.CallTo(() => _representationInfoRep.DisplayNameFor(_parameterAlternativeGroup)).Returns("DISPLAY_GROUP");
      }

      protected override void Because()
      {
         _result = sut.FullPathFor(_parameterAlternativeGroup);
      }

      [Observation]
      public void should_simply_return_the_display_name_of_the_parameter_group()
      {
         _result.ShouldBeEqualTo("DISPLAY_GROUP");
      }
   }
}