using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Infrastructure.Reporting.Markdown.Elements;
using PKSim.Infrastructure.Reporting.Markdown.Mappers;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ParameterToParameterElementMapper : ContextSpecification<IParameterToParameterElementMapper>
   {
      private IParameterListOfValuesRetriever _listOfValuesRetriever;
      protected IParameter _parameter;
      protected IParameter _booleanParameter;

      protected override void Context()
      {
         _listOfValuesRetriever = new ParameterListOfValuesRetriever();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(60).WithName("Param");
         _parameter.Dimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _parameter.DisplayUnit = _parameter.Dimension.Unit("h");
         _parameter.ValueOrigin.UpdateFrom(new ValueOrigin {Method = ValueOriginDeterminationMethods.Assumption, Source = ValueOriginSources.Other});
         _booleanParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameters.HAS_HALOGENS);
         sut = new ParameterToParameterElementMapper(_listOfValuesRetriever);
      }
   }

   public class When_mapping_a_parameter_to_a_parameter_element : concern_for_ParameterToParameterElementMapper
   {
      private ParameterElement _parameterElement;

      protected override void Because()
      {
         _parameterElement = sut.MapFrom(_parameter);
      }

      [Observation]
      public void should_return_the_parameter_with_its_value_in_display_unit()
      {
         _parameterElement.Value.ShouldBeEqualTo("1 h");
      }

      [Observation]
      public void should_map_the_value_origin_and_name_of_the_original_parameter()
      {
         _parameterElement.Name.ShouldBeEqualTo(_parameter.Name);
         _parameterElement.ValueOrigin.ShouldBeEqualTo(_parameter.ValueOrigin);
      }
   }

   public class When_mapping_a_categorial_parameter_to_a_parameter_element : concern_for_ParameterToParameterElementMapper
   {
      private ParameterElement _parameterElement;

      protected override void Because()
      {
         _parameterElement = sut.MapFrom(_booleanParameter);
      }

      [Observation]
      public void should_return_the_parameter_with_the_categorial_value()
      {
         _parameterElement.Value.ShouldBeEqualTo(PKSimConstants.UI.Yes);
      }
   }

   public class When_mapping_a_parameter_to_a_derived_element : concern_for_ParameterToParameterElementMapper
   {
      private MyParameterElement _parameterElement;

      protected override void Because()
      {
         _parameterElement = sut.MapFrom<MyParameterElement>(_parameter, x=>x.MyDescription = "TOTO");
      }

      [Observation]
      public void should_have_mapped_the_extended_properties()
      {
         _parameterElement.MyDescription.ShouldBeEqualTo("TOTO");
      }

      private class MyParameterElement : ParameterElement
      {
         public string MyDescription { get; set; }
      }
   }


}