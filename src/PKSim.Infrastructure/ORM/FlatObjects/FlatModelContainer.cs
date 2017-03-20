namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatModelContainer : FlatContainerId
   {
      public string Model { get; set; }
      public string UsageInIndividual { get; set; }
      public int ParentId { get; set; }
   }
}