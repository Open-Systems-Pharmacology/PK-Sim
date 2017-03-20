namespace PKSim.Infrastructure.ORM.FlatObjects
{
    public class FlatModelCalculationMethod
    {
        public string Model { get; set; }
        public string Category { get; set; }
        public string CalculationMethod { get; set; }
        public bool IsDefault { get; set; }
        public int Sequence{ get; set; }
    }
}