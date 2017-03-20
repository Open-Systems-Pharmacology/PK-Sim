using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;

using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterScalingToParameterScalingDTOMapper : ContextSpecification<IParameterScalingToParameterScalingDTOMapper>
   {
      protected IFullPathDisplayResolver _fullPathDisplayResolver;
      protected IParameterToParameterDTOMapper _parameterDTOMapper;

      protected override void Context()
      {
         _fullPathDisplayResolver = A.Fake<IFullPathDisplayResolver>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();
         sut = new ParameterScalingToParameterScalingDTOMapper(_fullPathDisplayResolver, _parameterDTOMapper);
      }
   }

   public class When_a_parameter_scaling_dto_mapper_is_told_to_map_a_parameter_scaling_into_a_parameter_scaling_dto : concern_for_ParameterScalingToParameterScalingDTOMapper
   {
      private ParameterScaling _parameterScaling;
      private ParameterScalingDTO _result;
      private IParameter _sourceParameter;
      private IParameter _targetParameter;
      private ScalingMethod _scalingMethod;
      private double _newValueInGuiUnit;
      private string _displayFullPath;
      private ParameterDTO _targetParameterDTO;
      private ParameterDTO _sourceParameterDTO;

      protected override void Context()
      {
         base.Context();
         _newValueInGuiUnit = 145;
         _targetParameterDTO = A.Fake<ParameterDTO>();
         _sourceParameterDTO = A.Fake<ParameterDTO>();
         _parameterScaling = A.Fake<ParameterScaling>();
         _sourceParameter = A.Fake<IParameter>();
         _targetParameter = A.Fake<IParameter>();
         A.CallTo(() => _targetParameterDTO.Value).Returns(5);
         A.CallTo(() => _sourceParameterDTO.Value).Returns(6);
         _displayFullPath = "tralalal";
         _scalingMethod = A.Fake<ScalingMethod>();
         A.CallTo(() => _parameterScaling.SourceParameter).Returns(_sourceParameter);
         A.CallTo(() => _parameterScaling.TargetParameter).Returns(_targetParameter);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_sourceParameter)).Returns(_sourceParameterDTO);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_targetParameter)).Returns(_targetParameterDTO);
         _parameterScaling.ScalingMethod = _scalingMethod;
         A.CallTo(_fullPathDisplayResolver).WithReturnType<string>().Returns(_displayFullPath);
         A.CallTo(() => _parameterScaling.TargetScaledValueInDisplayUnit).Returns(_newValueInGuiUnit);
         A.CallTo(() => _parameterScaling.SourceValueInDisplayUnit).Returns(_sourceParameterDTO.Value);
         A.CallTo(() => _parameterScaling.TargetDefaultValueInDisplayUnit).Returns(_targetParameterDTO.Value);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_parameterScaling);
      }

      [Observation]
      public void should_map_the_elements_of_the_parameter_scaling_correctly_to_the_dto_properties()
      {
         _result.ParameterFullPathDisplay.ShouldBeEqualTo(_displayFullPath);
         _result.SourceParameter.ShouldBeEqualTo(_sourceParameterDTO);
         _result.TargetParameter.ShouldBeEqualTo(_targetParameterDTO);
         _result.TargetDefaultValue.ShouldBeEqualTo(_targetParameterDTO.Value);
         _result.SourceValue.ShouldBeEqualTo(_sourceParameterDTO.Value);
         _result.ScalingMethod.ShouldBeEqualTo(_parameterScaling.ScalingMethod);
         _result.ParameterScaling.ShouldBeEqualTo(_parameterScaling);
         _result.TargetScaledValue.ShouldBeEqualTo(_newValueInGuiUnit);
      }
   }
}