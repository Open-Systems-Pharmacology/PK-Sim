using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
    public abstract class FlatObject :IWithId
    {
        public string Id { get; set; }
    }
}