using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public interface ISimulationSettingsFactory
   {
      SimulationSettings CreateFor(Simulation simulation);
   }

   public class SimulationSettingsFactory : ISimulationSettingsFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IOutputSchemaFactory _outputSchemaFactory;
      private readonly ISolverSettingsFactory _solverSettingsFactory;

      public SimulationSettingsFactory(IObjectBaseFactory objectBaseFactory, IOutputSchemaFactory outputSchemaFactory, ISolverSettingsFactory solverSettingsFactory)
      {
         _objectBaseFactory = objectBaseFactory;
         _outputSchemaFactory = outputSchemaFactory;
         _solverSettingsFactory = solverSettingsFactory;
      }

      public SimulationSettings CreateFor(Simulation simulation)
      {
         //was already defined..nothing to do
         if (simulation.Settings != null)
            return simulation.Settings;

         var settings = _objectBaseFactory.Create<SimulationSettings>();
         settings.OutputSchema = _outputSchemaFactory.CreateFor(simulation);
         settings.Solver = _solverSettingsFactory.CreateDefault();
         return settings;
      }
   }
}