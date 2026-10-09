using DevExpress.XtraEditors.DXErrorProvider;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Format;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_MolWeightParameterDTO : ContextSpecification<EffectiveMolWeightParameterDTO>
   {
      protected IParameter _effectiveMolWeightParameter;
      protected IParameter _molWeightParameter;

      protected override void Context()
      {
         _effectiveMolWeightParameter = DomainHelperForSpecs.ConstantParameterWithValue(50);
         _molWeightParameter = DomainHelperForSpecs.ConstantParameterWithValue(60);

         sut = new EffectiveMolWeightParameterDTO(_effectiveMolWeightParameter, _molWeightParameter);
      }
   }

   public class When_the_molecular_weight_is_displayed_in_another_unit_than_the_effective_molecular_weight : concern_for_MolWeightParameterDTO
   {
      protected override void Context()
      {
         base.Context();
         _molWeightParameter.DisplayUnit = _molWeightParameter.Dimension.Unit("cm");
      }

      [Observation]
      public void should_display_the_effective_molecular_weight_in_the_display_unit_of_the_molecular_weight()
      {
         sut.DisplayUnit.ShouldBeEqualTo(_molWeightParameter.DisplayUnit);
      }

      [Observation]
      public void should_return_the_effective_molecular_weight_value_in_the_display_unit_of_the_molecular_weight()
      {
         sut.Value.ShouldBeEqualTo(_effectiveMolWeightParameter.ConvertToUnit(_molWeightParameter.DisplayUnit));
      }
   }

   public class When_effective_molecular_weight_is_less_than_min_and_the_molecular_weight_is_displayed_in_another_unit : concern_for_MolWeightParameterDTO
   {
      private ErrorInfo _errorInfo;
      private NumericFormatter<double> _numericFormatter;
      private Unit _molWeightDisplayUnit;

      protected override void Context()
      {
         base.Context();
         _errorInfo = new ErrorInfo();
         _effectiveMolWeightParameter.Value = 14;
         _effectiveMolWeightParameter.MinValue = 15;
         _molWeightDisplayUnit = _molWeightParameter.Dimension.Unit("cm");
         _molWeightParameter.DisplayUnit = _molWeightDisplayUnit;
         _numericFormatter = new(NumericFormatterOptions.Instance);
      }

      protected override void Because()
      {
         sut.GetPropertyError(nameof(_effectiveMolWeightParameter.Value), _errorInfo);
      }

      [Observation]
      public void should_report_the_minimum_value_in_the_display_unit_of_the_molecular_weight()
      {
         _errorInfo.ErrorText.Contains(PKSimConstants.Error.EffectiveMolWeightMustBeGreaterThan(_numericFormatter.Format(_effectiveMolWeightParameter.ConvertToUnit(_effectiveMolWeightParameter.MinValue.Value, _molWeightDisplayUnit)), _molWeightDisplayUnit.Name)).ShouldBeTrue();
      }
   }

   public class When_effective_molecular_weight_is_less_than_min : concern_for_MolWeightParameterDTO
   {
      private ErrorInfo _errorInfo;
      private NumericFormatter<double> _numericFormatter;

      protected override void Context()
      {
         base.Context();
         _errorInfo = new ErrorInfo();
         _effectiveMolWeightParameter.Value = 14;
         _effectiveMolWeightParameter.MinValue = 15;
         _numericFormatter = new(NumericFormatterOptions.Instance);
      }

      protected override void Because()
      {
         sut.GetPropertyError(nameof(_effectiveMolWeightParameter.Value), _errorInfo);
      }

      [Observation]
      public void should_return_an_error_about_effective_molecular_weight()
      {
         _errorInfo.ErrorText.Contains(PKSimConstants.Error.EffectiveMolWeightMustBeGreaterThan(_numericFormatter.Format(_effectiveMolWeightParameter.ConvertToDisplayUnit(_effectiveMolWeightParameter.MinValue.Value)), _effectiveMolWeightParameter.DisplayUnit.Name)).ShouldBeTrue();
      }

      [Observation]
      public void should_mark_the_error_as_critical()
      {
         _errorInfo.ErrorType.ShouldBeEqualTo(ErrorType.Critical);
      }
   }

   public class When_effective_molecular_weight_is_greater_than_min : concern_for_MolWeightParameterDTO
   {
      private ErrorInfo _errorInfo;

      protected override void Context()
      {
         base.Context();
         _errorInfo = new ErrorInfo();
         _effectiveMolWeightParameter.Value = 100;
         _effectiveMolWeightParameter.MinValue = 15;
      }

      protected override void Because()
      {
         sut.GetPropertyError(nameof(_effectiveMolWeightParameter.Value), _errorInfo);
      }

      [Observation]
      public void should_not_return_an_error_for_value_validation()
      {
         string.IsNullOrEmpty(_errorInfo.ErrorText).ShouldBeTrue();
      }
   }

   public class When_effective_molecular_weight_is_not_set : concern_for_MolWeightParameterDTO
   {
      private ErrorInfo _errorInfo;

      protected override void Context()
      {
         base.Context();
         _errorInfo = new ErrorInfo();
         _effectiveMolWeightParameter.Value = 100;
      }

      protected override void Because()
      {
         sut.GetPropertyError(nameof(_effectiveMolWeightParameter.Value), _errorInfo);
      }

      [Observation]
      public void should_not_return_an_error_for_value_validation()
      {
         string.IsNullOrEmpty(_errorInfo.ErrorText).ShouldBeTrue();
      }
   }
}