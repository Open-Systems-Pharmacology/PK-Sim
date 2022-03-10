namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatContainerParameterDescriptorCondition
   {
      public string ParameterName { get; set; }
      public int ContainerId { get; set; }
      public string ContainerType { get; set; }
      public string ContainerName { get; set; }
      public string Tag { get; set; }
      public CriteriaCondition Condition { get; set; }
   }
}