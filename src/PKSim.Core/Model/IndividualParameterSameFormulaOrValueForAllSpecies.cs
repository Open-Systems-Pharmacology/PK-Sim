namespace PKSim.Core.Model
{
   public class IndividualParameterSameFormulaOrValueForAllSpecies
   {
      public int ContainerId { get; set; }
      public string ContainerPath { get; set; }
      public string ParameterName { get; set; }
      public bool IsSameFormula { get; set; }
      public bool IsSameValue => !IsSameFormula;
      public bool IsSame => IsSameFormula || IsSameValue;
   }
}