namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatObjectPath
   {
      public int ContainerId { get; set; }
      public string ContainerType { get; set; }
      public string ContainerName { get; set; }
      public string RateObjectName { get; set; }
      public int RelativeObjectPathId { get; set; }
      public bool IsAbsolutePath { get; set; }

      /// <summary>
      /// For molecules: wether molecule amount or molecule concentration 
      /// should be used
      /// </summary>
      public bool UseAmount { get; set; }
   }
}
