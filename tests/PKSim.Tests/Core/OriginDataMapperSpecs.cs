using System.Threading.Tasks;
using FakeItEasy;
using NHibernate.Util;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using OriginData = PKSim.Core.Snapshots.OriginData;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_OriginDataMapper : ContextSpecificationAsync<OriginDataMapper>
   {
      protected ParameterMapper _parameterMapper;
      private IOriginDataTask _originDataTask;
      protected IDimensionRepository _dimensionRepository;
      protected IIndividualModelTask _individualModelTask;
      protected ISpeciesRepository _speciesRepository;
      protected OriginData _snapshot;
      protected Model.OriginData _originData;
      protected Parameter _ageSnapshotParameter;
      protected Parameter _heightSnapshotParameter;
      protected Parameter _weightSnapshotParameter;
      protected Parameter _gestationalAgeSnapshotParameter;
      protected Species _species;
      protected SpeciesPopulation _speciesPopulation;
      protected Gender _gender;
      protected SpeciesPopulation _anotherPopulation;
      protected Gender _anotherGender;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _originDataTask = A.Fake<IOriginDataTask>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _individualModelTask = A.Fake<IIndividualModelTask>();
         _speciesRepository = A.Fake<ISpeciesRepository>();

         sut = new OriginDataMapper(_parameterMapper, _originDataTask, _dimensionRepository, _individualModelTask, _speciesRepository);

         _ageSnapshotParameter = new Parameter {Value = 1};
         _heightSnapshotParameter = new Parameter {Value = 2};
         _weightSnapshotParameter = new Parameter {Value = 3};
         _gestationalAgeSnapshotParameter = new Parameter {Value = 4};

         _speciesPopulation = new SpeciesPopulation {Name = "SpeciesPopulation", IsHeightDependent = true, IsAgeDependent = true};
         _gender = new Gender {Name = "Unknown"};
         _species = new Species { Name = "Human" };
         _species.AddPopulation(_speciesPopulation);
         _anotherPopulation = new SpeciesPopulation { Name = "Another species population", IsHeightDependent = true, IsAgeDependent = true };

         _speciesPopulation.AddGender(_gender);
          _anotherGender = new Gender { Name = "AnotherGender" };
         
         A.CallTo(() => _speciesRepository.All()).Returns(new []{_species});

         _originData = new Model.OriginData
         {
            Age = 35,
            AgeUnit = "years",
            Height = 17.8,
            HeightUnit = "m",
            Weight = 73,
            WeightUnit = "kg",
            Species = _species,
            SpeciesPopulation = _speciesPopulation,
            Gender = _gender,
            GestationalAge = 40
         };

         A.CallTo(() => _parameterMapper.ParameterFrom(null, A<string>._, A<IDimension>._)).Returns(null);
         A.CallTo(() => _parameterMapper.ParameterFrom(_originData.Age, A<string>._, A<IDimension>._)).Returns(_ageSnapshotParameter);
         A.CallTo(() => _parameterMapper.ParameterFrom(_originData.Height, A<string>._, A<IDimension>._)).Returns(_heightSnapshotParameter);
         A.CallTo(() => _parameterMapper.ParameterFrom(_originData.Weight, A<string>._, A<IDimension>._)).Returns(_weightSnapshotParameter);
         A.CallTo(() => _parameterMapper.ParameterFrom(_originData.GestationalAge, A<string>._, A<IDimension>._)).Returns(_gestationalAgeSnapshotParameter);

         return _completed;
      }
   }

   public class When_mapping_an_origin_data_to_snapshot : concern_for_OriginDataMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _species.AddPopulation(_anotherPopulation);
         _speciesPopulation.AddGender(_anotherGender);
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_originData);
      }

      [Observation]
      public void should_save_the_origin_data_properties()
      {
         _snapshot.Species.ShouldBeEqualTo(_originData.Species.Name);
         _snapshot.Population.ShouldBeEqualTo(_originData.SpeciesPopulation.Name);
         _snapshot.Gender.ShouldBeEqualTo(_originData.Gender.Name);

         _snapshot.Age.ShouldBeEqualTo(_ageSnapshotParameter);
         _snapshot.Height.ShouldBeEqualTo(_heightSnapshotParameter);
         _snapshot.GestationalAge.ShouldBeEqualTo(_gestationalAgeSnapshotParameter);
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_where_the_species_is_not_defined : concern_for_OriginDataMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Species = "toto";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         TheAsync.Action(() => sut.MapToModel(_snapshot)).ShouldThrowAnAsync<PKSimException>();
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_where_the_species_is_empty_and_the_species_only_has_one_pop : concern_for_OriginDataMapper
   {
      private PKSim.Core.Model.OriginData _newOriginData;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Population = "";
      }

      protected override async Task Because()
      {
         _newOriginData = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_use_the_default_population()
      {
         _newOriginData.SpeciesPopulation.ShouldBeEqualTo(_speciesPopulation);
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_where_the_species_is_empty : concern_for_OriginDataMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Species = "";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         TheAsync.Action(() => sut.MapToModel(_snapshot)).ShouldThrowAnAsync<PKSimException>();
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_where_the_population_is_not_defined : concern_for_OriginDataMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Population = "toto";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         TheAsync.Action(() => sut.MapToModel(_snapshot)).ShouldThrowAnAsync<PKSimException>();
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_where_the_gender_is_not_defined : concern_for_OriginDataMapper
   {
      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Gender = "toto";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         TheAsync.Action(() => sut.MapToModel(_snapshot)).ShouldThrowAnAsync<PKSimException>();
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_where_the_gender_is_empty_and_the_population_has_only_one_gender : concern_for_OriginDataMapper
   {
      private Model.OriginData _newOriginData;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Gender = "";
      }

      protected override async Task Because()
      {
         _newOriginData = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_use_the_default_gender()
      {
         _newOriginData.Gender.ShouldBeEqualTo(_gender);
      }
   }


   public class When_mapping_snapshot_origin_data_to_origin_data : concern_for_OriginDataMapper
   {
      private Model.OriginData _newOriginData;

      protected override async Task Context()
      {
         await base.Context();

         _snapshot = await sut.MapToSnapshot(_originData);
     
         var meanWeightParameter = A.Fake<IParameter>();
         A.CallTo(() => _individualModelTask.MeanWeightFor(A<Model.OriginData>._)).Returns(meanWeightParameter);
         A.CallTo(() => meanWeightParameter.Dimension.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Weight.Value.Value)).Returns(_originData.Weight);

         var meanHeightParameter = A.Fake<IParameter>();
         A.CallTo(() => _individualModelTask.MeanHeightFor(A<Model.OriginData>._)).Returns(meanHeightParameter);
         A.CallTo(() => meanHeightParameter.Dimension.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Height.Value.Value)).Returns(_originData.Height.Value);

         var meanAgeParameter = A.Fake<IParameter>();
         A.CallTo(() => _individualModelTask.MeanAgeFor(A<Model.OriginData>._)).Returns(meanAgeParameter);
         A.CallTo(() => meanAgeParameter.Dimension.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Age.Value.Value)).Returns(_originData.Age.Value);

         var meanGestionalAgeParameter = A.Fake<IParameter>();
         A.CallTo(() => _individualModelTask.MeanGestationalAgeFor(A<Model.OriginData>._)).Returns(meanGestionalAgeParameter);
         A.CallTo(() => meanGestionalAgeParameter.Dimension.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.GestationalAge.Value.Value)).Returns(_originData.GestationalAge.Value);
      }
      
      protected override async Task Because()
      {
         _newOriginData = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_use_the_expected_individual_origin_data_to_create_the_individual()
      {
         _newOriginData.Species.ShouldBeEqualTo(_originData.Species);
         _newOriginData.SpeciesPopulation.ShouldBeEqualTo(_originData.SpeciesPopulation);
         _newOriginData.Gender.ShouldBeEqualTo(_originData.Gender);
         _newOriginData.Weight.ShouldBeEqualTo(_originData.Weight);
         _newOriginData.Height.ShouldBeEqualTo(_originData.Height);
         _newOriginData.Age.ShouldBeEqualTo(_originData.Age);
         _newOriginData.GestationalAge.ShouldBeEqualTo(_originData.GestationalAge);
      }
   }
}