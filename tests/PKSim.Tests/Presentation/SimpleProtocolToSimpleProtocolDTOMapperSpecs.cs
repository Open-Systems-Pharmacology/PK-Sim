using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using FakeItEasy;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Protocols;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;



namespace PKSim.Presentation
{
   public abstract class concern_for_SimpleProtocolToSimpleProtocolDTOMapper : ContextSpecification<ISimpleProtocolToSimpleProtocolDTOMapper>
   {
      protected IParameterToParameterDTOMapper _parameterDTOMapper;

      protected override void Context()
      {
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();
         sut = new SimpleProtocolToSimpleProtocolDTOMapper(_parameterDTOMapper);
      }
   }

   public class When_mapping_a_simple_protocol_to_a_simple_protocol_dto : concern_for_SimpleProtocolToSimpleProtocolDTOMapper
   {
      private SimpleProtocolDTO _result;
      private SimpleProtocol _simpleProtocol;
      private ParameterDTO _doseParameterDTO;
      private ParameterDTO _endTimeParameterDTO;

      protected override void Context()
      {
         base.Context();
         _doseParameterDTO = A.Fake<ParameterDTO>();
         _endTimeParameterDTO = A.Fake<ParameterDTO>();
         _simpleProtocol = new SimpleProtocol();
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.DOSE));
         _simpleProtocol.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.Parameters.END_TIME));
         _simpleProtocol.DosingInterval = DosingIntervals.DI_24;
         _simpleProtocol.ApplicationType = ApplicationTypes.Oral;
         A.CallTo(() => _parameterDTOMapper.MapFrom(_simpleProtocol.Dose)).Returns(_doseParameterDTO);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_simpleProtocol.EndTimeParameter)).Returns(_endTimeParameterDTO);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_simpleProtocol);
      }

      [Observation]
      public void should_set_the_application_type_dosing_interval_and_formulation_type_from_the_simple_protocol()
      {
         _result.DosingInterval.ShouldBeEqualTo(_simpleProtocol.DosingInterval);
         _result.ApplicationType.ShouldBeEqualTo(_simpleProtocol.ApplicationType);
      }

      [Observation]
      public void should_have_created_one_parameter_dto_for_the_dose_and_end_time()
      {
         _result.Dose.ShouldBeEqualTo(_doseParameterDTO);
         _result.EndTime.ShouldBeEqualTo(_endTimeParameterDTO);
      }
   }
}