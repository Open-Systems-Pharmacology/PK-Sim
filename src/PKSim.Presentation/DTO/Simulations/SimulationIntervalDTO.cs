using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Presentation.DTO.Parameters;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationIntervalDTO : ValidatableDTO
   {
      public OutputInterval SimulationInterval { get; set; }
      public ParameterDTO StartTimeParameter { get; set; }
      public ParameterDTO EndTimeParameter { get; set; }
      public ParameterDTO ResolutionParameter { get; set; }

      public SimulationIntervalDTO()
      {
         Rules.Add(startTimeLessThanEndTime);
         Rules.Add(endTimeGreaterThanStartTime);
      }

      public double StartTime
      {
         get { return StartTimeParameter.Value; }
         set { StartTimeParameter.Value = value; }
      }

      public double EndTime
      {
         get { return EndTimeParameter.Value; }
         set { EndTimeParameter.Value = value; }
      }

      public double Resolution
      {
         get { return ResolutionParameter.Value; }
         set { ResolutionParameter.Value = value; }
      }

      private IBusinessRule startTimeLessThanEndTime
      {
         get
         {
            return CreateRule.For<SimulationIntervalDTO>()
               .Property(x => x.StartTime)
               .WithRule((dto, value) => StartTimeParameter.Parameter.ConvertToBaseUnit(value) < EndTimeParameter.KernelValue)
               .WithError(PKSimConstants.Rules.Parameter.StartTimeLessThanOrEqualToEndTime);
         }
      }

      private IBusinessRule endTimeGreaterThanStartTime
      {
         get
         {
            return CreateRule.For<SimulationIntervalDTO>()
               .Property(x => x.EndTime)
               .WithRule((dto, value) => EndTimeParameter.Parameter.ConvertToBaseUnit(value) > StartTimeParameter.KernelValue)
               .WithError(PKSimConstants.Rules.Parameter.EndTimeGreaterThanOrEqualToStartTime);
         }
      }
   }
}