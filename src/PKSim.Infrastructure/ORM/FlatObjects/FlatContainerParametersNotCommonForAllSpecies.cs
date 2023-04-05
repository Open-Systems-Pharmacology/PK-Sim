namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatContainerParametersNotCommonForAllSpecies
   {
      public int ContainerId { get; set; }
      public string ContainerType { get; set; }
      public string ContainerName { get; set; }
      public string ParameterName { get; set; }
      public int SpeciesCount { get; set; }
   }
}