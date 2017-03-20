using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface ISolverSettingsFactory
   {
      SolverSettings CreateDefault();
   }

   public class SolverSettingsFactory : ISolverSettingsFactory
   {
      private readonly OSPSuite.Core.Domain.ISolverSettingsFactory _solverSettingsFactory;
      private readonly ICoreUserSettings _userSettings;

      public SolverSettingsFactory(OSPSuite.Core.Domain.ISolverSettingsFactory solverSettingsFactory, ICoreUserSettings userSettings)
      {
         _solverSettingsFactory = solverSettingsFactory;
         _userSettings = userSettings;
      }

      public SolverSettings CreateDefault()
      {
         var solverSettings = _solverSettingsFactory.CreateCVODE();
         solverSettings.AbsTol = _userSettings.AbsTol;
         solverSettings.RelTol = _userSettings.RelTol;
         return solverSettings;
      }
   }
}