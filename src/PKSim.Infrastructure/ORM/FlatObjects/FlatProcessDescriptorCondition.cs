namespace PKSim.Infrastructure.ORM.FlatObjects
{
   /// <summary>
   /// Source and Target are used for transport source/target container conditions
   /// Parent is used e.g. for reactions (parent) container conditions
   /// </summary>
   public enum ProcessTagType
   {
      Source,
      Target,
      Parent
   }

   public class FlatProcessDescriptorCondition : FlatDescriptorConditionBase
   {
      public string Process { get; set;}
      public ProcessTagType TagType { get; set; } 
   }
}
