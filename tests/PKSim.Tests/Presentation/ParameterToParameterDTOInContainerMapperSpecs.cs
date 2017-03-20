using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;
using IParameterToParameterDTOMapper = PKSim.Presentation.DTO.Mappers.IParameterToParameterDTOMapper;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterToParameterDTOInContainerMapper : ContextSpecification<IParameterToParameterDTOInContainerMapper<ParameterContainerForSpecs>>
   {
      protected IParameterToParameterDTOMapper _parameterMapper;
      protected IParameterDTO _result;
      protected IParameter _parameter;
      protected ParameterContainerForSpecs _parameterContainer;
      protected IParameterDTO _parameterDTO;

      protected override void Context()
      {
         _parameterMapper = A.Fake<IParameterToParameterDTOMapper>();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(5);
         _parameterDTO = new ParameterDTO(_parameter);
         _parameterContainer = new ParameterContainerForSpecs();
         A.CallTo(() => _parameterMapper.MapFrom(_parameter)).Returns(_parameterDTO);
         sut = new ParameterToParameterDTOInContainerMapper<ParameterContainerForSpecs>(_parameterMapper);
         _result = sut.MapFrom(_parameter, _parameterContainer, x => x.ParameterValue, x => x.ParmaeterDTO);
      }
   }

   public class When_mapping_a_parameter_to_a_parameter_dto_inside_a_parameter_container : concern_for_ParameterToParameterDTOInContainerMapper
   {
      [Observation]
      public void should_return_a_valide_parameter_dto()
      {
         _result.ShouldBeEqualTo(_parameterDTO);
      }
   }

   public class When_the_mapped_parameter_value_was_changed : concern_for_ParameterToParameterDTOInContainerMapper
   {
      private string _propertyName;

      protected override void Context()
      {
         base.Context();
         _parameterContainer.PropertyChanged += (o, e) => { _propertyName = e.PropertyName; };
      }

      protected override void Because()
      {
         _parameter.Value = 5;
      }

      [Observation]
      public void should_notify_the_property_changed_event_from_the_parameter_container_dto_with_the_name_of_the_property_given_as_in_the_mapping()
      {
         _propertyName.ShouldBeEqualTo("ParameterValue");
      }
   }

   public class ParameterContainerForSpecs : ValidatableDTO
   {
      public double ParameterValue { get; set; }
      public ParameterDTO ParmaeterDTO { get; set; }
   }
}