namespace PKSim.Infrastructure.ORM.FlatObjects
{
    public class FlatPopulation : FlatObject
    {
        public int RaceIndex{ get; set; }
        public string Species { get; set; }
        public bool IsAgeDependent { get; set; }
        public bool IsHeightDependent { get; set; }
        public bool IsBodySurfaceAreaDependent { get; set; }
        public int Sequence{ get; set; }
    }
}