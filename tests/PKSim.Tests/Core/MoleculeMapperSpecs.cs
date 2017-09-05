using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeMapper : ContextSpecification<MoleculeMapper>
   {
      protected ParameterMapper _parameterMapper;
      protected IndividualEnzyme _enzyme;
      protected IndividualTransporter _transporter;
      protected IndividualOtherProtein _otherProtein;
      protected Molecule _snapshot;
      private IParameter _enzymeParameter;
      private Parameter _enzymeParameterSnapshot;
      private MoleculeExpressionContainer _expressionContainer1;
      protected IParameter _relativeExpressionParameter1;
      private IParameter _relativeExpressionParameterNotSet;
      private MoleculeExpressionContainer _expressionContainer2;
      protected LocalizedParameter _relativeExpressionSnapshot1;
      protected IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private IExecutionContext _executionContext;
      protected IParameter _relativeExpressionParameterNorm1;
      private IParameter _relativeExpressionParameterNotSetNorm;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         _executionContext = A.Fake<IExecutionContext>();
         _individualMoleculeFactoryResolver = A.Fake<IIndividualMoleculeFactoryResolver>();
         sut = new MoleculeMapper(_parameterMapper, _individualMoleculeFactoryResolver, _executionContext);

         _enzyme = new IndividualEnzyme
         {
            Name = "Enzyme",
            Description = "Hellp"
         };

         _transporter = new IndividualTransporter
         {
            Name = "Transporter",
            Description = "Hellp"
         };

         _otherProtein = new IndividualOtherProtein
         {
            Name = "OtherProtein",
            Description = "Hellp"
         };

         _enzymeParameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("HalfLife");
         _enzymeParameterSnapshot = new Parameter();

         A.CallTo(() => _parameterMapper.MapToSnapshot(_enzymeParameter)).Returns(_enzymeParameterSnapshot);

         _expressionContainer1 = new MoleculeExpressionContainer {Name = "Exp Container1"};
         _expressionContainer2 = new MoleculeExpressionContainer {Name = "Exp Container2"};
         _enzyme.AddChildren(_expressionContainer1, _expressionContainer2);

         _relativeExpressionParameter1 = DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.Parameter.REL_EXP);
         _relativeExpressionParameterNorm1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.REL_EXP_NORM);
         _expressionContainer1.Add(_relativeExpressionParameter1);
         _expressionContainer1.Add(_relativeExpressionParameterNorm1);

         _relativeExpressionParameterNotSet = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameter.REL_EXP);
         _relativeExpressionParameterNotSetNorm = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameter.REL_EXP_NORM);
         _expressionContainer2.Add(_relativeExpressionParameterNotSet);
         _expressionContainer2.Add(_relativeExpressionParameterNotSetNorm);

         _relativeExpressionSnapshot1 = new LocalizedParameter
         {
            Path = _expressionContainer1.Name,
            Value = _relativeExpressionParameter1.Value
         };

         A.CallTo(() => _parameterMapper.LocalizedParameterFrom(_relativeExpressionParameter1, A<Func<IParameter, string>>._)).Returns(_relativeExpressionSnapshot1);
      }
   }

   public class When_mapping_an_individual_enzyme_to_snapshot : concern_for_MoleculeMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_enzyme);
      }

      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties_set_for_enzyme()
      {
         _snapshot.Name.ShouldBeEqualTo(_enzyme.Name);
         _snapshot.Description.ShouldBeEqualTo(_enzyme.Description);
      }

      [Observation]
      public void should_have_saved_the_relative_expression_parameters_values_that_are_set()
      {
         _snapshot.Expression.ShouldOnlyContain(_relativeExpressionSnapshot1);
      }

      [Observation]
      public void should_have_saved_enzyme_specific_properties()
      {
         _snapshot.IntracellularVascularEndoLocation.ShouldBeEqualTo(_enzyme.IntracellularVascularEndoLocation.ToString());
         _snapshot.MembraneLocation.ShouldBeEqualTo(_enzyme.MembraneLocation.ToString());
         _snapshot.TissueLocation.ShouldBeEqualTo(_enzyme.TissueLocation.ToString());
         _snapshot.TransportType.ShouldBeNull();
      }
   }

   public class When_mapping_an_individual_transporter_to_snapshot : concern_for_MoleculeMapper
   {
      protected override void Because()
      {
         _snapshot = sut.MapToSnapshot(_transporter);
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
         _snapshot.TransportType.ShouldBeEqualTo(_transporter.TransportType.ToString());
         _snapshot.MembraneLocation.ShouldBeNull();
         _snapshot.TissueLocation.ShouldBeNull();
         _snapshot.IntracellularVascularEndoLocation.ShouldBeNull();
      }
   }

   public class When_mapping_a_valid_enzyme_molecule_snahpshot_to_a_molecule : concern_for_MoleculeMapper
   {
      private ISimulationSubject _simulationSubject;
      private IndividualEnzyme _newMolecule;

      protected override void Context()
      {
         base.Context();
         _simulationSubject = A.Fake<ISimulationSubject>();
         _snapshot = sut.MapToSnapshot(_enzyme);

         _snapshot.IntracellularVascularEndoLocation = IntracellularVascularEndoLocation.Interstitial.ToString();
         _snapshot.MembraneLocation = MembraneLocation.BloodBrainBarrier.ToString();
         _snapshot.TissueLocation = TissueLocation.Interstitial.ToString();
         var enzymeFactory = A.Fake<IIndividualMoleculeFactory>();
         A.CallTo(() => _individualMoleculeFactoryResolver.FactoryFor<IndividualEnzyme>()).Returns(enzymeFactory);
         A.CallTo(() => enzymeFactory.CreateFor(_simulationSubject)).Returns(_enzyme);
         _relativeExpressionParameter1.Value = 0;
         _relativeExpressionParameterNorm1.Value = 0;

         A.CallTo(() => _parameterMapper.UpdateParameterFromSnapshot(_relativeExpressionParameter1, _relativeExpressionSnapshot1))
            .Invokes(x => _relativeExpressionParameter1.Value = _relativeExpressionSnapshot1.Value);
      }

      protected override void Because()
      {
         _newMolecule = sut.MapToModel(_snapshot, _simulationSubject) as IndividualEnzyme;
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newMolecule.ShouldNotBeNull();
      }

      [Observation]
      public void should_return_the_expected_molecule_with_the_matching_properties()
      {
         _newMolecule.IntracellularVascularEndoLocation.ShouldBeEqualTo(IntracellularVascularEndoLocation.Interstitial);
         _newMolecule.MembraneLocation.ShouldBeEqualTo(MembraneLocation.BloodBrainBarrier);
         _newMolecule.TissueLocation.ShouldBeEqualTo(TissueLocation.Interstitial);
      }

      [Observation]
      public void should_update_the_relative_expression_parameters_and_recalculate_the_norm_parameters()
      {
         _relativeExpressionParameter1.Value.ShouldBeEqualTo(0.5);
         _relativeExpressionParameterNorm1.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_mapping_a_valid_transporter_molecule_snahpshot_to_a_molecule : concern_for_MoleculeMapper
   {
      private ISimulationSubject _simulationSubject;
      private IndividualTransporter _newTransporter;

      protected override void Context()
      {
         base.Context();
         _simulationSubject = A.Fake<ISimulationSubject>();
         _snapshot = sut.MapToSnapshot(_transporter);

         _snapshot.TransportType = TransportType.PgpLike.ToString();
         var transporterFactory = A.Fake<IIndividualMoleculeFactory>();
         A.CallTo(() => _individualMoleculeFactoryResolver.FactoryFor<IndividualTransporter>()).Returns(transporterFactory);
         A.CallTo(() => transporterFactory.CreateFor(_simulationSubject)).Returns(_transporter);
      }

      protected override void Because()
      {
         _newTransporter = sut.MapToModel(_snapshot, _simulationSubject) as IndividualTransporter;
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newTransporter.ShouldNotBeNull();
      }

      [Observation]
      public void should_return_the_expected_molecule_with_the_matching_properties()
      {
         _newTransporter.TransportType.ShouldBeEqualTo(TransportType.PgpLike);
      }
   }

   public class When_mapping_a_valid_other_protein_molecule_snahpshot_to_a_molecule : concern_for_MoleculeMapper
   {
      private ISimulationSubject _simulationSubject;
      private IndividualOtherProtein _newOtherProtein;

      protected override void Context()
      {
         base.Context();
         _simulationSubject = A.Fake<ISimulationSubject>();
         _snapshot = sut.MapToSnapshot(_otherProtein);

         var individualOtherProteinFactory = A.Fake<IIndividualMoleculeFactory>();
         A.CallTo(() => _individualMoleculeFactoryResolver.FactoryFor<IndividualOtherProtein>()).Returns(individualOtherProteinFactory);
         A.CallTo(() => individualOtherProteinFactory.CreateFor(_simulationSubject)).Returns(_otherProtein);
      }

      protected override void Because()
      {
         _newOtherProtein = sut.MapToModel(_snapshot, _simulationSubject) as IndividualOtherProtein;
      }

      [Observation]
      public void should_return_a_molecule_having_the_expected_type()
      {
         _newOtherProtein.ShouldNotBeNull();
      }
   }
}