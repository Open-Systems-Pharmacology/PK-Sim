using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OriginData = PKSim.Core.Batch.OriginData;

namespace PKSim.Matlab
{
   internal abstract class concern_for_OriginDataMapper : ContextSpecification<IOriginDataMapper>
   {
      private ISpeciesRepository _speciesRepository;
      protected Species _human;
      protected SpeciesPopulation _icrp;
      protected Gender _male;
      private IOriginDataTask _originDataTask;
      private CalculationMethodCategory _category;
      protected CalculationMethod _cmForHuman;
      protected OriginData _batchOriginData;
      protected SpeciesPopulation _anotherPop;
      protected Gender _female;
      private IIndividualModelTask _individualModelTask;

      protected override void Context()
      {
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _originDataTask = A.Fake<IOriginDataTask>();
         _individualModelTask= A.Fake<IIndividualModelTask>();
         _male = new Gender().WithName(CoreConstants.Gender.Male);
         _female = new Gender().WithName(CoreConstants.Gender.Female);
         _human = new Species().WithName(CoreConstants.Species.Human);
         _icrp = new SpeciesPopulation {IsHeightDependent = true, IsAgeDependent = true}.WithName(CoreConstants.Population.ICRP);
         _anotherPop = new SpeciesPopulation().WithName("Another Pop");
         _cmForHuman = new CalculationMethod();
         _cmForHuman.AddSpecies(_human.Name);
         _icrp.AddGender(_male);
         _human.AddPopulation(_icrp);
         _category = new CalculationMethodCategory();
         _category.Add(_cmForHuman);
         A.CallTo(() => _speciesRepository.All()).Returns(new[] {_human});
         A.CallTo(() => _originDataTask.AllCalculationMethodCategoryFor(_human)).Returns(new[] {_category});
         _batchOriginData = new OriginData();
         sut = new OriginDataMapper(_speciesRepository, _originDataTask,_individualModelTask);
      }
   }

   internal class When_mapping_a_well_defined_origin_data_from_matlab_to_a_core_origin_data : concern_for_OriginDataMapper
   {
      private Core.Model.OriginData _result;

      protected override void Context()
      {
         base.Context();
         _batchOriginData.Species = _human.Name;
         _batchOriginData.Population = _icrp.Name;
         _batchOriginData.Gender = _male.Name;
         _batchOriginData.Age = 20;
         _batchOriginData.Height = 17.6;
         _batchOriginData.Weight = 75;
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_batchOriginData);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_species()
      {
         _result.Species.ShouldBeEqualTo(_human);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_population()
      {
         _result.SpeciesPopulation.ShouldBeEqualTo(_icrp);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_gender()
      {
         _result.Gender.ShouldBeEqualTo(_male);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_age()
      {
         _result.Age.ShouldBeEqualTo(_batchOriginData.Age);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_height()
      {
         _result.Height.ShouldBeEqualTo(_batchOriginData.Height); //cm to dm
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_weight()
      {
         _result.Weight.ShouldBeEqualTo(_batchOriginData.Weight);
      }

      [Observation]
      public void should_return_an_origin_data_containing_the_default_calculation_methods()
      {
         _result.AllCalculationMethods().ShouldContain(_cmForHuman);
      }
   }

   internal class When_mapping_an_origin_data_from_matlab_where_the_species_is_not_defined : concern_for_OriginDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _batchOriginData.Species = "toto";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapFrom(_batchOriginData)).ShouldThrowAn<PKSimException>();
      }
   }

   internal class When_mapping_an_origin_data_from_matlab_where_the_species_is_empty_and_the_species_only_has_one_pop : concern_for_OriginDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _batchOriginData.Species = _human.Name;
         _batchOriginData.Population = "";
      }

      [Observation]
      public void should_use_the_default_population()
      {
         sut.MapFrom(_batchOriginData).SpeciesPopulation.ShouldBeEqualTo(_icrp);
      }
   }

   internal class When_mapping_an_origin_data_from_matlab_where_the_species_is_empty_and_the_species_only_has_more_than_one_pop : concern_for_OriginDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _human.AddPopulation(_anotherPop);
         _batchOriginData.Species = _human.Name;
         _batchOriginData.Population = "";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapFrom(_batchOriginData)).ShouldThrowAn<PKSimException>();
      }
   }

   internal class When_mapping_an_origin_data_from_matlab_where_the_population_is_not_defined : concern_for_OriginDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _batchOriginData.Species = _human.Name;
         _batchOriginData.Population = "toto";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapFrom(_batchOriginData)).ShouldThrowAn<PKSimException>();
      }
   }

   internal class When_mapping_an_origin_data_from_matlab_where_the_gender_is_not_defined : concern_for_OriginDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _batchOriginData.Species = _human.Name;
         _batchOriginData.Population = _icrp.Name;
         _batchOriginData.Gender = "toto";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapFrom(_batchOriginData)).ShouldThrowAn<PKSimException>();
      }
   }

   internal class When_mapping_an_origin_data_from_matlab_where_the_gender_is_empty_and_the_population_has_only_one_gender : concern_for_OriginDataMapper
   {
      protected override void Context()
      {
         base.Context();
         _batchOriginData.Species = _human.Name;
         _batchOriginData.Population = _icrp.Name;
      }

      [Observation]
      public void should_use_the_default_gender()
      {
         sut.MapFrom(_batchOriginData).Gender.ShouldBeEqualTo(_male);
      }
   }
}