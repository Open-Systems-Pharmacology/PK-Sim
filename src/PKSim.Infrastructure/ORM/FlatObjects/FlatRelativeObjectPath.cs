namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatRelativeObjectPath
   {
      public int PathId { get; set; }
      public int ParentPathId { get; set; }
      public string ContainerType { get; set; }
      public string ContainerName { get; set; }
   }
}