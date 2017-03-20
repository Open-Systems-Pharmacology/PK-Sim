namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatRateObjectPath : FlatObjectPath
    {
        public string CalculationMethod { get; set; }
        public string Rate { get; set; }
        public string Alias { get; set; }
        public string Dimension { get; set;}
    }
}