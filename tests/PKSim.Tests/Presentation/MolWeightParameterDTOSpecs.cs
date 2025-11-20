using DevExpress.XtraEditors.DXErrorProvider;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_MolWeightParameterDTO : ContextSpecification<MolWeightParameterDTO>
   {
      private IParameter _molWeightParameter;
      protected IParameter _effectiveMolWeightParameter;

      protected override void Context()
      {
         _molWeightParameter = DomainHelperForSpecs.ConstantParameterWithValue(50);
         _effectiveMolWeightParameter = DomainHelperForSpecs.ConstantParameterWithValue(50);

         sut = new MolWeightParameterDTO(_molWeightParameter, _effectiveMolWeightParameter);
      }
   }

   public class When_effective_molecular_weight_is_less_than_min : concern_for_MolWeightParameterDTO
   {
      private ErrorInfo _errorInfo;

      protected override void Context()
      {
         base.Context();
         _errorInfo = new ErrorInfo();
         _effectiveMolWeightParameter.Value = 14;
         _effectiveMolWeightParameter.MinValue = 15;
      }

      protected override void Because()
      {
         sut.GetPropertyError(nameof(_effectiveMolWeightParameter.Value), _errorInfo);
      }

      [Observation]
      public void should_return_an_error_about_effective_molecular_weight()
      {
         _errorInfo.ErrorText.ShouldBeEqualTo(PKSimConstants.Error.EffectiveMolWeightMustBeGreaterThan(_effectiveMolWeightParameter.ConvertToDisplayUnit(_effectiveMolWeightParameter.MinValue.Value), _effectiveMolWeightParameter.DisplayUnit.Name));
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