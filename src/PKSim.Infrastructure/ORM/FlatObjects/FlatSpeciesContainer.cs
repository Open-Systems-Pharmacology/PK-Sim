namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatSpeciesContainer : FlatContainerId
   {
      public int ParentId { get; set; }
      public string Species { get; set; }
   }
}