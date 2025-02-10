namespace PKSim.Core.Model
{
   public class IndividualParameterBySpecies
   {
      public int ContainerId { get; set; }
      public string ContainerPath { get; set; }
      public string ParameterName { get; set; }
      public int SpeciesCount { get; set; }
   }
}