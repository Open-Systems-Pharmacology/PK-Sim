using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Core.Snapshots.Services;
using ExpressionProfile = PKSim.Core.Snapshots.ExpressionProfile;
using Individual = PKSim.Core.Model.Individual;
using Ontogeny = PKSim.Core.Model.Ontogeny;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionProfileMapper : ContextSpecificationAsync<ExpressionProfileMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected ExpressionProfile _snapshot;
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
      protected IOntogenyTask _ontogenyTask;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      protected IExpressionProfileFactory _expressionProfileFactory;
      protected Model.ExpressionProfile _expressionProfileEnzyme;
      protected Model.ExpressionProfile _expressionProfileTransporter;
      protected Model.ExpressionProfile _expressionProfileOtherProtein;

      protected override Task Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _expressionContainerMapper = A.Fake<ExpressionContainerMapper>();
         _expressionProfileFactory = A.Fake<IExpressionProfileFactory>();
         _ontogenyMapper = A.Fake<OntogenyMapper>();
         _ontogenyTask = A.Fake<IOntogenyTask>();
         _moleculeExpressionTask = A.Fake<IMoleculeExpressionTask<Individual>>();
         sut = new ExpressionProfileMapper(
            _parameterMapper, 
            _expressionContainerMapper, 
            _ontogenyMapper, 
            _ontogenyTask, 
            _moleculeExpressionTask, 
            _expressionProfileFactory);

         _ontogeny = new DatabaseOntogeny
         {
            Name = "Ontogeny"
         };

         _expressionProfileEnzyme = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>("Enzyme");
         _expressionProfileEnzyme.Molecule.Ontogeny = _ontogeny;
         _expressionProfileEnzyme.Description = "Help";

         _expressionProfileTransporter = DomainHelperForSpecs.CreateExpressionProfile<IndividualTransporter>("Transporter");
         _expressionProfileTransporter.Description = "Help";

         _expressionProfileOtherProtein = DomainHelperForSpecs.CreateExpressionProfile<IndividualOtherProtein>("OtherProtein");

         _enzymeGlobalParameter = DomainHelperForSpecs.ConstantParameterWithValue(5, isDefault: true)
            .WithName(CoreConstants.Parameters.HALF_LIFE_LIVER);
         _enzymeGlobalParameterSnapshot = new Parameter();

         A.CallTo(() => _parameterMapper.MapToSnapshot(_enzymeGlobalParameter)).Returns(_enzymeGlobalParameterSnapshot);

         _expressionContainer1 = new MoleculeExpressionContainer {Name = "Exp Container1"};
         _expressionContainer2 = new MoleculeExpressionContainer {Name = "Exp Container2"};
         _expressionProfileEnzyme.Individual.AddChildren(_expressionContainer1, _expressionContainer2, _enzymeGlobalParameter);
         _expressionProfileEnzyme.Molecule.DowncastTo<IndividualEnzyme>().Localization = Localization.Intracellular | Localization.BloodCellsMembrane;

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

   public class When_mapping_an_individual_enzyme_to_snapshot : concern_for_ExpressionProfileMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_expressionProfileEnzyme);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties_set_for_enzyme()
      {
         _snapshot.Name.ShouldBeNull();
         _snapshot.Molecule.ShouldBeEqualTo(_expressionProfileEnzyme.Molecule.Name);
         _snapshot.Type.ShouldBeEqualTo(QuantityType.Enzyme);
         _snapshot.Category.ShouldBeEqualTo(_expressionProfileEnzyme.Category);
         _snapshot.Species.ShouldBeEqualTo(_expressionProfileEnzyme.Species.Name);
         _snapshot.Description.ShouldBeEqualTo(_expressionProfileEnzyme.Description);
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
         _snapshot.Localization.ShouldBeEqualTo(_expressionProfileEnzyme.Molecule.DowncastTo<IndividualEnzyme>().Localization);
         _snapshot.TransportType.ShouldBeNull();
      }

      [Observation]
      public void should_have_saved_the_ontogeny_of_the_molecule()
      {
         _snapshot.Ontogeny.ShouldBeEqualTo(_snapshotOntogeny);
      }

      [Observation]
      public void should_save_parameters_that_are_now_saved_in_the_individual()
      {
         _snapshot.Parameters.ShouldNotBeNull();
      }
   }

   public class When_mapping_an_individual_transporter_to_snapshot : concern_for_ExpressionProfileMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_expressionProfileTransporter);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties_set_for_transporter()
      {
         _snapshot.Description.ShouldBeEqualTo(_expressionProfileTransporter.Description);
      }

      [Observation]
      public void should_have_saved_transporter_specific_properties()
      {
         _snapshot.TransportType.ShouldBeEqualTo(_expressionProfileTransporter.Molecule.DowncastTo<IndividualTransporter>().TransportType);
         _snapshot.MembraneLocation.ShouldBeNull();
         _snapshot.TissueLocation.ShouldBeNull();
         _snapshot.IntracellularVascularEndoLocation.ShouldBeNull();
      }
   }

   public class When_mapping_a_valid_enzyme_expression_profile_snapshot_to_a_expression_profile : concern_for_ExpressionProfileMapper
   {
      private Model.ExpressionProfile _newExpressionProfile;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_expressionProfileEnzyme);
         _newExpressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         A.CallTo(() => _expressionProfileFactory.Create(_snapshot.Type, _snapshot.Species, _snapshot.Molecule))
            .Returns(_newExpressionProfile);

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

         _newExpressionProfile.Molecule.Ontogeny = null;
         A.CallTo(() => _ontogenyMapper.MapToModel(_snapshot.Ontogeny, _newExpressionProfile.Individual)).Returns(_ontogeny);

         var enzyme = _newExpressionProfile.Molecule.DowncastTo<IndividualEnzyme>();
         //Localization is now set in task. We override behavior here and pretend that command was executed
         A.CallTo(() => _moleculeExpressionTask.SetExpressionLocalizationFor(enzyme, A<Localization>._, _newExpressionProfile.Individual))
            .Invokes(x => enzyme.Localization = x.GetArgument<Localization>(1));

         //Ensure that the parameter is at the right location so that it will be normalized
         enzyme.Add(_relativeExpressionParameter1);
      }

      protected override async Task Because()
      {
         _newExpressionProfile = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newExpressionProfile.ShouldNotBeNull();
      }

      [Observation]
      public void should_return_the_expected_molecule_with_the_matching_properties()
      {
         _newExpressionProfile.Molecule.DowncastTo<IndividualEnzyme>().Localization.ShouldBeEqualTo(Localization.Interstitial | Localization.BloodCellsIntracellular | Localization.VascMembraneTissueSide);
      }

      [Observation]
      public void should_have_restored_the_ontogeny()
      {
         A.CallTo(() => _ontogenyTask.SetOntogenyForMolecule(_newExpressionProfile.Molecule, _ontogeny, _newExpressionProfile.Individual)).MustHaveHappened();
      }

      [Observation]
      public void should_have_normalized_the_value_of_the_expression_parameters_for_a_v9_format()
      {
         _relativeExpressionParameter1.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_mapping_a_valid_transporter_expression_profile_snapshot_to_a_expression_profile : concern_for_ExpressionProfileMapper
   {
      private Model.ExpressionProfile _newTransporterExpressionProfile;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_expressionProfileTransporter);
         _newTransporterExpressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualTransporter>();
         A.CallTo(() => _expressionProfileFactory.Create(_snapshot.Type, _snapshot.Species, _snapshot.Molecule))
            .Returns(_newTransporterExpressionProfile);
         _snapshot.TransportType = TransportType.PgpLike;
      }

      protected override async Task Because()
      {
         _newTransporterExpressionProfile = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newTransporterExpressionProfile.ShouldNotBeNull();
      }

      [Observation]
      public void should_return_the_expected_transporter_with_the_matching_properties()
      {
         A.CallTo(() => _moleculeExpressionTask.SetTransporterTypeFor(_newTransporterExpressionProfile.Molecule.DowncastTo<IndividualTransporter>(), TransportType.PgpLike)).MustHaveHappened();
      }
   }

   public class When_mapping_a_valid_other_protein_expression_profile_snapshot_to_a_expression_profile : concern_for_ExpressionProfileMapper
   {
      private Model.ExpressionProfile _newOtherProteinExpressionProfile;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_expressionProfileOtherProtein);
         _newOtherProteinExpressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualOtherProtein>();
         A.CallTo(() => _expressionProfileFactory.Create(_snapshot.Type, _snapshot.Species, _snapshot.Molecule))
            .Returns(_newOtherProteinExpressionProfile);
      }

      protected override async Task Because()
      {
         _newOtherProteinExpressionProfile = await sut.MapToModel(_snapshot);
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newOtherProteinExpressionProfile.ShouldNotBeNull();
      }
   }
}