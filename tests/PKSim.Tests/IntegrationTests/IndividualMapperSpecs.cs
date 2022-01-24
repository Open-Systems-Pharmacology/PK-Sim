﻿using OSPSuite.BDDHelper;
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
         enzyme.HalfLifeLiver.ValueInDisplayUnit.ShouldBeEqualTo(22);
         var expressionInBone = individual.Organism.EntityAt<MoleculeExpressionContainer>("Bone", "Intracellular", enzyme.Name);
         expressionInBone.RelativeExpressionParameter.Value.ShouldBeEqualTo(0.04749, 1e-2);
      }

      [Observation]
      public void should_have_created_an_expression_profile_based_on_the_value_defined_in_the_snapshot_file_for_the_transporter()
      {
         _expressionProfileTransporter.ShouldNotBeNull();
         var (transporter, individual) = _expressionProfileTransporter;
         transporter.Ontogeny.Name.ShouldBeEqualTo("UGT1A1");
         var expressionInBone = individual.Organism.EntityAt<TransporterExpressionContainer>("Bone", "Intracellular", transporter.Name);
         expressionInBone.ShouldNotBeNull();
         expressionInBone.RelativeExpressionParameter.Value.ShouldBeEqualTo(0.04442, 1e-2);
      }

      [Observation]
      public void should_have_added_the_newly_created_expression_profile_as_reference_in_the_individual()
      {
         _individual.AllExpressionProfiles().ShouldContain(_expressionProfileEnzyme, _expressionProfileTransporter);
      }
   }

   public class When_loading_a_snapshot_file_containing_expression_created_in_v11 : ContextWithLoadedSnapshot
   {
      private Individual _individual;
      private ExpressionProfile _expressionProfileEnzyme;
      private ExpressionProfile _expressionProfileTransporter;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadSnapshot("ind_expression_v11");
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
         enzyme.HalfLifeLiver.ValueInDisplayUnit.ShouldBeEqualTo(22);
         var expressionInBone = individual.Organism.EntityAt<MoleculeExpressionContainer>("Bone", "Intracellular", enzyme.Name);
         expressionInBone.RelativeExpressionParameter.Value.ShouldBeEqualTo(0.04749, 1e-2);
      }

      [Observation]
      public void should_have_created_an_expression_profile_based_on_the_value_defined_in_the_snapshot_file_for_the_transporter()
      {
         _expressionProfileTransporter.ShouldNotBeNull();
         var (transporter, individual) = _expressionProfileTransporter;
         transporter.Ontogeny.Name.ShouldBeEqualTo("UGT1A1");
         var expressionInBone = individual.Organism.EntityAt<TransporterExpressionContainer>("Bone", "Intracellular", transporter.Name);
         expressionInBone.ShouldNotBeNull();
         expressionInBone.RelativeExpressionParameter.Value.ShouldBeEqualTo(0.04442, 1e-2);
      }

      [Observation]
      public void should_have_added_the_newly_created_expression_profile_as_reference_in_the_individual()
      {
         _individual.AllExpressionProfiles().ShouldContain(_expressionProfileEnzyme, _expressionProfileTransporter);
      }
   }

   public class When_loading_a_snapshot_file_containing_expression_created_in_v9 : ContextWithLoadedSnapshot
   {
      private Individual _individual;
      private ExpressionProfile _expressionProfileCYP3A4;
      private ExpressionProfile _expressionProfileCYP2C8;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadSnapshot("ind_expression_v9");
         _individual = FindByName<Individual>("Ind");
         _expressionProfileCYP3A4 = FindByName<ExpressionProfile>("CYP3A4|Human|Ind");
         _expressionProfileCYP2C8 = FindByName<ExpressionProfile>("CYP2C8|Human|Ind");
      }

      [Observation]
      public void should_have_created_an_expression_profile_based_on_the_value_defined_in_the_snapshot_file_for_the_enzyme_cyp3A4()
      {
         _expressionProfileCYP3A4.ShouldNotBeNull();
         var (enzyme, individual) = _expressionProfileCYP3A4;
         enzyme.Ontogeny.Name.ShouldBeEqualTo("CYP3A4");
         enzyme.ReferenceConcentration.ValueInDisplayUnit.ShouldBeEqualTo(4.32);
         var expressionInBone = individual.Organism.EntityAt<MoleculeExpressionContainer>("Brain", "Intracellular", enzyme.Name);
         expressionInBone.RelativeExpressionParameter.Value.ShouldBeEqualTo(0.004168, 1e-2);
      }

      [Observation]
      public void should_have_created_an_expression_profile_based_on_the_value_defined_in_the_snapshot_file_for_the_enzyme_cyp2C8()
      {
         _expressionProfileCYP2C8.ShouldNotBeNull();
         var (enzyme, individual) = _expressionProfileCYP2C8;
         enzyme.Ontogeny.Name.ShouldBeEqualTo("CYP2C8");
         enzyme.ReferenceConcentration.ValueInDisplayUnit.ShouldBeEqualTo(2.56);
         enzyme.HalfLifeIntestine.ValueInDisplayUnit.ShouldBeEqualTo(23);
         var expressionInBone = individual.Organism.EntityAt<MoleculeExpressionContainer>("Kidney", "Intracellular", enzyme.Name);
         expressionInBone.RelativeExpressionParameter.Value.ShouldBeEqualTo(0.0012661, 1e-2);
      }

      [Observation]
      public void should_have_added_the_newly_created_expression_profile_as_reference_in_the_individual()
      {
         _individual.AllExpressionProfiles().ShouldContain(_expressionProfileCYP3A4, _expressionProfileCYP2C8);
      }
   }

}