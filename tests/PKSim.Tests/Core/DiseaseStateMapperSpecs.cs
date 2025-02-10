using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_DiseaseStateMapper : ContextSpecificationAsync<DiseaseStateMapper>
   {
      protected IDiseaseStateRepository _diseaseStateRepository;
      protected ParameterMapper _parameterMapper;
      protected IDimensionRepository _dimensionRepository;
      protected OriginData _originData;
      protected OriginDataParameter _diseaseStateParameter;
      private IDimension _timeDimension;

      protected override Task Context()
      {
         _diseaseStateRepository = A.Fake<IDiseaseStateRepository>();
         _parameterMapper = A.Fake<ParameterMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();

         sut = new DiseaseStateMapper(_diseaseStateRepository, _parameterMapper, _dimensionRepository);
         var speciesPopulation = new SpeciesPopulation {Name = "SpeciesPopulation", IsHeightDependent = true, IsAgeDependent = true};
         var gender = new Gender {Name = "Unknown"};
         var species = new Species {Name = "Human"};
         species.AddPopulation(speciesPopulation);

         _originData = new OriginData
         {
            Age = new OriginDataParameter(35, "years"),
            Height = new OriginDataParameter(1.78, "m"),
            Weight = new OriginDataParameter(73, "kg"),
            Species = species,
            Population = speciesPopulation,
            Gender = gender,
            GestationalAge = new OriginDataParameter(40)
         };

         _originData.DiseaseState = new DiseaseState {Name = "CKD"};
         _diseaseStateParameter = new OriginDataParameter {Name = "Param", Value = 60, Unit = "h"};
         _originData.AddDiseaseStateParameter(_diseaseStateParameter);
         _timeDimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         A.CallTo(() => _dimensionRepository.DimensionForUnit(_diseaseStateParameter.Unit)).Returns(_timeDimension);
         A.CallTo(() => _parameterMapper.ParameterFrom(_diseaseStateParameter.Value, _diseaseStateParameter.Unit, _timeDimension))
            .Returns(new Parameter {Value = 1, Unit = "h"});

         return _completed;
      }
   }

   public class When_mapping_a_model_disease_state_to_snapshot : concern_for_DiseaseStateMapper
   {
      private Snapshots.DiseaseState _snapshot;

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_originData);
      }

      [Observation]
      public void should_return_the_expected_snapshot()
      {
         _snapshot.Name.ShouldBeEqualTo(_originData.DiseaseState.Name);
         _snapshot.Parameters.Length.ShouldBeEqualTo(1);
         var param = _snapshot.Parameters[0];
         param.Name.ShouldBeEqualTo("Param");
         param.Unit.ShouldBeEqualTo(_diseaseStateParameter.Unit);
         param.Value.ShouldBeEqualTo(1); //60 in base unit converted to display unit
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_with_disease_state : concern_for_DiseaseStateMapper
   {
      private IDimension _timeDimension;
      private Snapshots.DiseaseState _snapshot;

      protected override async Task Context()
      {
         await base.Context();
         _timeDimension = DomainHelperForSpecs.TimeDimensionForSpecs();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Name = "Disease";
         _snapshot.Parameters = new[] {new Parameter {Name = "P1", Value = 1, Unit = "h"}};

         A.CallTo(() => _dimensionRepository.DimensionForUnit(_diseaseStateParameter.Unit)).Returns(_timeDimension);
         var diseaseState = new DiseaseState {Name = "Disease"};
         diseaseState.Add(DomainHelperForSpecs.ConstantParameterWithValue(20).WithName("P1"));
         A.CallTo(() => _diseaseStateRepository.AllFor(_originData.Population)).Returns(new[] {diseaseState});
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshot, new DiseaseStateContext(_originData, new SnapshotContext()));
      }

      [Observation]
      public void should_set_the_disease_state_and_parameters()
      {
         _originData.DiseaseState.Name.ShouldBeEqualTo("Disease");
         _originData.DiseaseStateParameters.Count.ShouldBeEqualTo(1);
         var param = _originData.DiseaseStateParameters[0];
         param.Value.ShouldBeEqualTo(60);
         param.Unit.ShouldBeEqualTo("h");
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_with_disease_state_parameter_unknown : concern_for_DiseaseStateMapper
   {
      private Snapshots.DiseaseState _snapshot;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Name = "Disease";
         _snapshot.Parameters = new[] {new Parameter {Name = "Unknown", Value = 10, Unit = "h"}};

         var diseaseState = new DiseaseState {Name = "Disease"};
         diseaseState.Add(DomainHelperForSpecs.ConstantParameterWithValue(20).WithName("P1"));
         A.CallTo(() => _diseaseStateRepository.AllFor(_originData.Population)).Returns(new[] {diseaseState});
      }

      protected override async Task Because()
      {
         await sut.MapToModel(_snapshot, new DiseaseStateContext(_originData, new SnapshotContext()));
      }

      [Observation]
      public void should_set_the_disease_state_and_parameters_as_default_from_the_disease_state()
      {
         _originData.DiseaseState.Name.ShouldBeEqualTo("Disease");
         _originData.DiseaseStateParameters.Count.ShouldBeEqualTo(1);
         var param = _originData.DiseaseStateParameters[0];
         param.Value.ShouldBeEqualTo(20);
      }
   }

   public class When_mapping_an_origin_data_from_snapshot_with_disease_state_that_is_unknown : concern_for_DiseaseStateMapper
   {
      private Snapshots.DiseaseState _snapshot;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_originData);
         _snapshot.Name = "UnknownDiseaseState";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.MapToModel(_snapshot, new DiseaseStateContext(_originData, new SnapshotContext()))).ShouldThrowAn<PKSimException>();
      }
   }
}