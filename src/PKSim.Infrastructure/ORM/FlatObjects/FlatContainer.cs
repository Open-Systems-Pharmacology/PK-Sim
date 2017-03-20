namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatContainer : FlatContainerId
   {
      public string ContainerPath { get; set; }
      public int? ParentId { get; set; }
      public string ParentType { get; set; }
      public string ParentName { get; set; }
      public bool Visible { get; set; }
      public string Extension { get; set; }
      public bool IsLogical { get; set; }
   }
}