namespace PKSim.Infrastructure.ORM.FlatObjects
{
   /// <summary>
   /// Mapping of compound process parameters measured in-vitro, in plasma etc.
   /// Parameters mapped are usually Organ/Compartment volumes, blood flows, etc.
   /// </summary>
   public class FlatCompoundProcessParameterMapping
   {
      public int ProcessId { get; set;}
      public string ProcessName { get; set; }
      public string ParameterName { get; set; }

      public int ContainerId { get; set; }
      public string ContainerType { get; set; }
      public string ContainerName { get; set; }
      public string ContainerParameterName { get; set; }
   }
}
