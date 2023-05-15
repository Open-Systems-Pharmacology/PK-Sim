using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.DiseaseStates;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_DiseaseStateUpdater : ContextSpecification<IDiseaseStateUpdater>
   {
      protected ICloner _cloner;
      protected IParameterToParameterDTOMapper _parameterMapper;
      protected IDiseaseStateRepository _diseaseStateRepository;
      protected IOriginDataParameterToParameterDTOMapper _originDataParameterMapper;
      protected IParameterDTOToOriginDataParameterMapper _parameterMapperDTOToOriginDataMapper;

      protected override void Context()
      {
         _cloner = A.Fake<ICloner>();
         _parameterMapper = A.Fake<IParameterToParameterDTOMapper>();
         _diseaseStateRepository = A.Fake<IDiseaseStateRepository>();
         _originDataParameterMapper = A.Fake<IOriginDataParameterToParameterDTOMapper>();
         _parameterMapperDTOToOriginDataMapper = new ParameterDTOToOriginDataParameterMapper();

         sut = new DiseaseStateUpdater(_cloner, _parameterMapper, _diseaseStateRepository, _originDataParameterMapper, _parameterMapperDTOToOriginDataMapper);
      }
   }

   public class When_mapping_an_individual_with_disease_state_parameter_to_an_individual_dto : concern_for_DiseaseStateUpdater
   {
      private OriginData _origin;
      private OriginDataParameter _diseaseStateParameter;
      private IParameterDTO _diseaseStateParameterDTO;
      private DiseaseStateDTO _diseaseStateDTO;

      protected override void Context()
      {
         base.Context();
         _diseaseStateDTO = new DiseaseStateDTO();
         _diseaseStateParameterDTO = A.Fake<IParameterDTO>();
         _origin = new OriginData
         {
            DiseaseState = new DiseaseState {Name = "MyDiseaseState"}
         };
         _diseaseStateParameter = new OriginDataParameter();
         _origin.AddDiseaseStateParameter(_diseaseStateParameter);
         A.CallTo(() => _originDataParameterMapper.MapFrom(_diseaseStateParameter)).Returns(_diseaseStateParameterDTO);
      }

      protected override void Because()
      {
         sut.UpdateDiseaseStateDTO(_diseaseStateDTO, _origin);
      }

      [Observation]
      public void should_set_take_the_disease_state_from_the_individual()
      {
         _diseaseStateDTO.Value.ShouldBeEqualTo(_origin.DiseaseState);
      }

      [Observation]
      public void should_update_the_disease_state_parameter()
      {
         _diseaseStateDTO.Parameter.ShouldBeEqualTo(_diseaseStateParameterDTO);
      }
   }

   //write test testing the update from origin data
   public class When_updating_an_origian_data_from_a_disease_state_DTO : concern_for_DiseaseStateUpdater
   {
      private OriginData _origin;
      private IParameterDTO _diseaseStateParameterDTO;
      private DiseaseStateDTO _diseaseStateDTO;
      private DiseaseState _ckdDiseaseState;

      protected override void Context()
      {
         base.Context();
         _origin = new OriginData();
         _ckdDiseaseState = new DiseaseState {Name = "CKD"};
         _diseaseStateParameterDTO = A.Fake<IParameterDTO>().WithName("TOTO");
         _diseaseStateDTO = new DiseaseStateDTO();

         A.CallTo(() => _diseaseStateParameterDTO.KernelValue).Returns(10);
         _diseaseStateParameterDTO.DisplayUnit = new Unit("mg", 1, 0);
         _diseaseStateDTO.Value = _ckdDiseaseState;
         _diseaseStateDTO.Parameter = _diseaseStateParameterDTO;
      }

      protected override void Because()
      {
         sut.UpdateOriginDataFromDiseaseState(_origin, _diseaseStateDTO);
      }

      [Observation]
      public void the_returned_origin_data_should_be_filled_with_the_expected_disease_State()
      {
         _origin.DiseaseState.ShouldBeEqualTo(_ckdDiseaseState);
         _origin.DiseaseStateParameters.Count.ShouldBeEqualTo(1);
         var diseaseStateParameter = _origin.DiseaseStateParameters.FindByName(_diseaseStateParameterDTO.Name);
         diseaseStateParameter.Value.ShouldBeEqualTo(_diseaseStateParameterDTO.KernelValue);
         diseaseStateParameter.Unit.ShouldBeEqualTo("mg");
      }
   }
}