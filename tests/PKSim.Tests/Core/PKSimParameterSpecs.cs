using System.Globalization;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimParameter : ContextSpecification<IParameter>
   {
      protected double _originValue;
      protected bool _valueChangedEventRaised;

      protected override void Context()
      {
         _originValue = 15;
         var container = new Container();
         var otherParameter = DomainHelperForSpecs.ConstantParameterWithValue(10).WithName("P");
         var objectPathFactory = new ObjectPathFactoryForSpecs();
         sut = new PKSimParameter().WithName("toto");
         container.Add(otherParameter);
         container.Add(sut);
         sut.Formula = new ExplicitFormula(_originValue.ToString(new NumberFormatInfo()));
         sut.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(sut, otherParameter));
         sut.PropertyChanged += (o, e) =>
                                   {
                                      if (e.PropertyName.Equals("Value"))
                                         _valueChangedEventRaised = true;
                                   };
      }
   }

   
   public class When_setting_the_value_for_a_parameter : concern_for_PKSimParameter
   {
      protected override void Because()
      {
         sut.Value = 20;
      }

      [Observation]
      public void the_parameter_should_be_marked_as_fixed()
      {
         sut.IsFixedValue.ShouldBeTrue();
      }

      [Observation]
      public void the_parameter_formula_should_not_have_been_changed()
      {
         sut.Formula.Calculate(sut).ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_value_changed_event_should_have_been_raised()
      {
         _valueChangedEventRaised.ShouldBeTrue();
      }
   }

   
   public class When_reseting_a_parameter_to_is_default_value : concern_for_PKSimParameter
   {
      protected override void Context()
      {
         base.Context();
         sut.Value = 20;
      }

      protected override void Because()
      {
         sut.ResetToDefault();
      }

      [Observation]
      public void the_parameter_should_not_be_fixed_anymore()
      {
         sut.IsFixedValue.ShouldBeFalse();
      }

      [Observation]
      public void the_parameter_formula_should_not_have_been_changed()
      {
         sut.Formula.Calculate(sut).ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_parameter_value_should_be_equal_to_the_formula_value()
      {
         sut.Value.ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_value_changed_event_should_have_been_raised()
      {
         _valueChangedEventRaised.ShouldBeTrue();
      }
   }

   
   public class When_setting_a_parameter_to_a_fixed_value_then_to_default_and_to_the_same_fixed_value : concern_for_PKSimParameter
   {
      protected override void Context()
      {
         base.Context();
         sut.Value = 20;
         sut.ResetToDefault();
         _valueChangedEventRaised = false;
      }

      protected override void Because()
      {
         sut.Value = 20;
      }

      [Observation]
      public void the_value_changed_event_should_have_been_raised()
      {
         _valueChangedEventRaised.ShouldBeTrue();
      }
   }

   
   public class When_asking_if_a_parameter_is_changed_by_the_create_individual_algorithm : concern_for_PKSimParameter
   {
      private Container _liver;
      private Container _fat;
      private Container _muscle;
      private Container _duodenum;
      private PKSimParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _fat = new Container().WithName(CoreConstants.Organ.FAT);
         _muscle = new Container().WithName(CoreConstants.Organ.MUSCLE);
         _liver = new Container().WithName(CoreConstants.Organ.LIVER);
         _duodenum = new Container().WithName(CoreConstants.Compartment.DUODENUM);
         _parameter = new PKSimParameter {BuildingBlockType = PKSimBuildingBlockType.Individual};
      }

      [Observation]
      public void should_return_true_for_a_volume_parameter()
      {
         _parameter.WithName(Constants.Parameters.VOLUME).IsChangedByCreateIndividual.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_volume_parameter_that_is_not_an_individual_parameter()
      {
         new PKSimParameter{BuildingBlockType = PKSimBuildingBlockType.Simulation}.WithName(Constants.Parameters.VOLUME).IsChangedByCreateIndividual.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_for_an_height_parameter()
      {
         _parameter.WithName(CoreConstants.Parameters.HEIGHT).IsChangedByCreateIndividual.ShouldBeTrue();
         _parameter.WithName(CoreConstants.Parameters.MEAN_HEIGHT).IsChangedByCreateIndividual.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_for_a_weight_parameter()
      {
         _parameter.WithName(CoreConstants.Parameters.WEIGHT).IsChangedByCreateIndividual.ShouldBeTrue();
         _parameter.WithName(CoreConstants.Parameters.MEAN_WEIGHT).IsChangedByCreateIndividual.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_for_a_bmi_parameter()
      {
         _parameter.WithName(CoreConstants.Parameters.BMI).IsChangedByCreateIndividual.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_for_an_age_parameter()
      {
         _parameter.WithName(CoreConstants.Parameters.AGE).IsChangedByCreateIndividual.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_for_an_ontogeny_parameter_defined_in_liver_or_duodenum()
      {

         _parameter.WithName(CoreConstants.Parameters.ONTOGENY_FACTOR).IsChangedByCreateIndividual.ShouldBeTrue();
         _parameter.WithName(CoreConstants.Parameters.ONTOGENY_FACTOR_GI).IsChangedByCreateIndividual.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_for_a_volume_fraction_lipid_parameters_defined_in_fat()
      {
         foreach (var name in CoreConstants.Parameters.VolumeFractionLipidsParameters)
         {
            _parameter.WithParentContainer(_fat).WithName(name).IsChangedByCreateIndividual.ShouldBeTrue();
         }
      }

      [Observation]
      public void should_return_false_for_a_volume_fraction_lipid_parameters_defined_in_muscle()
      {
         foreach (var name in CoreConstants.Parameters.VolumeFractionLipidsParameters)
         {
            _parameter.WithParentContainer(_muscle).WithName(name).IsChangedByCreateIndividual.ShouldBeFalse();
         }
      }

      [Observation]
      public void should_return_false_for_a_volume_fraction_lipid_parameters_not_defined_in_muscle()
      {
         foreach (var name in CoreConstants.Parameters.VolumeFractionLipidsParameters)
         {
            _parameter.WithParentContainer(_liver).WithName(name).IsChangedByCreateIndividual.ShouldBeFalse();
         }
      }

      [Observation]
      public void should_return_true_for_a_volume_fraction_protein_parameters_defined_in_muscle()
      {
         foreach (var name in CoreConstants.Parameters.VolumeFractionProteinsParameters)
         {
            _parameter.WithParentContainer(_muscle).WithName(name).IsChangedByCreateIndividual.ShouldBeTrue();
         }
      }

      [Observation]
      public void should_return_false_for_a_volume_fraction_protein_parameters_defined_in_fat()
      {
         foreach (var name in CoreConstants.Parameters.VolumeFractionProteinsParameters)
         {
            _parameter.WithParentContainer(_fat).WithName(name).IsChangedByCreateIndividual.ShouldBeFalse();
         }
      }

      [Observation]
      public void should_return_false_for_a_volume_fraction_protein_parameters_not_defined_in_fat()
      {
         foreach (var name in CoreConstants.Parameters.VolumeFractionProteinsParameters)
         {
            _parameter.WithParentContainer(_liver).WithName(name).IsChangedByCreateIndividual.ShouldBeFalse();
         }
      }

      [Observation]
      public void should_return_true_for_a_volume_fraction_water_parameters_defined_in_fat_or_muscle()
      {
         foreach (var name in CoreConstants.Parameters.VolumeFractionWaterParameters)
         {
            _parameter.WithParentContainer(_fat).WithName(name).IsChangedByCreateIndividual.ShouldBeTrue();
            _parameter.WithParentContainer(_muscle).WithName(name).IsChangedByCreateIndividual.ShouldBeTrue();
         }
      }

      [Observation]
      public void should_return_false_for_a_volume_fraction_water_parameters_defined_in_an_organ_that_is_not_fat_or_muscle()
      {
         foreach (var name in CoreConstants.Parameters.VolumeFractionWaterParameters)
         {
            _parameter.WithParentContainer(_liver).WithName(name).IsChangedByCreateIndividual.ShouldBeFalse();
         }
      }
   }

   
   public class When_setting_the_display_unit_of_a_parameter : concern_for_PKSimParameter
   {
      private Unit _unitToSet;
      private bool _unitChangedEventRaised;

      protected override void Context()
      {
         sut = DomainHelperForSpecs.ConstantParameterWithValue(10);
         sut.PropertyChanged += (o, e) =>
                                   {
                                      if (e.PropertyName.Equals("DisplayUnit"))
                                         _unitChangedEventRaised = true;
                                   };
         _unitToSet = sut.Dimension.Units.Last();
      }

      protected override void Because()
      {
         sut.DisplayUnit = _unitToSet;
      }

      [Observation]
      public void should_raise_the_property_changed_event()
      {
         _unitChangedEventRaised.ShouldBeTrue();
      }
   }
}