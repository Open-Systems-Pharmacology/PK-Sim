using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_SolverSettingsMapper : ContextSpecificationAsync<SolverSettingsMapper>
   {
      protected SolverSettings _solverSettings;
      protected Snapshots.SolverSettings _snapshot;

      protected override Task Context()
      {
         sut = new SolverSettingsMapper();

         _solverSettings = new SolverSettings
         {
            DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.Parameters.USE_JACOBIAN),
            DomainHelperForSpecs.ConstantParameterWithValue(1.5).WithName(Constants.Parameters.H0),
            DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(Constants.Parameters.H_MIN),
            DomainHelperForSpecs.ConstantParameterWithValue(2.5).WithName(Constants.Parameters.H_MAX),
            DomainHelperForSpecs.ConstantParameterWithValue(100).WithName(Constants.Parameters.MX_STEP),
            DomainHelperForSpecs.ConstantParameterWithValue(1e-5).WithName(Constants.Parameters.REL_TOL),
            DomainHelperForSpecs.ConstantParameterWithValue(1e-7).WithName(Constants.Parameters.ABS_TOL)
         };
         return Task.FromResult(true);
      }
   }

   public class When_mapping_the_solver_settings_to_snapshot : concern_for_SolverSettingsMapper
   {

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_solverSettings);
      }

      [Observation]
      public void should_be_able_to_save_all_solver_parameter_that_where_changed_by_the_user()
      {
         _snapshot.UseJacobian.ShouldBeEqualTo(_solverSettings.UseJacobian);
         _snapshot.AbsTol.ShouldBeEqualTo(_solverSettings.AbsTol);
         _snapshot.RelTol.ShouldBeEqualTo(_solverSettings.RelTol);
         _snapshot.MxStep.ShouldBeEqualTo(_solverSettings.MxStep);
         _snapshot.HMin.ShouldBeEqualTo(_solverSettings.HMin);
         _snapshot.HMax.ShouldBeEqualTo(_solverSettings.HMax);
         _snapshot.H0.ShouldBeEqualTo(_solverSettings.H0);
   }
   }
}