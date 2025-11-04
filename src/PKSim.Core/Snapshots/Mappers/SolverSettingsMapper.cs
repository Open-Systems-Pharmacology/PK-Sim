using ISolverSettingsFactory = PKSim.Core.Model.ISolverSettingsFactory;
using ModelSolverSettings = OSPSuite.Core.Domain.SolverSettings;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SolverSettingsMapper : OSPSuite.Core.Snapshots.Mappers.SolverSettingsMapper
   {
      private readonly ISolverSettingsFactory _solverSettingsFactory;

      public SolverSettingsMapper(ISolverSettingsFactory solverSettingsFactory) : base(solverSettingsFactory.CreateDefault())
      {
         _solverSettingsFactory = solverSettingsFactory;
      }

      protected override ModelSolverSettings CreateDefault()
      {
         return _solverSettingsFactory.CreateDefault();
      }
   }
}