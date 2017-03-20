namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public enum ProcessTagType
   {
      Source,
      Target
   }

   public class FlatProcessDescriptorCondition : FlatDescriptorConditionBase
   {
      public string Process { get; set;}
      public ProcessTagType TagType { get; set; } 
   }
}
