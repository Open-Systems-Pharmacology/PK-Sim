using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.ParameterIdentifications;
using PKSim.Core.Snapshots.Mappers;
using CalculationMethodCache = PKSim.Core.Snapshots.CalculationMethodCache;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterIdentificationRunModeMapper : ContextSpecificationAsync<ParameterIdentificationRunModeMapper>
   {
      protected CalculationMethodCacheMapper _calculationMethodMapper;
      protected Snapshots.ParameterIdentificationRunMode _snapshot;

      protected override Task Context()
      {
         _calculationMethodMapper= A.Fake<CalculationMethodCacheMapper>();
         sut = new ParameterIdentificationRunModeMapper(_calculationMethodMapper);

         return _completed;
      }
   }

   public class When_mapping_a_parameter_identification_standard_run_to_snapshot : concern_for_ParameterIdentificationRunModeMapper
   {
      private ParameterIdentificationRunMode _standardRunMode;
      protected override async Task Context()
      {
         await base.Context();
         _standardRunMode = new StandardParameterIdentificationRunMode();
      }
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_standardRunMode);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_run_mode()
      {
         _snapshot.Name.ShouldBeEqualTo(typeof(StandardParameterIdentificationRunMode).Name);
      }

      [Observation]
      public void should_have_set_all_other_parameters_to_null()
      {
         _snapshot.NumberOfRuns.ShouldBeNull();
         _snapshot.AllTheSameSelection.ShouldBeNull();
         _snapshot.CalculationMethods.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_identification_multiple_run_to_snapshot : concern_for_ParameterIdentificationRunModeMapper
   {
      private MultipleParameterIdentificationRunMode _multipleRunMode;
      protected override async Task Context()
      {
         await base.Context();
         _multipleRunMode = new MultipleParameterIdentificationRunMode {NumberOfRuns = 10};
      }
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_multipleRunMode);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_run_mode()
      {
         _snapshot.Name.ShouldBeEqualTo(typeof(MultipleParameterIdentificationRunMode).Name);
      }

      [Observation]
      public void should_have_set_the_number_of_run()
      {
         _snapshot.NumberOfRuns.ShouldBeEqualTo(_multipleRunMode.NumberOfRuns);
      }

      [Observation]
      public void should_have_set_all_other_parameters_to_null()
      {
         _snapshot.AllTheSameSelection.ShouldBeNull();
         _snapshot.CalculationMethods.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_identification_categorial_all_the_same_run_to_snapshot : concern_for_ParameterIdentificationRunModeMapper
   {
      private CategorialParameterIdentificationRunMode _categorialParameterIdentificationRunMode;
      private CalculationMethodCache _calculationMethodSnapshot;

      protected override async Task Context()
      {
         await base.Context();
         _calculationMethodSnapshot = new CalculationMethodCache();
         _categorialParameterIdentificationRunMode = new CategorialParameterIdentificationRunMode {AllTheSame = true};
         A.CallTo(() => _calculationMethodMapper.MapToSnapshot(_categorialParameterIdentificationRunMode.AllTheSameSelection)).Returns(_calculationMethodSnapshot);
      }
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_categorialParameterIdentificationRunMode);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_run_mode()
      {
         _snapshot.Name.ShouldBeEqualTo(typeof(CategorialParameterIdentificationRunMode).Name);
      }

      [Observation]
      public void should_have_set_the_used_calculation_methods()
      {
         _snapshot.AllTheSameSelection.ShouldBeEqualTo(_calculationMethodSnapshot);
      }

      [Observation]
      public void should_have_set_all_other_parameters_to_null()
      {
         _snapshot.NumberOfRuns.ShouldBeNull();
         _snapshot.CalculationMethods.ShouldBeNull();
      }
   }

   public class When_mapping_a_parameter_identification_categorial_run_with_different_compoound_settings_to_snapshot : concern_for_ParameterIdentificationRunModeMapper
   {
      private CategorialParameterIdentificationRunMode _categorialParameterIdentificationRunMode;
      private CalculationMethodCache _calculationMethodSnapshot;
      private OSPSuite.Core.Domain.CalculationMethodCache _compoundCalculationMethod;

      protected override async Task Context()
      {
         await base.Context();
         _compoundCalculationMethod = new OSPSuite.Core.Domain.CalculationMethodCache();
         _calculationMethodSnapshot = new CalculationMethodCache();
         _categorialParameterIdentificationRunMode = new CategorialParameterIdentificationRunMode { AllTheSame = false };
         _categorialParameterIdentificationRunMode.CalculationMethodsCache["C1"] = _compoundCalculationMethod;
         A.CallTo(() => _calculationMethodMapper.MapToSnapshot(_compoundCalculationMethod)).Returns(_calculationMethodSnapshot);
      }
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_categorialParameterIdentificationRunMode);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_run_mode()
      {
         _snapshot.Name.ShouldBeEqualTo(typeof(CategorialParameterIdentificationRunMode).Name);
      }

      [Observation]
      public void should_have_set_the_used_calculation_methods()
      {
         _snapshot.CalculationMethods["C1"].ShouldBeEqualTo(_calculationMethodSnapshot);
      }

      [Observation]
      public void should_have_set_all_other_parameters_to_null()
      {
         _snapshot.NumberOfRuns.ShouldBeNull();
         _snapshot.AllTheSameSelection.ShouldBeNull();
      }
   }

}