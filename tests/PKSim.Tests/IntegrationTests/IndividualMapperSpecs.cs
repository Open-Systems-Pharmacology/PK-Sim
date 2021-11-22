using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.IntegrationTests
{
   public class When_loading_a_snapshot_file_containing_expression_created_in_v10 : ContextWithLoadedSnapshot
   {
      private Individual _individual;
      private ExpressionProfile _expressionProfileEnzyme;
      private ExpressionProfile _expressionProfileTransporter;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadSnapshot("ind_expression_v10");
         _individual = FindByName<Individual>("Ind");
         _expressionProfileEnzyme = FindByName<ExpressionProfile>("CYP3A4|Human|Ind");
         _expressionProfileTransporter = FindByName<ExpressionProfile>("UGT8|Human|Ind");
      }

      [Observation]
      public void should_have_created_an_expression_profile_based_on_the_value_defined_in_the_snapshot_file_for_the_enzyme()
      {
         _expressionProfileEnzyme.ShouldNotBeNull();
         var (enzyme, individual) = _expressionProfileEnzyme;
         enzyme.Ontogeny.Name.ShouldBeEqualTo("CYP3A4");
         enzyme.HalfLifeLiver.Value.ShouldBeEqualTo(22);
      }

      [Observation]
      public void should_have_created_an_expression_profile_based_on_the_value_defined_in_the_snapshot_file_for_the_transporter()
      {
         _expressionProfileTransporter.ShouldNotBeNull();
         var (transporter, individual) = _expressionProfileTransporter;
         transporter.Ontogeny.Name.ShouldBeEqualTo("UGT1A1");
         var expressionInBone = individual.Organism.EntityAt<TransporterExpressionContainer>("Bone", "Intracellular", transporter.Name);
         expressionInBone.ShouldNotBeNull();
         expressionInBone.RelativeExpressionParameter.Value.ShouldBeEqualTo(0.04442);
      }

      [Observation]
      public void should_have_added_the_newly_created_expression_profile_as_reference_in_the_individual()
      {
         _individual.AllExpressionProfiles().ShouldContain(_expressionProfileEnzyme, _expressionProfileTransporter);
      }

   }
}