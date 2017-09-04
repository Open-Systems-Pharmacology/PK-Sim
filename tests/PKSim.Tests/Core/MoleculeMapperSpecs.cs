using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using Parameter = PKSim.Core.Snapshots.Parameter;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeMapper : ContextSpecification<MoleculeMapper>
   {
      private ParameterMapper _parameterMapper;
      protected IndividualEnzyme _enzyme;
      protected IndividualTransporter _transporter;
      protected Molecule _snapshot;
      private IParameter _enzymeParameter;
      private Parameter _enzymeParameterSnapshot;
      private MoleculeExpressionContainer _expressionContainer1;
      private IParameter _relativeExpressionParameter1;
      private IParameter _relativeExpressionParameterNotSet;
      private MoleculeExpressionContainer _expressionContainer2;
      protected LocalizedParameter _relativeExpressionSnapshot1;

      protected override void Context()
      {
         _parameterMapper = A.Fake<ParameterMapper>();
         sut = new MoleculeMapper(_parameterMapper);

         _enzyme = new IndividualEnzyme
         {
            Name = "Enzyme",
            Description = "Hellp"
         };

         _enzymeParameter = DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("HalfLife");
         _enzymeParameterSnapshot= new  Parameter();

         A.CallTo(() => _parameterMapper.MapToSnapshot(_enzymeParameter)).Returns(_enzymeParameterSnapshot);

         _expressionContainer1 = new MoleculeExpressionContainer{Name = "Exp Container1"};
         _expressionContainer2 = new MoleculeExpressionContainer{Name = "Exp Container2"};
         _enzyme.AddChildren(_expressionContainer1, _expressionContainer2);

         _relativeExpressionParameter1 = DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.Parameter.REL_EXP);
         _expressionContainer1.Add(_relativeExpressionParameter1);

         _relativeExpressionParameterNotSet = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(CoreConstants.Parameter.REL_EXP);
         _expressionContainer2.Add(_relativeExpressionParameterNotSet);

         _relativeExpressionSnapshot1 = new LocalizedParameter();
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
      [Observation]
      public void should_return_a_snapshot_having_the_expected_properties_set_for_transporter()
      {
      }
   }
}