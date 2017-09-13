using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using Individual = PKSim.Core.Snapshots.Individual;
using ParameterRange = PKSim.Core.Model.ParameterRange;
using Population = PKSim.Core.Snapshots.Population;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationMapper : ContextSpecification<PopulationMapper>
   {
      protected Population _snapshot;
      protected IndividualMapper _individualMapper;
      protected ParameterRangeMapper _parameterRangeMapper;
      protected IGenderRepository _genderRepository;
      protected Model.Individual _baseIndividual;
      protected ParameterRange _ageParameterRange;
      protected ParameterRange _weightParameterRange;
      protected Snapshots.ParameterRange _ageRangeSnapshot;
      protected Snapshots.ParameterRange _weightRangeSnapshot;
      protected Individual _snapshotIndividual;
      protected AdvancedParameterMapper _advancedParameterMapper;
      private AdvancedParameterCollection _advancedParameters;
      private AdvancedParameter _advancedParameter;
      protected Snapshots.AdvancedParameter _advancedParameterSnapshot;
      protected PopulationFile _populationFile1;
      protected PopulationFile _populationFile2;

      protected override void Context()
      {
         _individualMapper = A.Fake<IndividualMapper>();
         _parameterRangeMapper = A.Fake<ParameterRangeMapper>();
         _advancedParameterMapper = A.Fake<AdvancedParameterMapper>();
         _genderRepository = A.Fake<IGenderRepository>();

         sut = new PopulationMapper(_individualMapper, _parameterRangeMapper, _advancedParameterMapper, _genderRepository);

         _baseIndividual = new Model.Individual();

         _advancedParameters = new AdvancedParameterCollection();
         _advancedParameter = new AdvancedParameter
         {
            DistributedParameter = DomainHelperForSpecs.NormalDistributedParameter()
         };

         _advancedParameters.AddAdvancedParameter(_advancedParameter);


         _ageParameterRange = new ConstrainedParameterRange {ParameterName = CoreConstants.Parameter.AGE};
         _weightParameterRange = new ParameterRange {ParameterName = CoreConstants.Parameter.MEAN_WEIGHT};

         A.CallTo(() => _parameterRangeMapper.MapToSnapshot(null)).Returns(null);
         _ageRangeSnapshot = new Snapshots.ParameterRange();
         A.CallTo(() => _parameterRangeMapper.MapToSnapshot(_ageParameterRange)).Returns(_ageRangeSnapshot);

         _weightRangeSnapshot = new Snapshots.ParameterRange();
         A.CallTo(() => _parameterRangeMapper.MapToSnapshot(_weightParameterRange)).Returns(_weightRangeSnapshot);

         _snapshotIndividual = new Individual();
         A.CallTo(() => _individualMapper.MapToSnapshot(_baseIndividual)).Returns(_snapshotIndividual);

         _advancedParameterSnapshot = new Snapshots.AdvancedParameter();
         A.CallTo(() => _advancedParameterMapper.MapToSnapshot(_advancedParameter)).Returns(_advancedParameterSnapshot);

         _populationFile1 = new PopulationFile
         {
            FilePath = "Path1",
            NumberOfIndividuals = 5
         };
         _populationFile2 = new PopulationFile
         {
            FilePath = "Path2",
            NumberOfIndividuals = 10
         };
      }

      protected RandomPopulation CreateRandomPopulation()
      {
         var randomPopulation = new RandomPopulation
         {
            Name = "RandomPop",
            Description = "Random Pop description",
            Seed = 132,
            Settings = new RandomPopulationSettings
            {
               NumberOfIndividuals = 10,
               BaseIndividual = _baseIndividual
            },
         };


         randomPopulation.SetAdvancedParameters(_advancedParameters);
         randomPopulation.Settings.AddParameterRange(_weightParameterRange);
         randomPopulation.Settings.AddParameterRange(_ageParameterRange);
         return randomPopulation;
      }
   }

   public class When_mapping_a_random_population_to_snapshot : concern_for_PopulationMapper
   {
      protected RandomPopulation _population;

      protected override void Context()
      {
         base.Context();
         _population = CreateRandomPopulation();
      }

      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_population);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_population.Name);
         _snapshot.Description.ShouldBeEqualTo(_population.Description);
      }

      [Observation]
      public void should_save_the_seed_that_was_used_to_generate_the_population()
      {
         _snapshot.Seed.ShouldBeEqualTo(_population.Seed);
      }

      [Observation]
      public void should_save_the_base_individual()
      {
         _snapshot.Individual.ShouldBeEqualTo(_snapshotIndividual);
      }

      [Observation]
      public void should_save_all_advanced_parmaeters_to_snapshot()
      {
         _snapshot.AdvancedParameters.ShouldContain(_advancedParameterSnapshot);
      }

      [Observation]
      public void should_save_the_available_ranges()
      {
         _snapshot.Age.ShouldBeEqualTo(_ageRangeSnapshot);
         _snapshot.Weight.ShouldBeEqualTo(_weightRangeSnapshot);
      }

      [Observation]
      public void should_set_all_other_ranges_to_null()
      {
         _snapshot.Height.ShouldBeNull();
         _snapshot.GestationalAge.ShouldBeNull();
         _snapshot.BMI.ShouldBeNull();
      }
   }

   public class When_mapping_an_imported_population : concern_for_PopulationMapper
   {
      protected ImportPopulation _population;

      protected override void Context()
      {
         base.Context();
         _population = new ImportPopulation();
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapToSnapshot(_population)).ShouldThrowAn<OSPSuiteException>();
      }
   }

   public class When_mapping_a_mobi_population : concern_for_PopulationMapper
   {
      protected MoBiPopulation _population;

      protected override void Context()
      {
         base.Context();
         _population = new MoBiPopulation();
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapToSnapshot(_population)).ShouldThrowAn<OSPSuiteException>();
      }
   }
}