namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatModelContainerMolecule : FlatContainerId
   {
      public string Model { get; set; }
      public string Molecule { get; set; }
      public bool IsPresent { get; set; }
      public bool NegativeValuesAllowed { get; set; }
   }
}