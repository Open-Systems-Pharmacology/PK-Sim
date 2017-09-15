using System;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotSolverSettings = PKSim.Core.Snapshots.SolverSettings;
using ModelSolverSettings = OSPSuite.Core.Domain.SolverSettings;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SolverSettingsMapper : SnapshotMapperBase<ModelSolverSettings, SnapshotSolverSettings>
   {
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
         throw new NotImplementedException();
      }
   }
}