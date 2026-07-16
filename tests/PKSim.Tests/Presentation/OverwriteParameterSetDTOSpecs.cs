using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_OverwriteParameterSetDTO : ContextSpecification<OverwriteParameterSetDTO>
   {
      protected OverwriteParameterSet _overwriteParameterSet;

      protected override void Context()
      {
         _overwriteParameterSet = new OverwriteParameterSet { Name = "TestSet", IsDefault = true };
      }
   }

   public class When_the_underlying_overwrite_parameter_set_has_species_and_disease_state_extended_properties : concern_for_OverwriteParameterSetDTO
   {
      protected override void Context()
      {
         base.Context();
         _overwriteParameterSet.ExtendedProperties.Add(new ExtendedProperty<string> { Name = "Species", Value = "Human" });
         _overwriteParameterSet.ExtendedProperties.Add(new ExtendedProperty<string> { Name = "Disease State", Value = "CKD" });
      }

      protected override void Because()
      {
         sut = new OverwriteParameterSetDTO(_overwriteParameterSet);
      }

      [Observation]
      public void should_return_the_species_from_the_extended_property()
      {
         sut.Species.ShouldBeEqualTo("Human");
      }

      [Observation]
      public void should_return_the_disease_state_from_the_extended_property()
      {
         sut.DiseaseState.ShouldBeEqualTo("CKD");
      }
   }

   public class When_the_underlying_overwrite_parameter_set_has_no_extended_properties : concern_for_OverwriteParameterSetDTO
   {
      protected override void Because()
      {
         sut = new OverwriteParameterSetDTO(_overwriteParameterSet);
      }

      [Observation]
      public void should_return_empty_string_for_species()
      {
         sut.Species.ShouldBeEqualTo(string.Empty);
      }

      [Observation]
      public void should_return_empty_string_for_disease_state()
      {
         sut.DiseaseState.ShouldBeEqualTo(string.Empty);
      }
   }
}
