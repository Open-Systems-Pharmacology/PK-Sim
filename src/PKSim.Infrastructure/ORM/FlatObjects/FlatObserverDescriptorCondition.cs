namespace PKSim.Infrastructure.ORM.FlatObjects
{

   public enum ObserverTagType
   {
      PARENT,
      FIRST_NEIGHBOR,
      SECOND_NEIGHBOR
   }

   public class FlatObserverDescriptorCondition : FlatDescriptorConditionBase
   {
      public string Observer { get; set; }
      public ObserverTagType TagType { get; set; }
   }
}
