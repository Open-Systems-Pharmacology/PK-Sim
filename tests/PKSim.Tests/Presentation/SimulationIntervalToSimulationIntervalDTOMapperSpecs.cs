using PKSim.Core.Model;
using PKSim.Presentation.DTO.Mappers;

using PKSim.Presentation.DTO.Parameters;

using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationIntervalToSimulationIntervalDTOMapper : ContextSpecification<IOutputIntervalToOutputIntervalDTOMapper>
   {
      protected IParameterToParameterDTOInContainerMapper<OutputIntervalDTO> _parameterDTOMapper;

      protected override void Context()
      {
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOInContainerMapper<OutputIntervalDTO>>();
         sut = new OutputIntervalToOutputIntervalDTOMapper(_parameterDTOMapper);
      }
   }

   
   public class When_mapping_a_simulation_interval_to_a_simulation_interval_dto : concern_for_SimulationIntervalToSimulationIntervalDTOMapper
   {
      private OutputInterval _simulationInterval;
      private OutputIntervalDTO _result;

      protected override void Context()
      {
         base.Context();
         _simulationInterval = A.Fake<OutputInterval>();
         A.CallTo(() => _simulationInterval.StartTime).Returns(A.Fake<IParameter>());
         A.CallTo(() => _simulationInterval.EndTime).Returns(A.Fake<IParameter>());
         A.CallTo(() => _simulationInterval.Resolution).Returns(A.Fake<IParameter>());
         A.CallTo(_parameterDTOMapper).WithReturnType<ParameterDTO>().Returns(A.Fake<ParameterDTO>());
         _result = sut.MapFrom(_simulationInterval);
      }

      [Observation]
      public void the_mapper_should_return_a_simulation_dto_fullfilled_with_the_parameter_information()
      {
         _result.OutputInterval.ShouldBeEqualTo(_simulationInterval);
         _result.StartTimeParameter.ShouldNotBeNull();
         _result.EndTimeParameter.ShouldNotBeNull();
         _result.ResolutionParameter.ShouldNotBeNull();
      }
   }
}	