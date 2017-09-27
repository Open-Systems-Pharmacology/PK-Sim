﻿using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Batch.Mapper;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using IndividualMapper = PKSim.Core.Snapshots.Mappers.IndividualMapper;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using ModelIndividual = PKSim.Core.Model.Individual;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualMapper : ContextSpecificationAsync<IndividualMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected ModelIndividual _individual;
      protected SnapshotIndividual _snapshot;
      protected IParameter _parameterLiver;
      protected IParameter _parameterKidney;
      protected string _parameterKidneyPath = "ParameterKidneyPath";
      protected IDimensionRepository _dimensionRepository;
      protected IndividualEnzyme _enzyme;
      protected IndividualTransporter _transporter;
      protected MoleculeMapper _moleculeMapper;
      protected Parameter _ageSnapshotParameter;
      protected Parameter _heightSnapshotParameter;
      protected Molecule _enzymeSnapshot;
      protected Molecule _transporterSnapshot;
      protected LocalizedParameter _localizedParameterKidney;
      protected IIndividualFactory _individualFactory;
      protected IOriginDataMapper _originDataMapper;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _moleculeMapper = A.Fake<MoleculeMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _originDataMapper = A.Fake<IOriginDataMapper>();

         sut = new IndividualMapper(_parameterMapper, _dimensionRepository, _moleculeMapper, _individualFactory, _originDataMapper);

         _individual = DomainHelperForSpecs.CreateIndividual();
         _individual.Name = "Ind";
         _individual.Description = "Model Description";

         _individual.OriginData.Age = 35;
         _individual.OriginData.AgeUnit = "years";
         _individual.OriginData.Height = 17.8;
         _individual.OriginData.HeightUnit = "m";
         _individual.OriginData.Weight = 73;
         _individual.OriginData.WeightUnit = "kg";

         _parameterLiver = _individual.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Organ.Liver, "PLiver");
         _parameterKidney = _individual.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Organ.Kidney, "PKidney");

         _parameterLiver.ValueDiffersFromDefault().ShouldBeFalse();
         _parameterKidney.ValueDiffersFromDefault().ShouldBeFalse();

         _parameterKidney.Value = 40;
         _parameterKidney.ValueDiffersFromDefault().ShouldBeTrue();

         _enzyme = new IndividualEnzyme
         {
            Name = "Enz",
         };
         _individual.AddMolecule(_enzyme);

         _transporter = new IndividualTransporter
         {
            Name = "Trans",
         };

         _individual.AddMolecule(_transporter);

         _ageSnapshotParameter = new Parameter();
         _heightSnapshotParameter = new Parameter();

         A.CallTo(() => _parameterMapper.ParameterFrom(null, A<string>._, A<IDimension>._)).Returns(null);
         A.CallTo(() => _parameterMapper.ParameterFrom(_individual.OriginData.Age, A<string>._, A<IDimension>._)).Returns(_ageSnapshotParameter);
         A.CallTo(() => _parameterMapper.ParameterFrom(_individual.OriginData.Height, A<string>._, A<IDimension>._)).Returns(_heightSnapshotParameter);

         _enzymeSnapshot = new Molecule { Type = QuantityType.Enzyme};
         A.CallTo(() => _moleculeMapper.MapToSnapshot(_enzyme)).Returns(_enzymeSnapshot);
         _transporterSnapshot = new Molecule {Type = QuantityType.Transporter};
         A.CallTo(() => _moleculeMapper.MapToSnapshot(_transporter)).Returns(_transporterSnapshot);

         _localizedParameterKidney = new LocalizedParameter {Path = "Organism|Kidney|PKidney"};
         A.CallTo(() => _parameterMapper.LocalizedParameterFrom(_parameterKidney)).Returns(_localizedParameterKidney);
         return Task.FromResult(true);
      }
   }

   public class When_mapping_an_individual_to_snapshot : concern_for_IndividualMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_individual);
      }

      [Observation]
      public void should_save_the_individual_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_individual.Name);
         _snapshot.Description.ShouldBeEqualTo(_individual.Description);
      }

      [Observation]
      public void should_save_the_origin_data_properties()
      {
         _snapshot.Species.ShouldBeEqualTo(_individual.OriginData.Species.Name);
         _snapshot.Population.ShouldBeEqualTo(_individual.OriginData.SpeciesPopulation.Name);
         _snapshot.Gender.ShouldBeEqualTo(_individual.OriginData.Gender.Name);

         _snapshot.Age.ShouldBeEqualTo(_ageSnapshotParameter);
         _snapshot.Height.ShouldBeEqualTo(_heightSnapshotParameter);

         //those parameters where not set in example
         _snapshot.GestationalAge.ShouldBeNull();
      }

      [Observation]
      public void should_save_all_individual_parameters_that_have_been_changed_by_the_user()
      {
         _snapshot.Parameters.ShouldOnlyContain(_localizedParameterKidney);
      }

      [Observation]
      public void should_save_the_individual_molecules()
      {
         _snapshot.Molecules.ShouldOnlyContain(_enzymeSnapshot, _transporterSnapshot);
      }
   }

   public class When_mapping_a_valid_individual_snapshot_to_an_individual : concern_for_IndividualMapper
   {
      private ModelIndividual _newIndividual;
      private IndividualMolecule _molecule1;
      private IndividualMolecule _molecule2;
      private Batch.OriginData _batchOriginData;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_individual);

         A.CallTo(() => _originDataMapper.MapFrom(A<Batch.OriginData>._))
            .Invokes(x => _batchOriginData = x.GetArgument<Batch.OriginData>(0));
            

         A.CallTo(() => _individualFactory.CreateAndOptimizeFor(A<OriginData>._, _snapshot.Seed))
            .Returns(_individual);

         _snapshot.Name = "New individual";
         _snapshot.Description = "The description that will be deserialized";

         //clear enzyme before mapping them again
         _individual.RemoveMolecule(_enzyme);
         _individual.RemoveMolecule(_transporter);

         //reset parameter
         _parameterKidney.ResetToDefault();

         _molecule1 = new IndividualEnzyme().WithName("Mol1");
         _molecule2 = new IndividualEnzyme().WithName("Mol2");

         A.CallTo(() => _moleculeMapper.MapToModels(_snapshot.Molecules, _individual)).ReturnsAsync(new[] {_molecule1,_molecule2});


         A.CallTo(() => _dimensionRepository.Mass.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Weight.Value)).Returns(10);
         A.CallTo(() => _dimensionRepository.AgeInYears.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Age.Value)).Returns(20);
         A.CallTo(() => _dimensionRepository.Length.UnitValueToBaseUnitValue(A<Unit>._, _snapshot.Height.Value)).Returns(30);
      }

      protected override async Task Because()
      {
         _newIndividual = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_use_the_expected_individual_origin_data_to_create_the_individual()
      {
         _batchOriginData.Species.ShouldBeEqualTo(_snapshot.Species);
         _batchOriginData.Population.ShouldBeEqualTo(_snapshot.Population);
         _batchOriginData.Gender.ShouldBeEqualTo(_snapshot.Gender);
         _batchOriginData.Weight.ShouldBeEqualTo(10);
         _batchOriginData.Age.ShouldBeEqualTo(20);
         _batchOriginData.Height.ShouldBeEqualTo(30);

         double.IsNaN(_batchOriginData.GestationalAge).ShouldBeTrue();

      }
      [Observation]
      public void should_have_created_an_event_with_the_expected_properties()
      {
         _newIndividual.Name.ShouldBeEqualTo(_snapshot.Name);
         _newIndividual.Description.ShouldBeEqualTo(_snapshot.Description);
      }

      [Observation]
      public void should_have_created_the_expected_molecules()
      {
         _newIndividual.AllMolecules().ShouldOnlyContain(_molecule1, _molecule2);
      }

      [Observation]
      public void should_have_updated_the_parameter_previously_set_by_the_user()
      {
         A.CallTo(() => _parameterMapper.MapLocalizedParameters(_snapshot.Parameters, _individual.Organism)).MustHaveHappened();
      }
   }
}