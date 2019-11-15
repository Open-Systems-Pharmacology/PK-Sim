namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatCalculationMethodRateDescriptorConditions 
   {
      public string Tag { get; set; }
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public CriteriaCondition Condition { get; set; }
   }
}
