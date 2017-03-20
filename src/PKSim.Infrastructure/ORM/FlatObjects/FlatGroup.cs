namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatGroup 
   {
      public string Name { get; set; }
      public string ParentGroup { get; set; }
      public string IconName { get; set; }
      public string DisplayName { get; set; }
      public string Description { get; set; }
      public bool Visible { get; set; }
      public int Sequence { get; set; }
      public bool IsAdvanced { get; set; }
      public int Id { get; set; }
      public string PopDisplayName { get; set; }
      public string FullName { get; set; }
   }
}