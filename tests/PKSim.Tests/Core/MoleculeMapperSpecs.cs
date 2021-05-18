using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using Individual = PKSim.Core.Model.Individual;
using Ontogeny = PKSim.Core.Model.Ontogeny;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeMapper : ContextSpecificationAsync<MoleculeMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected IndividualEnzyme _enzyme;
      protected IndividualTransporter _transporter;
      protected IndividualOtherProtein _otherProtein;
      protected Molecule _snapshot;
      private IParameter _enzymeGlobalParameter;
      protected Parameter _enzymeGlobalParameterSnapshot;
      private MoleculeExpressionContainer _expressionContainer1;
      protected IParameter _relativeExpressionParameter1;
      private IParameter _relativeExpressionParameterNotSet;
      private MoleculeExpressionContainer _expressionContainer2;
      protected ExpressionContainer _relativeExpressionContainerSnapshot1;
      protected ExpressionContainer _relativeExpressionContainerSnapshot2;
      protected Individual _individual;
      protected OntogenyMapper _ontogenyMapper;
      protected Ontogeny _ontogeny;
      protected Snapshots.Ontogeny _snapshotOntogeny;
      protected ExpressionContainerMapper _expressionContainerMapper;
      protected IOntogenyTask<Individual> _ontogenyTask;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _expressionContainerMapper = A.Fake<ExpressionContainerMapper>();
         _ontogenyMapper = A.Fake<OntogenyMapper>();
         _ontogenyTask = A.Fake<IOntogenyTask<Individual>>();
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         sut = new MoleculeMapper(_parameterMapper, _expressionContainerMapper, _ontogenyMapper, _ontogenyTask, _moleculeExpressionTask);

         _ontogeny = new DatabaseOntogeny
         {
            Name = "Ontogeny"
         };

         _enzyme = new IndividualEnzyme
         {
            Name = "Enzyme",
            Description = "Help",
            Ontogeny = _ontogeny,
         };

         _transporter = new IndividualTransporter
         {
            Name = "Transporter",
            Description = "Help"
         };

         _otherProtein = new IndividualOtherProtein
         {
            Name = "OtherProtein",
            Description = "Help"
         };

         _enzymeGlobalParameter = DomainHelperForSpecs.ConstantParameterWithValue(5, isDefault: true)
            .WithName(CoreConstants.Parameters.HALF_LIFE_LIVER);
         _enzymeGlobalParameterSnapshot = new Parameter();

         A.CallTo(() => _parameterMapper.MapToSnapshot(_enzymeGlobalParameter)).Returns(_enzymeGlobalParameterSnapshot);

         _expressionContainer1 = new MoleculeExpressionContainer {Name = "Exp Container1"};
         _expressionContainer2 = new MoleculeExpressionContainer {Name = "Exp Container2"};
         _enzyme.AddChildren(_expressionContainer1, _expressionContainer2, _enzymeGlobalParameter);
         _enzyme.Localization = Localization.Intracellular | Localization.BloodCellsMembrane;

         _relativeExpressionParameter1 = DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.Parameters.REL_EXP);
         _expressionContainer1.Add(_relativeExpressionParameter1);

         _relativeExpressionParameterNotSet = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameters.REL_EXP);
         _expressionContainer2.Add(_relativeExpressionParameterNotSet);

         _relativeExpressionContainerSnapshot1 = new ExpressionContainer();
         _relativeExpressionContainerSnapshot2 = new ExpressionContainer();

         A.CallTo(() => _expressionContainerMapper.MapToSnapshot(_expressionContainer1)).Returns(_relativeExpressionContainerSnapshot1);
         A.CallTo(() => _expressionContainerMapper.MapToSnapshot(_expressionContainer2)).Returns(_relativeExpressionContainerSnapshot2);

         _snapshotOntogeny = new Snapshots.Ontogeny();
         A.CallTo(() => _ontogenyMapper.MapToSnapshot(_ontogeny)).Returns(_snapshotOntogeny);
         _individual = new Individual();

         return _completed;
      }
   }

   public class When_mapping_an_individual_enzyme_to_snapshot : concern_for_MoleculeMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_enzyme, _individual);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties_set_for_enzyme()
      {
         _snapshot.Name.ShouldBeEqualTo(_enzyme.Name);
         _snapshot.Description.ShouldBeEqualTo(_enzyme.Description);
      }

      [Observation]
      public void should_not_set_container_specific_parameters()
      {
         _snapshot.Expression.ShouldBeNull();
      }

      [Observation]
      public void should_have_saved_enzyme_specific_properties()
      {
         _snapshot.IntracellularVascularEndoLocation.ShouldBeNull();
         _snapshot.MembraneLocation.ShouldBeNull();
         _snapshot.TissueLocation.ShouldBeNull();
         _snapshot.Localization.ShouldBeEqualTo(_enzyme.Localization);
         _snapshot.TransportType.ShouldBeNull();
      }

      [Observation]
      public void should_have_saved_the_ontogeny_of_the_molecule()
      {
         _snapshot.Ontogeny.ShouldBeEqualTo(_snapshotOntogeny);
      }

      [Observation]
      public void should_also_not_saved_parameters_that_are_now_saved_in_the_individual()
      {
         _snapshot.Parameters.ShouldBeNull();
      }
   }

   public class When_mapping_an_individual_transporter_to_snapshot : concern_for_MoleculeMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_transporter, _individual);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties_set_for_transporter()
      {
         _snapshot.Name.ShouldBeEqualTo(_transporter.Name);
         _snapshot.Description.ShouldBeEqualTo(_transporter.Description);
      }

      [Observation]
      public void should_have_saved_transporter_specific_properties()
      {
         _snapshot.TransportType.ShouldBeEqualTo(_transporter.TransportType);
         _snapshot.MembraneLocation.ShouldBeNull();
         _snapshot.TissueLocation.ShouldBeNull();
         _snapshot.IntracellularVascularEndoLocation.ShouldBeNull();
      }
   }

   public class When_mapping_a_valid_enzyme_molecule_snapshot_to_a_molecule : concern_for_MoleculeMapper
   {
      private IndividualEnzyme _newMolecule;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_enzyme, _individual);

         _snapshot.Localization = null;
         _snapshot.IntracellularVascularEndoLocation = IntracellularVascularEndoLocation.Interstitial;
         _snapshot.MembraneLocation = MembraneLocation.BloodBrainBarrier;
         _snapshot.TissueLocation = TissueLocation.Interstitial;
         //Add expression container to simulate v9 format
         _snapshot.Expression = new[] {_relativeExpressionContainerSnapshot1, _relativeExpressionContainerSnapshot2,};
         _relativeExpressionParameter1.Value = 0;
         _relativeExpressionContainerSnapshot1.Value = 1000;
         A.CallTo(() => _expressionContainerMapper.MapToModel(_relativeExpressionContainerSnapshot1, A<ExpressionContainerMapperContext>._))
            .Invokes(x => _relativeExpressionParameter1.Value = _relativeExpressionContainerSnapshot1.Value.Value);

         _enzyme.Ontogeny = null;
         A.CallTo(() => _ontogenyMapper.MapToModel(_snapshot.Ontogeny, _individual)).Returns(_ontogeny);

         //we need to add the molecule that is now added as part of the creation process of a molecule
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualEnzyme>(_individual, _snapshot.Name))
            .Invokes(x => _individual.AddMolecule(_enzyme));


         //Localization is now set in task. We override behavior here and pretend that command was executed
         A.CallTo(() => _moleculeExpressionTask.SetExpressionLocalizationFor(_enzyme, A<Localization>._, _individual))
            .Invokes(x => _enzyme.Localization = x.GetArgument<Localization>(1));

         //Ensure that the parameter is at the right location so that it will be normalized
         _enzyme.Add(_relativeExpressionParameter1);
      }

      protected override async Task Because()
      {
         _newMolecule = await sut.MapToModel(_snapshot, _individual) as IndividualEnzyme;
      }

      [Observation]
      public void should_have_added_molecule_to_the_individual()
      {
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualEnzyme>(_individual, _snapshot.Name)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newMolecule.ShouldNotBeNull();
      }

      [Observation]
      public void should_return_the_expected_molecule_with_the_matching_properties()
      {
         _newMolecule.Localization.ShouldBeEqualTo(Localization.Interstitial | Localization.BloodCellsIntracellular | Localization.VascMembraneTissueSide);
      }

      [Observation]
      public void should_have_restored_the_ontogeny()
      {
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_newMolecule, _ontogeny, _individual)).MustHaveHappened();
      }

      [Observation]
      public void should_have_normalized_the_value_of_the_expression_parameters_for_a_v9_format()
      {
         _relativeExpressionParameter1.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_mapping_a_valid_transporter_molecule_snapshot_to_a_molecule : concern_for_MoleculeMapper
   {
      private IndividualTransporter _newTransporter;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_transporter, _individual);

         _snapshot.TransportType = TransportType.PgpLike;

         //we need to add the molecule that is now added as part of the creation process of a molecule
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualTransporter>(_individual, _snapshot.Name))
            .Invokes(x => _individual.AddMolecule(_transporter));
      }

      protected override async Task Because()
      {
         _newTransporter = await sut.MapToModel(_snapshot, _individual) as IndividualTransporter;
      }

      [Observation]
      public void should_have_added_molecule_to_the_individual()
      {
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualTransporter>(_individual, _snapshot.Name)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newTransporter.ShouldNotBeNull();
      }

      [Observation]
      public void should_return_the_expected_transporter_with_the_matching_properties()
      {
         _newTransporter.TransportType.ShouldBeEqualTo(TransportType.PgpLike);
      }
   }

   public class When_mapping_a_valid_other_protein_molecule_snapshot_to_a_molecule : concern_for_MoleculeMapper
   {
      private IndividualOtherProtein _newOtherProtein;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_otherProtein, _individual);

         //we need to add the molecule that is now added as part of the creation process of a molecule
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualOtherProtein>(_individual, _snapshot.Name))
            .Invokes(x => _individual.AddMolecule(_otherProtein));
      }

      protected override async Task Because()
      {
         _newOtherProtein = await sut.MapToModel(_snapshot, _individual) as IndividualOtherProtein;
      }

      [Observation]
      public void should_have_added_molecule_to_the_individual()
      {
         A.CallTo(() => _moleculeExpressionTask.AddMoleculeTo<IndividualOtherProtein>(_individual, _snapshot.Name)).MustHaveHappened();
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newOtherProtein.ShouldNotBeNull();
      }
   }
}