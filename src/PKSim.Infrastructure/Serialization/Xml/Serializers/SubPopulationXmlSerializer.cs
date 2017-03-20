using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
    public class SubPopulationXmlSerializer : BaseXmlSerializer<SubPopulation>
    {
        public override void PerformMapping()
        {
            MapEnumerable(x => x.ParameterValueVersions,x => x.AddParameterValueVersion);
        }
    }
}