using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationIntervalDTO : ContextSpecification<OutputIntervalDTO>
   {
      protected override void Context()
      {
         sut = new OutputIntervalDTO();
      }
   }

   public class When_a_simulation_interval_is_defined_with_a_start_time_equal_to_end_time : concern_for_SimulationIntervalDTO
   {
      protected override void Because()
      {
         sut.EndTimeParameter = new ParameterDTO(DomainHelperForSpecs.ConstantParameterWithValue(10));
         sut.StartTimeParameter = new ParameterDTO(DomainHelperForSpecs.ConstantParameterWithValue(10));
      }

      [Observation]
      public void it_should_be_invalid()
      {
         sut.IsValid().ShouldBeFalse();
      }
   }

   public class When_a_simulation_interval_is_defined_with_a_start_time_smaller_than_end_time_but_the_ui_values_are_in_differnet_unit : concern_for_SimulationIntervalDTO
   {
      protected override void Context()
      {
         base.Context();
         var dimension = new Dimension(new BaseDimensionRepresentation {TimeExponent = 1}, "Time", "min");
         dimension.AddUnit("h", 60, 0);

         sut.StartTimeParameter = new ParameterDTO(DomainHelperForSpecs.ConstantParameterWithValue(20).WithDimension(dimension));
         sut.StartTimeParameter.Parameter.DisplayUnit = dimension.Unit("min");

         sut.EndTimeParameter = new ParameterDTO(DomainHelperForSpecs.ConstantParameterWithValue(120).WithDimension(dimension));
         sut.EndTimeParameter.Parameter.DisplayUnit = dimension.Unit("h");
      }

      [Observation]
      public void it_should_be_valid()
      {
         sut.IsValid().ShouldBeTrue();
      }
   }
}