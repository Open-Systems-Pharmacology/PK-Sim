using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Snapshots.Mappers;
using ISolverSettingsFactory = PKSim.Core.Model.ISolverSettingsFactory;

namespace PKSim.Core
{
   public abstract class concern_for_SolverSettingsMapper : ContextSpecificationAsync<SolverSettingsMapper>
   {
      protected SolverSettings _solverSettings;
      protected Snapshots.SolverSettings _snapshot;
      protected ISolverSettingsFactory _solverSettingsFactory;

      protected override Task Context()
      {
         _solverSettingsFactory = A.Fake<ISolverSettingsFactory>();
     
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

         var solverSettings = A.Fake<SolverSettings>();
         solverSettings.AbsTol = 1;
         solverSettings.RelTol = 2;
         solverSettings.MxStep = 3;
         solverSettings.HMin = 4;
         solverSettings.H0 = 5;
         solverSettings.HMax = 6;
         solverSettings.UseJacobian = false;
         A.CallTo(() => _solverSettingsFactory.CreateDefault()).Returns(solverSettings);

         sut = new SolverSettingsMapper(_solverSettingsFactory);
         return _completed;
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

   public class When_mapping_a_solver_setting_snapshot_to_solver_settings : concern_for_SolverSettingsMapper
   {
      private SolverSettings _newSolverSettings;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_solverSettings);

         _snapshot.H0 = null;
         _snapshot.HMax = null;
      }

      protected override async Task Because()
      {
         _newSolverSettings = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_update_values_that_were_set_in_the_snapshot()
      {
         _newSolverSettings.AbsTol.ShouldBeEqualTo(_snapshot.AbsTol.Value);
         _newSolverSettings.RelTol.ShouldBeEqualTo(_snapshot.RelTol.Value);
         _newSolverSettings.MxStep.ShouldBeEqualTo(_snapshot.MxStep.Value);
         _newSolverSettings.HMin.ShouldBeEqualTo(_snapshot.HMin.Value);
         _newSolverSettings.UseJacobian.ShouldBeEqualTo(_snapshot.UseJacobian.Value);
      }

      [Observation]
      public void should_leave_default_value_as_is()
      {
         _newSolverSettings.H0.ShouldBeEqualTo(5);
         _newSolverSettings.HMax.ShouldBeEqualTo(6);
      }
   }
}