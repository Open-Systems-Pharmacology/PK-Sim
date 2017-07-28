namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatPopulationContainer : FlatContainerId
   {
      public int ParentId { get; set; }
      public string Population { get; set; }
   }
}