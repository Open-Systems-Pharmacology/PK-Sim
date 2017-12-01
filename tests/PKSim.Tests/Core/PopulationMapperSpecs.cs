using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Exceptions;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using AdvancedParameter = PKSim.Core.Model.AdvancedParameter;
using Population = PKSim.Core.Snapshots.Population;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationMapper : ContextSpecificationAsync<PopulationMapper>
   {
      protected Population _snapshot;
      protected AdvancedParameterMapper _advancedParameterMapper;
      private AdvancedParameterCollection _advancedParameters;
      private AdvancedParameter _advancedParameter;
      protected Snapshots.AdvancedParameter _advancedParameterSnapshot;
      protected IRandomPopulationFactory _randomPopulationFactory;
      protected RandomPopulationSettingsMapper _randomPopulationSettingsMapper;
      protected PopulationSettings _settingsSnapshot;
      protected RandomPopulation _population;

      protected override Task Context()
      {
         _advancedParameterMapper = A.Fake<AdvancedParameterMapper>();
         _randomPopulationFactory = A.Fake<IRandomPopulationFactory>();
         _randomPopulationSettingsMapper = A.Fake<RandomPopulationSettingsMapper>();
         sut = new PopulationMapper(_advancedParameterMapper, _randomPopulationSettingsMapper, _randomPopulationFactory);

         _advancedParameters = new AdvancedParameterCollection();
         _advancedParameter = new AdvancedParameter
         {
            DistributedParameter = DomainHelperForSpecs.NormalDistributedParameter()
         };

         _advancedParameters.AddAdvancedParameter(_advancedParameter);
         _advancedParameterSnapshot = new Snapshots.AdvancedParameter();
         A.CallTo(() => _advancedParameterMapper.MapToSnapshot(_advancedParameter)).Returns(_advancedParameterSnapshot);


         _population = CreateRandomPopulation();

         _settingsSnapshot = new PopulationSettings();
         A.CallTo(() => _randomPopulationSettingsMapper.MapToSnapshot(_population.Settings)).Returns(_settingsSnapshot);

         return Task.FromResult(true);
      }

      protected RandomPopulation CreateRandomPopulation()
      {
         var randomPopulation = new RandomPopulation
         {
            Name = "RandomPop",
            Description = "Random Pop description",
            Seed = 132,
            Settings = new RandomPopulationSettings()
         };

         randomPopulation.SetAdvancedParameters(_advancedParameters);
         return randomPopulation;
      }
   }

   public class When_mapping_a_random_population_to_snapshot : concern_for_PopulationMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_population);
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
      public void should_save_all_advanced_parameters_to_snapshot()
      {
         _snapshot.AdvancedParameters.ShouldContain(_advancedParameterSnapshot);
      }

      [Observation]
      public void should_save_the_population_settings_into_the_snapshot()
      {
         _snapshot.Settings.ShouldBeEqualTo(_settingsSnapshot);
      }
   }

   public class When_mapping_a_valid_population_snapshot_to_a_population : concern_for_PopulationMapper
   {
      private RandomPopulation _newPopulation;
      private RandomPopulation _randomPopulation;
      private RandomPopulationSettings _newPopulationSettings;

      protected override async Task Context()
      {
         await base.Context();
         _randomPopulation = CreateRandomPopulation();
         _newPopulationSettings = new RandomPopulationSettings();
         _snapshot = await sut.MapToSnapshot(_randomPopulation);
         A.CallTo(() => _randomPopulationSettingsMapper.MapToModel(_snapshot.Settings)).Returns(_newPopulationSettings);
         var mappedPopulation = A.Fake<RandomPopulation>();
         mappedPopulation.SetAdvancedParameters(new AdvancedParameterCollection());
         A.CallTo(() => _randomPopulationFactory.CreateFor(_newPopulationSettings, CancellationToken.None, _snapshot.Seed, false)).Returns(mappedPopulation);
      }

      protected override async Task Because()
      {
         _newPopulation = await sut.MapToModel(_snapshot) as RandomPopulation;
      }

      [Observation]
      public void should_use_the_snapshot_seed_and_seettings_to_create_the_population()
      {
         A.CallTo(() => _randomPopulationFactory.CreateFor(_newPopulationSettings, CancellationToken.None, _snapshot.Seed,false)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_population_having_the_expected_properties()
      {
         _newPopulation.Name.ShouldBeEqualTo(_snapshot.Name);
         _newPopulation.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_map_all_advanced_parmaeters()
      {
         A.CallTo(() => _advancedParameterMapper.MapToModel(_snapshot.AdvancedParameters, _newPopulation)).MustHaveHappened();
      }
   }

   public class When_mapping_an_imported_population : concern_for_PopulationMapper
   {
      [Observation]
      public void should_throw_an_exception()
      {
         TheAsync.Action(() => sut.MapToSnapshot(new ImportPopulation())).ShouldThrowAnAsync<OSPSuiteException>();
      }
   }

   public class When_mapping_a_mobi_population : concern_for_PopulationMapper
   {
      [Observation]
      public void should_throw_an_exception()
      {
         TheAsync.Action(() => sut.MapToSnapshot(new MoBiPopulation())).ShouldThrowAnAsync<OSPSuiteException>();
      }
   }
}