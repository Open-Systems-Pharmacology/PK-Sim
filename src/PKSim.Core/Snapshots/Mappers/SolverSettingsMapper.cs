using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using ISolverSettingsFactory = PKSim.Core.Model.ISolverSettingsFactory;
using SnapshotSolverSettings = PKSim.Core.Snapshots.SolverSettings;
using ModelSolverSettings = OSPSuite.Core.Domain.SolverSettings;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SolverSettingsMapper : SnapshotMapperBase<ModelSolverSettings, SnapshotSolverSettings>
   {
      private readonly ISolverSettingsFactory _solverSettingsFactory;

      public SolverSettingsMapper(ISolverSettingsFactory solverSettingsFactory)
      {
         _solverSettingsFactory = solverSettingsFactory;
      }

      public override Task<SnapshotSolverSettings> MapToSnapshot(ModelSolverSettings solverSettings)
      {
         return SnapshotFrom(solverSettings, snapshot =>
         {
            snapshot.AbsTol = solverSettings.AbsTol;
            snapshot.RelTol = solverSettings.RelTol;

            //only map other parameters if the have changed from default
            snapshot.UseJacobian = mapSolverParameterValue<bool>(solverSettings, Constants.Parameters.USE_JACOBIAN);
            snapshot.H0 = mapSolverParameterValue<double>(solverSettings, Constants.Parameters.H0);
            snapshot.HMin = mapSolverParameterValue<double>(solverSettings, Constants.Parameters.H_MIN);
            snapshot.HMax = mapSolverParameterValue<double>(solverSettings, Constants.Parameters.H_MAX);
            snapshot.MxStep = mapSolverParameterValue<int>(solverSettings, Constants.Parameters.MX_STEP);
         });
      }

      private T? mapSolverParameterValue<T>(ModelSolverSettings solverSettings, string parameterName) where T : struct
      {
         var parameter = solverSettings.Parameter(parameterName);
         if (parameter.ParameterHasChanged())
            return parameter.Value.ConvertedTo<T>();

         return null;
      }

      public override Task<ModelSolverSettings> MapToModel(SnapshotSolverSettings snapshot)
      {
         var solverSettings = _solverSettingsFactory.CreateDefault();
         solverSettings.AbsTol = setSolverParameterValues(snapshot.AbsTol, solverSettings.AbsTol);
         solverSettings.RelTol = setSolverParameterValues(snapshot.RelTol, solverSettings.RelTol);
         solverSettings.UseJacobian = setSolverParameterValues(snapshot.UseJacobian, solverSettings.UseJacobian);
         solverSettings.H0 = setSolverParameterValues(snapshot.H0, solverSettings.H0);
         solverSettings.HMin = setSolverParameterValues(snapshot.HMin, solverSettings.HMin);
         solverSettings.HMax = setSolverParameterValues(snapshot.HMax, solverSettings.HMax);
         solverSettings.MxStep = setSolverParameterValues(snapshot.MxStep, solverSettings.MxStep);

         return Task.FromResult(solverSettings);
      }

      private T setSolverParameterValues<T>(T? snapshotValue, T solverDefaultValue) where T : struct
      {
         if (snapshotValue.HasValue)
            return snapshotValue.Value;

         return solverDefaultValue;
      }
   }
}