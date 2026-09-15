using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_ParameterToParameterCommitDTOMapper : ContextSpecification<ParameterToParameterCommitDTOMapper>
   {
      protected override void Context()
      {
         sut = new ParameterToParameterCommitDTOMapper();
      }
   }

   public class When_mapping_a_parameter_to_commit_dto : concern_for_ParameterToParameterCommitDTOMapper
   {
      private ParameterCommitDTO _result;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(3.5).WithName("Lipophilicity");
         _parameter.Dimension = DomainHelperForSpecs.LengthDimensionForSpecs();
         _parameter.DisplayUnit = _parameter.Dimension.Unit("cm");
         _parameter.ValueOrigin.Source = ValueOriginSources.Publication;
         _parameter.ValueOrigin.Description = "From literature";
      }

      protected override void Because()
      {
         _result = sut.MapFrom("Organism|Aspirin|Lipophilicity", _parameter, isRemoval: false);
      }

      [Observation]
      public void should_set_the_path()
      {
         _result.Path.ShouldBeEqualTo("Organism|Aspirin|Lipophilicity");
      }

      [Observation]
      public void should_set_the_value_in_the_display_unit()
      {
         _result.Value.ShouldBeEqualTo(350);
      }

      [Observation]
      public void should_set_the_display_unit()
      {
         _result.Unit.ShouldBeEqualTo("cm");
      }

      [Observation]
      public void should_set_the_value_origin()
      {
         _result.ValueOrigin.ShouldBeEqualTo(_parameter.ValueOrigin.Display);
      }

      [Observation]
      public void should_be_selected_by_default()
      {
         _result.Selected.ShouldBeTrue();
      }

      [Observation]
      public void should_describe_the_change_as_an_update_of_the_value()
      {
         _result.IsRemoval.ShouldBeFalse();
         _result.Change.ShouldBeEqualTo(PKSimConstants.UI.UpdateValueInParameterSet);
      }
   }

   public class When_mapping_a_reset_parameter_to_commit_dto : concern_for_ParameterToParameterCommitDTOMapper
   {
      private ParameterCommitDTO _result;

      protected override void Because()
      {
         _result = sut.MapFrom("Organism|Aspirin|Lipophilicity", DomainHelperForSpecs.ConstantParameterWithValue(3.5), isRemoval: true);
      }

      [Observation]
      public void should_describe_the_change_as_a_removal_from_the_set()
      {
         _result.IsRemoval.ShouldBeTrue();
         _result.Change.ShouldBeEqualTo(PKSimConstants.UI.RemoveFromParameterSet);
      }
   }

   public class When_mapping_a_null_parameter_to_commit_dto : concern_for_ParameterToParameterCommitDTOMapper
   {
      private ParameterCommitDTO _result;

      protected override void Because()
      {
         _result = sut.MapFrom("Organism|Aspirin|Missing", null, isRemoval: false);
      }

      [Observation]
      public void should_set_value_to_nan()
      {
         double.IsNaN(_result.Value).ShouldBeTrue();
      }
   }
}
