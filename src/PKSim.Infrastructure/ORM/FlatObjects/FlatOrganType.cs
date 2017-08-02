using PKSim.Core.Model;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
    public class FlatOrganType
    {
        public string OrganName { get; set; }
        public OrganType OrganType { get; set; }
    }
}