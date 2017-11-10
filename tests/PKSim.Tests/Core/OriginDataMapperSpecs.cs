using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using OriginData = PKSim.Core.Model.OriginData;

namespace PKSim.Core
{
   public abstract class concern_for_OriginDataMapper : ContextSpecificationAsync<OriginDataMapper>
   {
      private ParameterMapper _parameterMapper;
      private IOriginDataTask _originDataTask;
      protected IDimensionRepository _dimensionRepository;
      private IIndividualModelTask _individualModelTask;
      private ISpeciesRepository _speciesRepository;
      protected PKSim.Core.Snapshots.OriginData _snapshot;
      protected OriginData _originData;
      protected Parameter _ageSnapshotParameter;
      protected Parameter _heightSnapshotParameter;
      protected Parameter _weightSnapshotParameter;

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

         _originData = new OriginData
         {
            Age = 35,
            AgeUnit = "years",
            Height = 17.8,
            HeightUnit = "m",
            Weight = 73,
            WeightUnit = "kg"
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
      private OriginData _newOriginData;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         A.CallTo(() => _dimensionRepository.Mass.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Weight.Value.Value)).Returns(10);
         A.CallTo(() => _dimensionRepository.AgeInYears.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Age.Value.Value)).Returns(20);
         A.CallTo(() => _dimensionRepository.Length.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Height.Value.Value)).Returns(30);
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
         _newOriginData.Weight.ShouldBeEqualTo(10);
         _newOriginData.Age.ShouldBeEqualTo(20);
         _newOriginData.Height.ShouldBeEqualTo(30);

         _newOriginData.GestationalAge.ShouldBeEqualTo(_originData.GestationalAge);
      }

   }
}