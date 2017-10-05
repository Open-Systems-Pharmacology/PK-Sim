using System.Threading.Tasks;
using PKSim.Core.Model;
using SnapshotSolverSettings = PKSim.Core.Snapshots.SolverSettings;
using ModelSolverSettings = OSPSuite.Core.Domain.SolverSettings;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SolverSettingsMapper : SnapshotMapperBase<ModelSolverSettings, SnapshotSolverSettings>
   {
      private readonly ISolverSettingsFactory _solverSettingsFactory;
      private readonly ModelSolverSettings _defaultSolverSettings;

      public SolverSettingsMapper(ISolverSettingsFactory solverSettingsFactory)
      {
         _solverSettingsFactory = solverSettingsFactory;
         _defaultSolverSettings = _solverSettingsFactory.CreateDefault();
      }

      public override Task<SnapshotSolverSettings> MapToSnapshot(ModelSolverSettings solverSettings)
      {
         return SnapshotFrom(solverSettings, snapshot =>
         {
            snapshot.AbsTol = SnapshotValueFor(solverSettings.AbsTol, _defaultSolverSettings.AbsTol);
            snapshot.RelTol = SnapshotValueFor(solverSettings.RelTol, _defaultSolverSettings.RelTol);
            snapshot.UseJacobian = SnapshotValueFor(solverSettings.UseJacobian, _defaultSolverSettings.UseJacobian);
            snapshot.H0 = SnapshotValueFor(solverSettings.H0, _defaultSolverSettings.H0);
            snapshot.HMin = SnapshotValueFor(solverSettings.HMin, _defaultSolverSettings.HMin);
            snapshot.HMax = SnapshotValueFor(solverSettings.HMax, _defaultSolverSettings.HMax);
            snapshot.MxStep = SnapshotValueFor(solverSettings.MxStep, _defaultSolverSettings.MxStep);
         });
      }

      public override Task<ModelSolverSettings> MapToModel(SnapshotSolverSettings snapshot)
      {
         var solverSettings = _solverSettingsFactory.CreateDefault();
         if (snapshot == null)
            return Task.FromResult(solverSettings);

         solverSettings.AbsTol = ModelValueFor(snapshot.AbsTol, _defaultSolverSettings.AbsTol);
         solverSettings.RelTol = ModelValueFor(snapshot.RelTol, _defaultSolverSettings.RelTol);
         solverSettings.UseJacobian = ModelValueFor(snapshot.UseJacobian, _defaultSolverSettings.UseJacobian);
         solverSettings.H0 = ModelValueFor(snapshot.H0, _defaultSolverSettings.H0);
         solverSettings.HMin = ModelValueFor(snapshot.HMin, _defaultSolverSettings.HMin);
         solverSettings.HMax = ModelValueFor(snapshot.HMax, _defaultSolverSettings.HMax);
         solverSettings.MxStep = ModelValueFor(snapshot.MxStep, _defaultSolverSettings.MxStep);

         return Task.FromResult(solverSettings);
      }
   }
}