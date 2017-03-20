namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public enum ObserverBuilderType
   {
      Amount,
      Neighborhood,
      Container
   }

   public class FlatObserver
   {
      public string Name { get; set; }
      public ObserverBuilderType BuilderType { get; set; }
      public string Category { get; set; }
      public bool ForAllMolecules { get; set; }
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public string Dimension { get; set; }
   }
}
