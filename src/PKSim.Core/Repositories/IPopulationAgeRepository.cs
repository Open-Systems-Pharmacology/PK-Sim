using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
    public interface IPopulationAgeRepository : IStartableRepository<PopulationAgeSettings>
    {
        PopulationAgeSettings PopulationAgeSettingsFrom(string population);
    }
}
