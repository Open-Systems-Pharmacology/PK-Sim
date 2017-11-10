using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using ModelIndividual = PKSim.Core.Model.Individual;
using OriginData = PKSim.Core.Snapshots.OriginData;

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
      protected Molecule _enzymeSnapshot;
      protected Molecule _transporterSnapshot;
      protected LocalizedParameter _localizedParameterKidney;
      protected IIndividualFactory _individualFactory;
      protected OriginDataMapper _originDataMapper;
      protected OriginData _originDataSnapshot;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _moleculeMapper = A.Fake<MoleculeMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _originDataMapper = A.Fake<OriginDataMapper>();

         sut = new IndividualMapper(_parameterMapper, _moleculeMapper, _originDataMapper, _individualFactory);

         _individual = DomainHelperForSpecs.CreateIndividual();
         _individual.Name = "Ind";
         _individual.Description = "Model Description";

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

         _enzymeSnapshot = new Molecule {Type = QuantityType.Enzyme};
         _transporterSnapshot = new Molecule {Type = QuantityType.Transporter};

         A.CallTo(() => _moleculeMapper.MapToSnapshot(_enzyme)).Returns(_enzymeSnapshot);
         A.CallTo(() => _moleculeMapper.MapToSnapshot(_transporter)).Returns(_transporterSnapshot);

         _originDataSnapshot = new OriginData();
         A.CallTo(() => _originDataMapper.MapToSnapshot(_individual.OriginData)).Returns(_originDataSnapshot);

         _localizedParameterKidney = new LocalizedParameter {Path = "Organism|Kidney|PKidney"};
         A.CallTo(() => _parameterMapper.LocalizedParametersFrom(A<IEnumerable<IParameter>>.That.Contains(_parameterKidney))).Returns(new[] {_localizedParameterKidney});

         return _completed;
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
         _snapshot.OriginData.ShouldBeEqualTo(_originDataSnapshot);
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
      private Model.OriginData _newOriginData;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_individual);

   
         _snapshot.Name = "New individual";
         _snapshot.Description = "The description that will be deserialized";

         //clear enzyme before mapping them again
         _individual.RemoveMolecule(_enzyme);
         _individual.RemoveMolecule(_transporter);

         //reset parameter
         _parameterKidney.ResetToDefault();

         _molecule1 = new IndividualEnzyme().WithName("Mol1");
         _molecule2 = new IndividualEnzyme().WithName("Mol2");

         A.CallTo(() => _moleculeMapper.MapToModel(_enzymeSnapshot, _individual)).Returns(_molecule1);
         A.CallTo(() => _moleculeMapper.MapToModel(_transporterSnapshot, _individual)).Returns(_molecule2);

         _newOriginData = new Model.OriginData();
         A.CallTo(() => _originDataMapper.MapToModel(_snapshot.OriginData)).Returns(_newOriginData);

         A.CallTo(() => _individualFactory.CreateAndOptimizeFor(_newOriginData, _snapshot.Seed))
            .Returns(_individual);

      }

      protected override async Task Because()
      {
         _newIndividual = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_use_the_expected_individual_origin_data_to_create_the_individual()
      {
         _newIndividual.ShouldNotBeNull();
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