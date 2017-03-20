using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_AgeDependentMetaDataExtensions : StaticContextSpecification
   {
      private List<ParameterDistributionMetaData> _allMetaData;
      protected List<ParameterDistributionMetaData> _result;
      protected OriginData _originData = new OriginData();

      protected override void Context()
      {
         _allMetaData = new List<ParameterDistributionMetaData>();
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 0, GestationalAge = 25});
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 0, GestationalAge = 40});
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 0.5, GestationalAge = 25});
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 0.5, GestationalAge = 26 });
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 0.5, GestationalAge = 40 });
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 1, GestationalAge = 25});
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 1, GestationalAge = 26});
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 1, GestationalAge = 40});
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 3, GestationalAge = 40});
         _allMetaData.Add(new ParameterDistributionMetaData {Age = 4, GestationalAge = 40});
      }

      protected override void Because()
      {
         _result = _allMetaData.DefinedFor(_originData).ToList();
      }

      protected void VerifyValues(IEnumerable<ParameterDistributionMetaData> allMetaDatas, double age, double ga)
      {
         var metaData = allMetaDatas.Single(x => x.Age == age);
         metaData.Age.ShouldBeEqualTo(age);
         metaData.GestationalAge.ShouldBeEqualTo(ga);
      }
   }

   public class When_retrieving_all_meta_data_defined_for_a_given_origin_data_with_a_default_GA : concern_for_AgeDependentMetaDataExtensions
   {
      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS;
      }

      [Observation]
      public void should_return_all_meta_data_defined_for_an_age_the_GA()
      {
         _result.Count.ShouldBeEqualTo(5);
      }
   }

   public class When_retrieving_all_meta_data_defined_for_a_given_origin_data_with_a_specific_GA_for_which_data_exists : concern_for_AgeDependentMetaDataExtensions
   {
      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = 26;
      }

      [Observation]
      public void should_return_all_meta_data_defined_for_that_GA_as_well_as_the_data_for_an_age_strict_bigger_than_two_for_any_ga_()
      {
         _result.Count.ShouldBeEqualTo(5);
      }
   }

   public class When_retrieving_all_meta_data_defined_for_a_given_origin_data_with_a_specific_GA_for_which_data_not_exists : concern_for_AgeDependentMetaDataExtensions
   {
      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = 30;
      }

      [Observation]
      public void should_return_all_meta_data_defined_for_the_default_GA()
      {
         _result.Count.ShouldBeEqualTo(5);
      }
   }

   public class When_retrieving_all_meta_data_defined_for_a_given_origin_data_with_a_specific_GA_for_which_only_some_data_exists : concern_for_AgeDependentMetaDataExtensions
   {
      protected override void Context()
      {
         base.Context();
         _originData.GestationalAge = 26;
      }

      [Observation]
      public void should_return_all_meta_data_defined_for_the_default_GA()
      {
         _result.Count.ShouldBeEqualTo(5);
         VerifyValues(_result, 0, 40);
         VerifyValues(_result, 0.5, 26);
         VerifyValues(_result, 1, 26);
         VerifyValues(_result, 3, 40);
         VerifyValues(_result, 4, 40);
      }
   }
}