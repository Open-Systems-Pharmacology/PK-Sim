using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
    public abstract class CategoryItemXmlSerializer<TCategoryIem> : BaseXmlSerializer<TCategoryIem> where TCategoryIem : CategoryItem
    {
        public override void PerformMapping()
        {
            Map(x => x.Name);
        }
    }
}