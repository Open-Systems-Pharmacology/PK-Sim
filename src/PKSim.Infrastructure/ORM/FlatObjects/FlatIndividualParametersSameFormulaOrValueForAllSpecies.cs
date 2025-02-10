namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatIndividualParametersSameFormulaOrValueForAllSpecies
   {
      public int ContainerId { get; set; }
      public string ContainerType { get; set; }
      public string ContainerName { get; set; }
      public string ParameterName { get; set; }
      public bool IsSameFormula { get; set; }
      public void Deconstruct(out int containerId, out string parameterName, out bool isSameFormula, out string containerType, out string containerName)
      {
         containerId = ContainerId;
         parameterName = ParameterName;
         isSameFormula = IsSameFormula;
         containerType = ContainerType;
         containerName = ContainerName;
      }
   }
}
