using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Extensions
{
   public static class SimulationConfigurationExtensions
   {
      /// <summary>
      /// Short cut to access the one and only module defined in PK-Sim per configuration
      /// </summary>
      /// <returns>The first module from the module configuration</returns>
      public static Module Module(this SimulationConfiguration simulationConfiguration)
      {
         return simulationConfiguration.ModuleConfigurations.Select(x => x.Module).First();
      }

      public static SpatialStructure SpatialStructure(this SimulationConfiguration simulationConfiguration)
      {
         return simulationConfiguration.Module().SpatialStructure;
      }
   }
}