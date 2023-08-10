namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatIndividualParametersNotCommonForAllSpecies
   {
      public int ContainerId { get; set; }
      public string ContainerType { get; set; }
      public string ContainerName { get; set; }
      public string ParameterName { get; set; }
      public int SpeciesCount { get; set; }

      public void Deconstruct(out int containerId, out string parameterName, out int speciesCount, out string containerName)
      {
         containerId = ContainerId;
         parameterName = ParameterName;
         speciesCount = SpeciesCount;
         containerName = ContainerName;
      }
   }
}