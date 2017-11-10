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
      private ParameterMapper _parameterMapper;
      private IOriginDataTask _originDataTask;
      protected IDimensionRepository _dimensionRepository;
      protected IIndividualModelTask _individualModelTask;
      private ISpeciesRepository _speciesRepository;
      protected OriginData _snapshot;
      protected Model.OriginData _originData;
      protected Parameter _ageSnapshotParameter;
      protected Parameter _heightSnapshotParameter;
      protected Parameter _weightSnapshotParameter;
      protected Species _species;
      private SpeciesPopulation _speciesPopulation;
      private Gender _gender;

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

         _species = new Species {Name = "Human"};
         _speciesPopulation = new SpeciesPopulation {Name = "SpeciesPopulation"};
         _gender = new Gender {Name = "Unknown"};
         _species.AddPopulation(_speciesPopulation);
         _speciesPopulation.AddGender(_gender);
         
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
         };

         A.CallTo(() => _parameterMapper.ParameterFrom(null, A<string>._, A<IDimension>._)).Returns(null);
         A.CallTo(() => _parameterMapper.ParameterFrom(_originData.Age, A<string>._, A<IDimension>._)).Returns(_ageSnapshotParameter);
         A.CallTo(() => _parameterMapper.ParameterFrom(_originData.Height, A<string>._, A<IDimension>._)).Returns(_heightSnapshotParameter);
         A.CallTo(() => _parameterMapper.ParameterFrom(_originData.Weight, A<string>._, A<IDimension>._)).Returns(_weightSnapshotParameter);

         return _completed;
      }
   }

   public class When_mapping_an_origin_data_to_snapshot : concern_for_OriginDataMapper
   {
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

         //those parameters where not set in example
         _snapshot.GestationalAge.ShouldBeNull();
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
         _newOriginData.Age.ShouldBeEqualTo(_originData.Age);
         _newOriginData.Height.ShouldBeEqualTo(_originData.Height);
         _newOriginData.GestationalAge.ShouldBeEqualTo(_originData.GestationalAge);
      }
   }
}