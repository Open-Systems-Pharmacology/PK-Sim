namespace PKSim.Infrastructure.ORM.FlatObjects
{
    public class FlatCalculationMethod : FlatObject
    {
        public string Category { get; set; }
        public bool NeedsLicense { get; set; }
        public int Sequence { get; set; }
    }
}