using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Repositories
{
    public interface IPopulationAgeRepository : IStartableRepository<PopulationAgeSettings>
    {
        PopulationAgeSettings PopulationAgeSettingsFrom(string population);
    }
}
