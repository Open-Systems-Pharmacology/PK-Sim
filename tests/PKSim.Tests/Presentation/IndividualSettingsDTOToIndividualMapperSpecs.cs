using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualSettingsDTOToIndividualMapper : ContextSpecification<IIndividualSettingsDTOToIndividualMapper>
   {
      protected IIndividualSettingsDTOToOriginDataMapper _mapper;
      protected IIndividualFactory _individualFactory;
      protected IndividualSettingsDTO _individualSettingsDTO;

      protected override void Context()
      {
         _mapper = A.Fake<IIndividualSettingsDTOToOriginDataMapper>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _individualSettingsDTO =new IndividualSettingsDTO();
         sut = new IndividualSettingsDTOToIndividualMapper(_individualFactory, _mapper);
      }
   }

   
   public class When_mapping_a_individual_dto_to_an_individual : concern_for_IndividualSettingsDTOToIndividualMapper
   {
      private PKSim.Core.Model.Individual _result;
      private OriginData _originData;
      private PKSim.Core.Model.Individual _individual;

      protected override void Context()
      {
         base.Context();
         _originData = new OriginData();
         _individual = A.Fake<PKSim.Core.Model.Individual>();
         A.CallTo(() => _mapper.MapFrom(_individualSettingsDTO)).Returns(_originData);
         A.CallTo(() => _individualFactory.CreateAndOptimizeFor(_originData)).Returns(_individual);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_individualSettingsDTO);
      }

      [Observation]
      public void should_ask_the_individual_factory_to_create_an_individual()
      {
         _result.ShouldBeEqualTo(_individualFactory.CreateAndOptimizeFor(_originData));
      }
   }
}