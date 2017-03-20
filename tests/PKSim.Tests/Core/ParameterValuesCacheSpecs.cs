using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterValuesCache : ContextSpecification<ParameterValuesCache>
   {
      protected override void Context()
      {
         sut = new ParameterValuesCache();
      }
   }

   public class When_adding_a_parameter_value_to_a_parameter_value_cache : concern_for_ParameterValuesCache
   {
      private ParameterValue _parameterValue1;
      private ParameterValue _parameterValue2;

      protected override void Context()
      {
         base.Context();
         _parameterValue1 = new ParameterValue("PATH1", 11, 0.1);
         _parameterValue2 = new ParameterValue("PATH2", 21, 0.2);
      }

      protected override void Because()
      {
         sut.Add(new[] {_parameterValue1, _parameterValue2});
         sut.Add(new[] {new ParameterValue(_parameterValue1.ParameterPath, 12, 0.3), new ParameterValue(_parameterValue2.ParameterPath, 22, 0.5)});
      }

      [Observation]
      public void should_store_the_value_for_the_given_parameter_path_and_be_able_to_retrieve_the_parameter_paths_afterwards()
      {
         sut.AllParameterPaths().ShouldOnlyContainInOrder(_parameterValue1.ParameterPath, _parameterValue2.ParameterPath);
      }

      [Observation]
      public void should_return_true_if_asked_if_it_contains_some_values_for_the_added_path()
      {
         sut.Has(_parameterValue1.ParameterPath).ShouldBeTrue();
         sut.Has(_parameterValue2.ParameterPath).ShouldBeTrue();
      }

      [Observation]
      public void should_be_able_to_retrieve_the_values_for_the_individual_path()
      {
         sut.ValuesFor(_parameterValue1.ParameterPath).ShouldOnlyContainInOrder(11, 12);
      }
   }

   public class When_removing_a_parameter_by_path_from_the_parameter_cache : concern_for_ParameterValuesCache
   {
      protected override void Context()
      {
         base.Context();
         var parameterValue1 = new ParameterValue("PATH1", 11, 0.1);
         sut.Add(new[] {parameterValue1});
         sut.Add(new[] {parameterValue1});
      }

      protected override void Because()
      {
         sut.Remove("PATH1");
      }

      [Observation]
      public void should_not_contain_any_values_for_the_given_path_anymore()
      {
         sut.Has("PATH1").ShouldBeFalse();
      }

      [Observation]
      public void should_have_delted_all_rows_if_resulting_cache_as_no_parameter()
      {
         sut.AllParameterValues.Count().ShouldBeEqualTo(0);
      }
   }

   public class When_cloning_a_parameter_value_cache : concern_for_ParameterValuesCache
   {
      private ParameterValue _parameterValue1;
      private ParameterValue _parameterValue2;
      private ParameterValuesCache _result;

      protected override void Context()
      {
         base.Context();
         _parameterValue1 = new ParameterValue("PATH1", 11, 0.1);
         _parameterValue2 = new ParameterValue("PATH2", 21, 0.2);
         sut.Add(new[] {_parameterValue1, _parameterValue2});
         sut.Add(new[] {new ParameterValue(_parameterValue1.ParameterPath, 12, 0.1), new ParameterValue(_parameterValue2.ParameterPath, 22, 0.2)});
      }

      protected override void Because()
      {
         _result = sut.Clone();
      }

      [Observation]
      public void should_return_a_new_cache_containing_the_same_values_as_the_original_cache()
      {
         _result.Has("PATH1").ShouldBeTrue();
         _result.Has("PATH2").ShouldBeTrue();
         _result.ValuesFor("PATH1").ShouldOnlyContain(11, 12);
         _result.ValuesFor("PATH2").ShouldOnlyContain(21, 22);
      }
   }

   public class When_renaming_a_parameter_path_that_does_not_exist : concern_for_ParameterValuesCache
   {
      protected override void Context()
      {
         base.Context();
         sut.Add(new[] {new ParameterValue("PATH1", 11, 0.1)});
      }

      protected override void Because()
      {
         sut.RenamePath("PATH2", "PATH3");
      }

      [Observation]
      public void should_do_nothing()
      {
         sut.Has("PATH1").ShouldBeTrue();
      }
   }

   public class When_renaming_a_parameter_path_that_does_exist : concern_for_ParameterValuesCache
   {
      protected override void Context()
      {
         base.Context();
         sut.Add(new[] {new ParameterValue("PATH1", 11, 0.1)});
      }

      protected override void Because()
      {
         sut.RenamePath("PATH1", "PATH2");
      }

      [Observation]
      public void should_do_nothing()
      {
         sut.Has("PATH2").ShouldBeTrue();
      }

      [Observation]
      public void should_have_renamed_the_path_and_keep_the_values()
      {
         sut.Has("PATH2").ShouldBeTrue();
         sut.ValuesFor("PATH2").ShouldOnlyContain(11);
      }

      [Observation]
      public void should_have_removed_the_old_path()
      {
         sut.Has("PATH1").ShouldBeFalse();
      }
   }

   public class When_merging_two_caches : concern_for_ParameterValuesCache
   {
      private ParameterValuesCache _cacheToMerge;
      private PathCache<IParameter> _parameterCache;
      private IParameter _parameter1;
      private IParameter _parameter2;
      private IParameter _parameter3;

      protected override void Context()
      {
         base.Context();
         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(1);
         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(2);
         _parameter3 = DomainHelperForSpecs.ConstantParameterWithValue(3);
         _parameterCache = A.Fake<PathCache<IParameter>>();
         A.CallTo(() => _parameterCache["PATH1"]).Returns(_parameter1);
         A.CallTo(() => _parameterCache["PATH2"]).Returns(_parameter2);
         A.CallTo(() => _parameterCache["PATH3"]).Returns(_parameter3);
         sut.SetValues("PATH1", new double[] {1, 2, 3});
         sut.SetValues("PATH2", new double[] {4, 5, 6});

         _cacheToMerge = new ParameterValuesCache();
         _cacheToMerge.SetValues("PATH1", new double[] {7, 8});
         _cacheToMerge.SetValues("PATH3", new double[] {9, 10});
      }

      protected override void Because()
      {
         sut.Merge(_cacheToMerge, _parameterCache);
      }

      [Observation]
      public void should_simply_expand_the_existing_values_with_the_new_ones()
      {
         sut.ValuesFor("PATH1").ShouldOnlyContain(1, 2, 3, 7, 8);
      }

      [Observation]
      public void should_fill_up_the_missing_values_in_the_with_nan_for_the_existing_keys()
      {
         sut.ValuesFor("PATH2").ShouldOnlyContain(4, 5, 6, 2, 2);
      }

      [Observation]
      public void should_fill_up_the_missing_values_in_the_with_nan_for_the_new_keys()
      {
         sut.ValuesFor("PATH3").ShouldOnlyContain(3, 3, 3, 9, 10);
      }
   }
}