namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatDescriptorConditionBase
   {
      public string Tag { get; set; }
      public bool ShouldHave { get; set; } //true if tag must be available for container
      //false if tag must NOT be defined for container
   }
}
