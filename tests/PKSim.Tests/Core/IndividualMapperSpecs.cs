using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;
using ModelIndividual = PKSim.Core.Model.Individual;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualMapper : ContextSpecification<IndividualMapper>
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

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _moleculeMapper = A.Fake<MoleculeMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();

         sut = new IndividualMapper(_parameterMapper, _dimensionRepository, _moleculeMapper);

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

         A.CallTo(() => _moleculeMapper.MapToSnapshot(_enzyme)).Returns(_enzymeSnapshot);
         A.CallTo(() => _moleculeMapper.MapToSnapshot(_transporter)).Returns(_transporterSnapshot);

         _localizedParameterKidney= new LocalizedParameter();
         A.CallTo(() => _parameterMapper.LocalizedParameterFrom(_parameterKidney)).Returns(_localizedParameterKidney);
      }
   }

   public class When_mapping_an_individual_to_snapshot : concern_for_IndividualMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_individual);
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
         _snapshot.Enzymes.ShouldContain(_enzymeSnapshot);
         _snapshot.Transporters.ShouldContain(_transporterSnapshot);
      }
   }
}