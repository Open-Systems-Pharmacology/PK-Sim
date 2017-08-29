using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Matlab.Mappers;

namespace PKSim.Matlab
{
   internal abstract class concern_for_MatlabOriginDataToOriginDataMapper : ContextSpecification<IMatlabOriginDataToOriginDataMapper>
   {
      protected Species _human;
      protected SpeciesPopulation _icrp;
      protected Gender _male;
      private CalculationMethodCategory _category;
      protected CalculationMethod _cmForHuman;
      protected OriginData _matlabOriginData;
      protected SpeciesPopulation _anotherPop;
      protected Gender _female;
      protected OriginDataMapper _originDataMapper;
      private IDimensionRepository _dimensionRepository;
      protected IMatlabParameterToSnapshotParameterMapper _parameterMapper;

      protected override void Context()
      {
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
         _originDataMapper = A.Fake<OriginDataMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _parameterMapper = A.Fake<IMatlabParameterToSnapshotParameterMapper>();

         _matlabOriginData = new OriginData();
         sut = new MatlabOriginDataToOriginDataMapper(_originDataMapper, _dimensionRepository, _parameterMapper);
      }
   }

   internal class mapping_a_well_defined_matlab_origin_data_to_origin_data_from_matlab_to_a_core_matlab_origin_data_to_origin_data : concern_for_MatlabOriginDataToOriginDataMapper
   {
      private Core.Model.OriginData _result;
      private Core.Snapshots.OriginData _snapshotOriginData;
      private Core.Model.OriginData _originData;

      protected override void Context()
      {
         base.Context();
         _matlabOriginData.Species = _human.Name;
         _matlabOriginData.Population = _icrp.Name;
         _matlabOriginData.Gender = _male.Name;
         _matlabOriginData.Age = 20;
         _matlabOriginData.Height = 17.6;
         _matlabOriginData.Weight = 75;
         _originData = new Core.Model.OriginData();

         A.CallTo(() => _originDataMapper.MapToModel(A<Core.Snapshots.OriginData>._)).Invokes(x =>
            {
               _snapshotOriginData = x.GetArgument<Core.Snapshots.OriginData>(0);
            })
            .Returns(_originData);

         A.CallTo(() => _parameterMapper.MapFrom(A<double>._, A<IDimension>._)).ReturnsLazily(x => new Core.Snapshots.Parameter
         {
            Value = x.GetArgument<double>(0)
         });

      }

      protected override void Because()
      {
         _result = sut.MapFrom(_matlabOriginData);
      }

      [Observation]
      public void should_return_the_origin_data_map_from_snapshot()
      {
         _result.ShouldBeEqualTo(_originData);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_species()
      {
         _snapshotOriginData.Species.ShouldBeEqualTo(_human.Name);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_population()
      {
         _snapshotOriginData.Population.ShouldBeEqualTo(_icrp.Name);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_gender()
      {
         _snapshotOriginData.Gender.ShouldBeEqualTo(_male.Name);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_age()
      {
         _snapshotOriginData.Age.Value.ShouldBeEqualTo(_matlabOriginData.Age);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_height()
      {
         _snapshotOriginData.Height.Value.ShouldBeEqualTo(_matlabOriginData.Height);
      }

      [Observation]
      public void should_return_an_origin_data_setup_with_the_corresponding_weight()
      {
         _snapshotOriginData.Weight.Value.ShouldBeEqualTo(_matlabOriginData.Weight);
      }

      [Observation]
      public void should_return_an_origin_data_containing_the_default_calculation_methods()
      {
         foreach (var cateogry in _matlabOriginData.AllCalculationMethods.Keys)
         {
            _snapshotOriginData.CalculationMethodFor(cateogry).ShouldBeEqualTo(_matlabOriginData.CalculationMethodFor(cateogry));
         }
      }
   }
}

  