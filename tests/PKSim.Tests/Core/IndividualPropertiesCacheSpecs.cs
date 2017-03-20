using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualPropertiesCache : ContextSpecification<IndividualPropertiesCache>
   {
      protected override void Context()
      {
         sut = new IndividualPropertiesCache();
      }
   }

   public class When_adding_an_individual_properties_to_the_cache : concern_for_IndividualPropertiesCache
   {
      private IndividualProperties _individualProperties;

      protected override void Context()
      {
         base.Context();
         _individualProperties = new IndividualProperties {Covariates = new IndividualCovariates {Gender = new Gender(), Race = new SpeciesPopulation()}};
         _individualProperties.AddParameterValue(new ParameterValue("PATH1", 5, 0.5));
         _individualProperties.AddParameterValue(new ParameterValue("PATH2", 10, 0.8));
      }

      protected override void Because()
      {
         sut.Add(_individualProperties);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_gender_and_seeds_of_the_individual_properties()
      {
         sut.Genders.ShouldContain(_individualProperties.Covariates.Gender);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_population_of_the_individual_properties()
      {
         sut.Races.ShouldContain(_individualProperties.Covariates.Race);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_parameter_values()
      {
         _individualProperties.ParameterValue("PATH1").Value.ShouldBeEqualTo(5);
         _individualProperties.ParameterValue("PATH2").Value.ShouldBeEqualTo(10);
      }
   }

   public class When_merging_an_individual_properties_cache_with_another_one  : concern_for_IndividualPropertiesCache
   {
      private IndividualPropertiesCache _individualPropertiesCacheToMerge;
      private ParameterValuesCache _originalValueCache;
      private ParameterValuesCache _cacheToMerge;
      private List<IndividualCovariates> _originalCovariates;
      private List<IndividualCovariates> _covariatesToMerge;
      private IndividualCovariates _cov1;
      private IndividualCovariates _cov2;
      private IndividualCovariates _cov3;
      private IndividualCovariates _cov4;
      private PathCache<IParameter> _parameterCache;

      protected override void Context()
      {
         base.Context();
         _originalValueCache = A.Fake<ParameterValuesCache>();
         _parameterCache = A.Fake<PathCache<IParameter>>();
         _cov1 = new IndividualCovariates();
         _cov2 = new IndividualCovariates();
         _cov3 = new IndividualCovariates();
         _cov4 = new IndividualCovariates();
         _cacheToMerge = A.Fake<ParameterValuesCache>();
         _originalCovariates =new List<IndividualCovariates>{_cov1,_cov2};
         _covariatesToMerge = new List<IndividualCovariates> { _cov3, _cov4 };
         sut= new IndividualPropertiesCache(_originalValueCache,_originalCovariates);
         _individualPropertiesCacheToMerge = new IndividualPropertiesCache(_cacheToMerge,_covariatesToMerge);
      }
      protected override void Because()
      {
         sut.Merge(_individualPropertiesCacheToMerge,_parameterCache);
      }

      [Observation]
      public void should_add_the_covariates_from_the_properties_cache()
      {
         sut.AllCovariates.ShouldOnlyContain(_cov1,_cov2,_cov3,_cov4);
      }

      [Observation]
      public void should_merge_the_parameter_values()
      {
         A.CallTo(() => _originalValueCache.Merge(_cacheToMerge, _parameterCache)).MustHaveHappened();
      }
   }
}