using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Assets;
using PKSim.Core.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterListOfValuesRetriever : ContextSpecification<IParameterListOfValuesRetriever>
   {
      protected IParameter _parameter;
      protected readonly HashSet<string> _listOfParameterWithValues = new HashSet<string>();

      protected override void Context()
      {
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue();
         sut = new ParameterListOfValuesRetriever(_listOfParameterWithValues);
      }
   }

   public class When_retrieving_the_list_of_values_for_a_parameter_defined_as_categorial_but_the_mapping_is_not_defined : concern_for_ParameterListOfValuesRetriever
   {
      protected override void Context()
      {
         base.Context();
         _parameter.Name = "TOTO";
         _listOfParameterWithValues.Add(_parameter.Name);
      }

      [Observation]
      public void should_throw_an_argument_exception()
      {
         The.Action(() => sut.ListOfValuesFor(_parameter)).ShouldThrowAn<ArgumentException>();
      }
   }

   public class When_retrieving_the_list_of_values_for_a_parameter_defined_with_a_min_and_max : concern_for_ParameterListOfValuesRetriever
   {
      private ICache<double, string> _listOfValues;

      protected override void Context()
      {
         base.Context();
         _parameter.Name = CoreConstants.Parameters.NUMBER_OF_BINS;
         _listOfParameterWithValues.Add(_parameter.Name);
      }

      protected override void Because()
      {
         _listOfValues = sut.ListOfValuesFor(_parameter);
      }

      [Observation]
      public void should_return_the_expected_value_between_min_and_max()
      {
         _listOfValues.Count.ShouldBeEqualTo(CoreConstants.Parameters.MAX_NUMBER_OF_BINS);
      }
   }

   public class When_retrieving_the_list_of_values_for_a_parameter_defined_as_boolean : concern_for_ParameterListOfValuesRetriever
   {
      private ICache<double, string> _listOfValues;

      protected override void Context()
      {
         base.Context();
         _parameter.Name = Constants.Parameters.USE_JACOBIAN;
         _listOfParameterWithValues.Add(_parameter.Name);
      }

      protected override void Because()
      {
         _listOfValues = sut.ListOfValuesFor(_parameter);
      }

      [Observation]
      public void should_return_the_expected_boolean_value()
      {
         _listOfValues.Count.ShouldBeEqualTo(2);
         _listOfValues[0].ShouldBeEqualTo(PKSimConstants.UI.No);
         _listOfValues[1].ShouldBeEqualTo(PKSimConstants.UI.Yes);
      }
   }
}