namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatNeighborhood : FlatContainerId
   {
      public int FirstNeighborId { get; set; }
      public string FirstNeighborType { get; set; }
      public string FirstNeighborName { get; set; }

      public int SecondNeighborId { get; set; }
      public string SecondNeighborType { get; set; }
      public string SecondNeighborName { get; set; }
   }
}
