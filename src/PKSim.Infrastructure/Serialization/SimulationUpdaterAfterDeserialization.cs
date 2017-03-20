using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.Serialization
{
   public class SimulationUpdaterAfterDeserialization : ISimulationUpdaterAfterDeserialization
   {
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly IReferencesResolver _referencesResolver;

      public SimulationUpdaterAfterDeserialization(IParameterIdUpdater parameterIdUpdater, IReferencesResolver referencesResolver)
      {
         _parameterIdUpdater = parameterIdUpdater;
         _referencesResolver = referencesResolver;
      }

      public void UpdateSimulation(Simulation simulation)
      {
         if (simulation == null) return;

         _referencesResolver.ResolveReferencesIn(simulation.Model);
         _parameterIdUpdater.UpdateSimulationId(simulation);
      }
   }
}