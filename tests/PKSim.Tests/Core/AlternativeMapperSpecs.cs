using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_AlternativeMapper : ContextSpecificationAsync<AlternativeMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected IParameterAlternativeFactory _parameterAlternativeFactory;
      protected ISpeciesRepository _speciesRepository;
      protected ParameterAlternative _alternative;
      protected ParameterAlternativeGroup _parameterGroup;
      protected Alternative _snapshot;
      protected ParameterAlternativeWithSpecies _alternativeWithSpecies;
      protected Species _species;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _parameterAlternativeFactory = A.Fake<IParameterAlternativeFactory>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         sut = new AlternativeMapper(_parameterMapper, _parameterAlternativeFactory, _speciesRepository);

         _parameterGroup = new ParameterAlternativeGroup {Name = "ParameterGroup"};
         _alternative = new ParameterAlternative
         {
            Name = "Alternative1",
            IsDefault = true,
            Description = "Hello"
         };

         _species = new Species {Name = "Hello"};
         _alternativeWithSpecies = new ParameterAlternativeWithSpecies
         {
            Name = "Alternative2",
            IsDefault = false,
            Description = "Hello",
            Species = _species
         };

         _parameterGroup.AddAlternative(_alternativeWithSpecies);
         A.CallTo(() => _speciesRepository.All()).Returns(new[] {_species});

         return _completed;
      }
   }

   public class When_mapping_a_calculated_compound_alternative_to_snapshot : concern_for_AlternativeMapper
   {
      private ParameterAlternative _calculatedAlternative;

      protected override async Task Context()
      {
         await base.Context();
         _calculatedAlternative = A.Fake<ParameterAlternative>();
         A.CallTo(() => _calculatedAlternative.IsCalculated).Returns(true);
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_calculatedAlternative);
      }

      [Observation]
      public void should_return_null()
      {
         _snapshot.ShouldBeNull();
      }
   }

   public class When_mapping_a_compound_alternative_to_snapshot : concern_for_AlternativeMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_alternative);
      }

      [Observation]
      public void should_save_the_expected_properties()
      {
         _snapshot.IsDefault.ShouldBeNull();
         _snapshot.Name.ShouldBeEqualTo(_alternative.Name);
         _snapshot.Description.ShouldBeEqualTo(_alternative.Description);
      }
   }

   public class When_mapping_a_compound_alternative__with_species_to_snapshot : concern_for_AlternativeMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_alternativeWithSpecies);
      }

      [Observation]
      public void should_save_the_expected_properties()
      {
         _snapshot.IsDefault.ShouldBeEqualTo(_alternativeWithSpecies.IsDefault);
         _snapshot.Name.ShouldBeEqualTo(_alternativeWithSpecies.Name);
         _snapshot.Description.ShouldBeEqualTo(_alternativeWithSpecies.Description);
         _snapshot.Species.ShouldBeEqualTo(_alternativeWithSpecies.Species.Name);
      }
   }

   public class When_mapping_a_valid_snapshot_alternative_to_model : concern_for_AlternativeMapper
   {
      private ParameterAlternativeWithSpecies _newAlternativeWithSpecies;
      private Parameter _snapshotParameter;
      private IParameter _alternativeParameter;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_alternativeWithSpecies);
         var newAlternativeWithSpecies = new ParameterAlternativeWithSpecies();
         A.CallTo(() => _parameterAlternativeFactory.CreateAlternativeFor(_parameterGroup)).Returns(newAlternativeWithSpecies);
         _snapshotParameter = new Parameter {Name = "P1"};
         _alternativeParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(_snapshotParameter.Name);
         newAlternativeWithSpecies.Add(_alternativeParameter);
         _snapshot.Parameters = new []{_snapshotParameter};
      }

      protected override async Task Because()
      {
         _newAlternativeWithSpecies = await sut.MapToModel(_snapshot, _parameterGroup) as ParameterAlternativeWithSpecies;
      }

      [Observation]
      public void should_return_an_alternative_having_the_expected_properties()
      {
         _newAlternativeWithSpecies.Name.ShouldBeEqualTo(_snapshot.Name);
         _newAlternativeWithSpecies.Description.ShouldBeEqualTo(_snapshot.Description);
         _newAlternativeWithSpecies.IsDefault.ShouldBeEqualTo(_alternativeWithSpecies.IsDefault);
         _alternativeWithSpecies.Species.ShouldBeEqualTo(_species);
      }

      [Observation]
      public void should_have_updated_the_alternative_parameters()
      {
         A.CallTo(() => _parameterMapper.MapParameters(_snapshot.Parameters, _newAlternativeWithSpecies, _parameterGroup.Name)).MustHaveHappened();
      }
   }
}